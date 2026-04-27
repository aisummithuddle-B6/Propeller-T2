using ExcelDataReader;
using Learning.Application.Ingestion;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Learning.Api.Controllers;

[ApiController]
[Route("api/data-ingestion")]
public sealed class DataIngestionController : ControllerBase
{
    private readonly IDataIngestionService _dataIngestionService;

    public DataIngestionController(IDataIngestionService dataIngestionService)
    {
        _dataIngestionService = dataIngestionService;
    }

    [HttpGet("status")]
    public async Task<ActionResult<IngestionStatusDto>> GetStatus(CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.GetStatusAsync(cancellationToken));
    }

    [HttpGet("sources")]
    public async Task<ActionResult<IReadOnlyCollection<DataSourceDto>>> GetDataSources(CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.GetDataSourcesAsync(cancellationToken));
    }

    [HttpPost("sources")]
    public async Task<ActionResult<DataSourceDto>> CreateDataSource(
        CreateDataSourceRequest request,
        CancellationToken cancellationToken)
    {
        var source = await _dataIngestionService.CreateDataSourceAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetDataSources), new { id = source.Id }, source);
    }

    [HttpGet("imports")]
    public async Task<ActionResult<IReadOnlyCollection<ImportBatchDto>>> GetImportBatches(CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.GetImportBatchesAsync(cancellationToken));
    }

    [HttpPost("imports")]
    public async Task<ActionResult<ImportBatchDto>> CreateImportBatch(
        CreateImportBatchRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.CreateImportBatchAsync(request, cancellationToken));
    }

    [HttpGet("errors")]
    public async Task<ActionResult<IReadOnlyCollection<ImportErrorDto>>> GetImportErrors(
        [FromQuery] bool unresolvedOnly,
        CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.GetImportErrorsAsync(unresolvedOnly, cancellationToken));
    }

    [HttpPost("errors/{importErrorId:guid}/resolve")]
    public async Task<IActionResult> ResolveImportError(Guid importErrorId, CancellationToken cancellationToken)
    {
        var resolved = await _dataIngestionService.ResolveImportErrorAsync(importErrorId, cancellationToken);
        return resolved ? NoContent() : NotFound();
    }

    [HttpGet("reconciliation-issues")]
    public async Task<ActionResult<IReadOnlyCollection<DataReconciliationIssueDto>>> GetReconciliationIssues(
        [FromQuery] bool unresolvedOnly,
        CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.GetReconciliationIssuesAsync(unresolvedOnly, cancellationToken));
    }

    [HttpPost("reconciliation-issues/{issueId:guid}/resolve")]
    public async Task<IActionResult> ResolveReconciliationIssue(
        Guid issueId,
        [FromBody] ResolveReconciliationIssueRequest request,
        CancellationToken cancellationToken)
    {
        var resolved = await _dataIngestionService.ResolveReconciliationIssueAsync(
            issueId,
            request.ResolutionNotes,
            cancellationToken);

        return resolved ? NoContent() : NotFound();
    }

    [HttpPost("employees")]
    public async Task<ActionResult<IngestionResultDto>> IngestEmployee(
        IngestEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.IngestEmployeeAsync(request, cancellationToken));
    }

    [HttpPost("attendance")]
    public async Task<ActionResult<IngestionResultDto>> IngestAttendance(
        IngestAttendanceRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.IngestAttendanceAsync(request, cancellationToken));
    }

    [HttpPost("assessments")]
    public async Task<ActionResult<IngestionResultDto>> IngestAssessment(
        IngestAssessmentRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.IngestAssessmentAsync(request, cancellationToken));
    }

    [HttpPost("competency-milestones")]
    public async Task<ActionResult<IngestionResultDto>> IngestCompetencyMilestone(
        IngestCompetencyMilestoneRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _dataIngestionService.IngestCompetencyMilestoneAsync(request, cancellationToken));
    }

    [HttpPost("excel-upload")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<ExcelUploadResultDto>> UploadExcel(
        [FromForm] Guid dataSourceId,
        [FromForm] string importType,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("Upload a non-empty Excel file.");
        }

        var normalizedImportType = Normalize(importType);
        if (normalizedImportType is not ("employee" or "attendance" or "assessment" or "competencymilestone"))
        {
            return BadRequest("Import type must be Employee, Attendance, Assessment, or CompetencyMilestone.");
        }

        await using var stream = file.OpenReadStream();
        using var reader = ExcelReaderFactory.CreateReader(stream);

        if (!reader.Read())
        {
            return BadRequest("The Excel file must contain a header row.");
        }

        var headers = ReadHeaders(reader);
        var successfulRecords = 0;
        var failedRecords = 0;
        var messages = new List<string>();
        var rowNumber = 1;

        while (reader.Read())
        {
            rowNumber++;
            if (IsEmptyRow(reader))
            {
                continue;
            }

            try
            {
                var row = ReadRow(reader, headers);
                var result = normalizedImportType switch
                {
                    "employee" => await _dataIngestionService.IngestEmployeeAsync(ToEmployeeRequest(dataSourceId, row), cancellationToken),
                    "attendance" => await _dataIngestionService.IngestAttendanceAsync(ToAttendanceRequest(dataSourceId, row), cancellationToken),
                    "assessment" => await _dataIngestionService.IngestAssessmentAsync(ToAssessmentRequest(dataSourceId, row), cancellationToken),
                    "competencymilestone" => await _dataIngestionService.IngestCompetencyMilestoneAsync(ToCompetencyMilestoneRequest(dataSourceId, row), cancellationToken),
                    _ => throw new InvalidOperationException("Unsupported import type.")
                };

                successfulRecords += result.SuccessfulRecords;
                failedRecords += result.FailedRecords;
                messages.Add($"Row {rowNumber}: {result.Message}");
            }
            catch (Exception ex) when (ex is FormatException or InvalidOperationException or KeyNotFoundException)
            {
                failedRecords++;
                messages.Add($"Row {rowNumber}: {ex.Message}");
            }
        }

        return Ok(new ExcelUploadResultDto(
            file.FileName,
            importType,
            successfulRecords + failedRecords,
            successfulRecords,
            failedRecords,
            messages.Take(25).ToArray()));
    }

    private static Dictionary<int, string> ReadHeaders(IExcelDataReader reader)
    {
        var headers = new Dictionary<int, string>();
        for (var column = 0; column < reader.FieldCount; column++)
        {
            var value = reader.GetValue(column)?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                headers[column] = Normalize(value);
            }
        }

        return headers;
    }

    private static Dictionary<string, string> ReadRow(IExcelDataReader reader, Dictionary<int, string> headers)
    {
        var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (column, header) in headers)
        {
            row[header] = reader.GetValue(column)?.ToString()?.Trim() ?? string.Empty;
        }

        return row;
    }

    private static bool IsEmptyRow(IExcelDataReader reader)
    {
        for (var column = 0; column < reader.FieldCount; column++)
        {
            if (!string.IsNullOrWhiteSpace(reader.GetValue(column)?.ToString()))
            {
                return false;
            }
        }

        return true;
    }

    private static IngestEmployeeRequest ToEmployeeRequest(Guid dataSourceId, IReadOnlyDictionary<string, string> row)
    {
        return new IngestEmployeeRequest(
            dataSourceId,
            Required(row, "employeenumber"),
            Required(row, "fullname"),
            Required(row, "email"),
            Required(row, "department"),
            Required(row, "jobrole"),
            Optional(row, "manageremail"),
            Optional(row, "sourcerecordid"));
    }

    private static IngestAttendanceRequest ToAttendanceRequest(Guid dataSourceId, IReadOnlyDictionary<string, string> row)
    {
        return new IngestAttendanceRequest(
            dataSourceId,
            Required(row, "employeenumber"),
            Required(row, "programcode"),
            Required(row, "sessioncode"),
            Required(row, "status"),
            Decimal(row, "attendancepercentage"),
            Date(row, "attendancedate"),
            Optional(row, "sourcerecordid"));
    }

    private static IngestAssessmentRequest ToAssessmentRequest(Guid dataSourceId, IReadOnlyDictionary<string, string> row)
    {
        return new IngestAssessmentRequest(
            dataSourceId,
            Required(row, "employeenumber"),
            Required(row, "programcode"),
            Required(row, "competencycode"),
            Decimal(row, "score"),
            Required(row, "status"),
            Optional(row, "assessmenttype") ?? "Quiz",
            Optional(row, "scoretype") ?? "Percentage",
            Int(row, "attemptnumber", fallback: 1),
            Date(row, "assessmentdate"),
            Optional(row, "sourcerecordid"));
    }

    private static IngestCompetencyMilestoneRequest ToCompetencyMilestoneRequest(Guid dataSourceId, IReadOnlyDictionary<string, string> row)
    {
        return new IngestCompetencyMilestoneRequest(
            dataSourceId,
            Required(row, "employeenumber"),
            Required(row, "programcode"),
            Required(row, "competencycode"),
            Required(row, "status"),
            Date(row, "duedate"),
            OptionalDate(row, "completedon"),
            Optional(row, "sourcerecordid"));
    }

    private static string Required(IReadOnlyDictionary<string, string> row, string key)
    {
        return row.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new KeyNotFoundException($"Missing required column value '{key}'.");
    }

    private static string? Optional(IReadOnlyDictionary<string, string> row, string key)
    {
        return row.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;
    }

    private static DateOnly Date(IReadOnlyDictionary<string, string> row, string key)
    {
        var value = Required(row, key);
        return DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed
            : throw new FormatException($"Column '{key}' must be a valid date.");
    }

    private static DateOnly? OptionalDate(IReadOnlyDictionary<string, string> row, string key)
    {
        var value = Optional(row, key);
        if (value is null)
        {
            return null;
        }

        return DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed
            : throw new FormatException($"Column '{key}' must be a valid date.");
    }

    private static decimal Decimal(IReadOnlyDictionary<string, string> row, string key)
    {
        var value = Required(row, key);
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : throw new FormatException($"Column '{key}' must be a valid number.");
    }

    private static int Int(IReadOnlyDictionary<string, string> row, string key, int fallback)
    {
        var value = Optional(row, key);
        if (value is null)
        {
            return fallback;
        }

        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : throw new FormatException($"Column '{key}' must be a valid whole number.");
    }

    private static string Normalize(string value)
    {
        return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
    }
}

public sealed record ResolveReconciliationIssueRequest(string ResolutionNotes);

public sealed record ExcelUploadResultDto(
    string FileName,
    string ImportType,
    int TotalRecords,
    int SuccessfulRecords,
    int FailedRecords,
    IReadOnlyCollection<string> Messages);
