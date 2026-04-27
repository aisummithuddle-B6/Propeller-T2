namespace Learning.Application.Ingestion;

public sealed record DataSourceDto(
    Guid Id,
    string Name,
    string SourceType,
    string Owner,
    string? Endpoint,
    bool IsActive);

public sealed record CreateDataSourceRequest(
    string Name,
    string SourceType,
    string Owner,
    string? Endpoint);

public sealed record ImportBatchDto(
    Guid Id,
    string DataSourceName,
    string ImportType,
    string FileName,
    string Status,
    int TotalRecords,
    int SuccessfulRecords,
    int FailedRecords,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt);

public sealed record ImportErrorDto(
    Guid Id,
    Guid ImportBatchId,
    int RowNumber,
    string FieldName,
    string ErrorMessage,
    string? RawValue,
    bool IsResolved);

public sealed record DataReconciliationIssueDto(
    Guid Id,
    string EntityType,
    string SourceRecordId,
    string IssueType,
    string Description,
    bool IsResolved,
    string? ResolutionNotes);

public sealed record IngestionStatusDto(
    int DataSources,
    int ImportBatches,
    int FailedRecords,
    int OpenErrors,
    int OpenReconciliationIssues);

public sealed record CreateImportBatchRequest(
    Guid DataSourceId,
    string ImportType,
    string FileName);

public sealed record IngestAttendanceRequest(
    Guid DataSourceId,
    string EmployeeNumber,
    string ProgramCode,
    string SessionCode,
    string Status,
    decimal AttendancePercentage,
    DateOnly AttendanceDate,
    string? SourceRecordId);

public sealed record IngestAssessmentRequest(
    Guid DataSourceId,
    string EmployeeNumber,
    string ProgramCode,
    string CompetencyCode,
    decimal Score,
    string Status,
    string AssessmentType,
    string ScoreType,
    int AttemptNumber,
    DateOnly AssessmentDate,
    string? SourceRecordId);

public sealed record IngestCompetencyMilestoneRequest(
    Guid DataSourceId,
    string EmployeeNumber,
    string ProgramCode,
    string CompetencyCode,
    string Status,
    DateOnly DueDate,
    DateOnly? CompletedOn,
    string? SourceRecordId);

public sealed record IngestEmployeeRequest(
    Guid DataSourceId,
    string EmployeeNumber,
    string FullName,
    string Email,
    string Department,
    string JobRole,
    string? ManagerEmail,
    string? SourceRecordId);

public sealed record IngestionResultDto(
    Guid ImportBatchId,
    bool Accepted,
    string Message,
    int SuccessfulRecords,
    int FailedRecords);
