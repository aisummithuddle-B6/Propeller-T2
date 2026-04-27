using Learning.Application.Ingestion;
using Learning.Domain.Entities;
using Learning.Domain.Enums;
using Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learning.Infrastructure.Ingestion;

public sealed class EfDataIngestionService : IDataIngestionService
{
    private readonly LearningDbContext _dbContext;

    public EfDataIngestionService(LearningDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IngestionStatusDto> GetStatusAsync(CancellationToken cancellationToken)
    {
        return new IngestionStatusDto(
            await _dbContext.DataSources.CountAsync(cancellationToken),
            await _dbContext.ImportBatches.CountAsync(cancellationToken),
            await _dbContext.ImportBatches.SumAsync(batch => batch.FailedRecords, cancellationToken),
            await _dbContext.ImportErrors.CountAsync(error => !error.IsResolved, cancellationToken),
            await _dbContext.DataReconciliationIssues.CountAsync(issue => !issue.IsResolved, cancellationToken));
    }

    public async Task<IReadOnlyCollection<DataSourceDto>> GetDataSourcesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.DataSources
            .AsNoTracking()
            .OrderBy(source => source.Name)
            .Select(source => new DataSourceDto(
                source.Id,
                source.Name,
                source.SourceType,
                source.Owner,
                source.Endpoint,
                source.IsActive))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<DataSourceDto> CreateDataSourceAsync(CreateDataSourceRequest request, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.DataSources
            .FirstOrDefaultAsync(source => source.Name == request.Name, cancellationToken);

        if (existing is not null)
        {
            existing.SetEndpoint(request.Endpoint, DateTimeOffset.UtcNow);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ToDto(existing);
        }

        var dataSource = new DataSource(request.Name, request.SourceType, request.Owner);
        dataSource.SetEndpoint(request.Endpoint, DateTimeOffset.UtcNow);
        _dbContext.DataSources.Add(dataSource);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(dataSource);
    }

    public async Task<IReadOnlyCollection<ImportBatchDto>> GetImportBatchesAsync(CancellationToken cancellationToken)
    {
        return await (
            from batch in _dbContext.ImportBatches.AsNoTracking()
            join source in _dbContext.DataSources.AsNoTracking() on batch.DataSourceId equals source.Id
            orderby batch.CreatedAt descending
            select new ImportBatchDto(
                batch.Id,
                source.Name,
                batch.ImportType,
                batch.FileName,
                batch.Status.ToString(),
                batch.TotalRecords,
                batch.SuccessfulRecords,
                batch.FailedRecords,
                batch.StartedAt,
                batch.CompletedAt))
            .Take(25)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ImportErrorDto>> GetImportErrorsAsync(bool unresolvedOnly, CancellationToken cancellationToken)
    {
        var query = _dbContext.ImportErrors.AsNoTracking();
        if (unresolvedOnly)
        {
            query = query.Where(error => !error.IsResolved);
        }

        return await query
            .OrderByDescending(error => error.CreatedAt)
            .Select(error => new ImportErrorDto(
                error.Id,
                error.ImportBatchId,
                error.RowNumber,
                error.FieldName,
                error.ErrorMessage,
                error.RawValue,
                error.IsResolved))
            .Take(50)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DataReconciliationIssueDto>> GetReconciliationIssuesAsync(
        bool unresolvedOnly,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.DataReconciliationIssues.AsNoTracking();
        if (unresolvedOnly)
        {
            query = query.Where(issue => !issue.IsResolved);
        }

        return await query
            .OrderByDescending(issue => issue.CreatedAt)
            .Select(issue => new DataReconciliationIssueDto(
                issue.Id,
                issue.EntityType,
                issue.SourceRecordId,
                issue.IssueType,
                issue.Description,
                issue.IsResolved,
                issue.ResolutionNotes))
            .Take(50)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<ImportBatchDto> CreateImportBatchAsync(CreateImportBatchRequest request, CancellationToken cancellationToken)
    {
        var source = await RequireSourceAsync(request.DataSourceId, cancellationToken);
        var batch = new ImportBatch(request.DataSourceId, request.ImportType, request.FileName);
        batch.Start(DateTimeOffset.UtcNow);
        batch.Complete(totalRecords: 0, successfulRecords: 0, failedRecords: 0, DateTimeOffset.UtcNow);
        _dbContext.ImportBatches.Add(batch);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ImportBatchDto(batch.Id, source.Name, batch.ImportType, batch.FileName, batch.Status.ToString(), 0, 0, 0, batch.StartedAt, batch.CompletedAt);
    }

    public async Task<IngestionResultDto> IngestEmployeeAsync(IngestEmployeeRequest request, CancellationToken cancellationToken)
    {
        var source = await RequireSourceAsync(request.DataSourceId, cancellationToken);
        var batch = StartBatch(source.Id, "Employee", $"api-employee-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}");
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(item => item.EmployeeNumber == request.EmployeeNumber, cancellationToken);

        if (string.IsNullOrWhiteSpace(request.EmployeeNumber) || string.IsNullOrWhiteSpace(request.Email))
        {
            return await FailAsync(batch, "employee", "EmployeeNumber/Email", "Employee number and email are required.", request.EmployeeNumber, cancellationToken);
        }

        if (employee is null)
        {
            employee = new Employee(request.EmployeeNumber, request.FullName, request.Email, request.Department, request.JobRole);
            _dbContext.Employees.Add(employee);
        }
        else
        {
            employee.UpdateProfile(request.FullName, request.Email, request.Department, request.JobRole, DateTimeOffset.UtcNow);
        }

        if (!string.IsNullOrWhiteSpace(request.ManagerEmail))
        {
            var manager = await _dbContext.SystemUsers.FirstOrDefaultAsync(user => user.Email == request.ManagerEmail, cancellationToken);
            if (manager is null)
            {
                _dbContext.DataReconciliationIssues.Add(new DataReconciliationIssue("Employee", request.EmployeeNumber, "MissingManager", $"Manager '{request.ManagerEmail}' was not found."));
            }
            else
            {
                employee.AssignManager(manager.Id, DateTimeOffset.UtcNow);
            }
        }

        return await CompleteAsync(batch, successfulRecords: 1, failedRecords: 0, "Employee record ingested.", cancellationToken);
    }

    public async Task<IngestionResultDto> IngestAttendanceAsync(IngestAttendanceRequest request, CancellationToken cancellationToken)
    {
        var source = await RequireSourceAsync(request.DataSourceId, cancellationToken);
        var batch = StartBatch(source.Id, "Attendance", $"api-attendance-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}");
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(item => item.EmployeeNumber == request.EmployeeNumber, cancellationToken);
        var program = await _dbContext.LearningPrograms.FirstOrDefaultAsync(item => item.Code == request.ProgramCode, cancellationToken);
        var session = program is null
            ? null
            : await _dbContext.TrainingSessions.FirstOrDefaultAsync(item => item.ProgramId == program.Id && item.SessionCode == request.SessionCode, cancellationToken);

        if (employee is null) return await FailAsync(batch, "attendance", "EmployeeNumber", "Employee not found.", request.EmployeeNumber, cancellationToken);
        if (program is null) return await FailAsync(batch, "attendance", "ProgramCode", "Program not found.", request.ProgramCode, cancellationToken);
        if (session is null) return await FailAsync(batch, "attendance", "SessionCode", "Training session not found.", request.SessionCode, cancellationToken);
        if (request.AttendancePercentage is < 0 or > 100) return await FailAsync(batch, "attendance", "AttendancePercentage", "Attendance must be between 0 and 100.", request.AttendancePercentage.ToString(), cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SourceRecordId)
            && await _dbContext.AttendanceRecords.AnyAsync(item => item.SourceSystem == source.Name && item.SourceRecordId == request.SourceRecordId, cancellationToken))
        {
            return await FailAsync(batch, "attendance", "SourceRecordId", "Duplicate attendance source record.", request.SourceRecordId, cancellationToken);
        }

        var status = ParseEnum(request.Status, AttendanceStatus.Pending);
        var record = new AttendanceRecord(employee.Id, program.Id, session.Id, status, request.AttendancePercentage);
        record.SetSource(source.Name, request.SourceRecordId, request.AttendanceDate);
        _dbContext.AttendanceRecords.Add(record);

        return await CompleteAsync(batch, successfulRecords: 1, failedRecords: 0, "Attendance record ingested.", cancellationToken);
    }

    public async Task<IngestionResultDto> IngestAssessmentAsync(IngestAssessmentRequest request, CancellationToken cancellationToken)
    {
        var source = await RequireSourceAsync(request.DataSourceId, cancellationToken);
        var batch = StartBatch(source.Id, "Assessment", $"api-assessment-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}");
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(item => item.EmployeeNumber == request.EmployeeNumber, cancellationToken);
        var program = await _dbContext.LearningPrograms.FirstOrDefaultAsync(item => item.Code == request.ProgramCode, cancellationToken);
        var competency = await _dbContext.Competencies.FirstOrDefaultAsync(item => item.Code == request.CompetencyCode, cancellationToken);

        if (employee is null) return await FailAsync(batch, "assessment", "EmployeeNumber", "Employee not found.", request.EmployeeNumber, cancellationToken);
        if (program is null) return await FailAsync(batch, "assessment", "ProgramCode", "Program not found.", request.ProgramCode, cancellationToken);
        if (competency is null) return await FailAsync(batch, "assessment", "CompetencyCode", "Competency not found.", request.CompetencyCode, cancellationToken);
        if (request.Score is < 0 or > 100) return await FailAsync(batch, "assessment", "Score", "Score must be between 0 and 100.", request.Score.ToString(), cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SourceRecordId)
            && await _dbContext.AssessmentResults.AnyAsync(item => item.SourceSystem == source.Name && item.SourceRecordId == request.SourceRecordId, cancellationToken))
        {
            return await FailAsync(batch, "assessment", "SourceRecordId", "Duplicate assessment source record.", request.SourceRecordId, cancellationToken);
        }

        var status = ParseEnum(request.Status, AssessmentStatus.Pending);
        var result = new AssessmentResult(employee.Id, program.Id, competency.Id, request.Score, status);
        result.SetMetadata(request.AssessmentType, request.ScoreType, request.AttemptNumber, request.AssessmentDate, source.Name, request.SourceRecordId);
        _dbContext.AssessmentResults.Add(result);

        return await CompleteAsync(batch, successfulRecords: 1, failedRecords: 0, "Assessment record ingested.", cancellationToken);
    }

    public async Task<IngestionResultDto> IngestCompetencyMilestoneAsync(
        IngestCompetencyMilestoneRequest request,
        CancellationToken cancellationToken)
    {
        var source = await RequireSourceAsync(request.DataSourceId, cancellationToken);
        var batch = StartBatch(source.Id, "CompetencyMilestone", $"api-competency-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}");
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(item => item.EmployeeNumber == request.EmployeeNumber, cancellationToken);
        var program = await _dbContext.LearningPrograms.FirstOrDefaultAsync(item => item.Code == request.ProgramCode, cancellationToken);
        var competency = await _dbContext.Competencies.FirstOrDefaultAsync(item => item.Code == request.CompetencyCode, cancellationToken);

        if (employee is null) return await FailAsync(batch, "competency", "EmployeeNumber", "Employee not found.", request.EmployeeNumber, cancellationToken);
        if (program is null) return await FailAsync(batch, "competency", "ProgramCode", "Program not found.", request.ProgramCode, cancellationToken);
        if (competency is null) return await FailAsync(batch, "competency", "CompetencyCode", "Competency not found.", request.CompetencyCode, cancellationToken);

        var milestone = await _dbContext.CompetencyMilestones.FirstOrDefaultAsync(
            item => item.EmployeeId == employee.Id && item.ProgramId == program.Id && item.CompetencyCode == competency.Code,
            cancellationToken);

        if (milestone is null)
        {
            milestone = new CompetencyMilestone(employee.Id, program.Id, competency.Code, request.DueDate);
            _dbContext.CompetencyMilestones.Add(milestone);
        }

        milestone.UpdateStatus(request.Status, request.CompletedOn, DateTimeOffset.UtcNow);
        return await CompleteAsync(batch, successfulRecords: 1, failedRecords: 0, "Competency milestone ingested.", cancellationToken);
    }

    public async Task<bool> ResolveImportErrorAsync(Guid importErrorId, CancellationToken cancellationToken)
    {
        var error = await _dbContext.ImportErrors.FirstOrDefaultAsync(item => item.Id == importErrorId, cancellationToken);
        if (error is null) return false;
        error.Resolve(DateTimeOffset.UtcNow);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ResolveReconciliationIssueAsync(Guid reconciliationIssueId, string resolutionNotes, CancellationToken cancellationToken)
    {
        var issue = await _dbContext.DataReconciliationIssues.FirstOrDefaultAsync(item => item.Id == reconciliationIssueId, cancellationToken);
        if (issue is null) return false;
        issue.Resolve(resolutionNotes, DateTimeOffset.UtcNow);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TEnum ParseEnum<TEnum>(string value, TEnum fallback)
        where TEnum : struct
    {
        return Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;
    }

    private async Task<DataSource> RequireSourceAsync(Guid dataSourceId, CancellationToken cancellationToken)
    {
        return await _dbContext.DataSources.FirstOrDefaultAsync(source => source.Id == dataSourceId, cancellationToken)
            ?? throw new InvalidOperationException($"Data source '{dataSourceId}' was not found.");
    }

    private ImportBatch StartBatch(Guid dataSourceId, string importType, string fileName)
    {
        var batch = new ImportBatch(dataSourceId, importType, fileName);
        batch.Start(DateTimeOffset.UtcNow);
        _dbContext.ImportBatches.Add(batch);
        return batch;
    }

    private async Task<IngestionResultDto> FailAsync(
        ImportBatch batch,
        string entityType,
        string fieldName,
        string errorMessage,
        string? rawValue,
        CancellationToken cancellationToken)
    {
        var error = new ImportError(batch.Id, rowNumber: 1, fieldName, errorMessage);
        error.SetRawValue(rawValue);
        _dbContext.ImportErrors.Add(error);
        _dbContext.DataReconciliationIssues.Add(new DataReconciliationIssue(entityType, rawValue ?? "unknown", fieldName, errorMessage));
        batch.Fail(totalRecords: 1, failedRecords: 1, DateTimeOffset.UtcNow);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new IngestionResultDto(batch.Id, Accepted: false, errorMessage, SuccessfulRecords: 0, FailedRecords: 1);
    }

    private async Task<IngestionResultDto> CompleteAsync(
        ImportBatch batch,
        int successfulRecords,
        int failedRecords,
        string message,
        CancellationToken cancellationToken)
    {
        batch.Complete(totalRecords: successfulRecords + failedRecords, successfulRecords, failedRecords, DateTimeOffset.UtcNow);
        _dbContext.DashboardMetricSnapshots.Add(new DashboardMetricSnapshot("LastSuccessfulIngestion", batch.ImportType, successfulRecords, DateTimeOffset.UtcNow));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new IngestionResultDto(batch.Id, Accepted: failedRecords == 0, message, successfulRecords, failedRecords);
    }

    private static DataSourceDto ToDto(DataSource source)
    {
        return new DataSourceDto(source.Id, source.Name, source.SourceType, source.Owner, source.Endpoint, source.IsActive);
    }
}
