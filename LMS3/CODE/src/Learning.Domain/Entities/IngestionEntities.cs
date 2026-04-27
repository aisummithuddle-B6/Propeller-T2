using Learning.Domain.Common;
using Learning.Domain.Enums;

namespace Learning.Domain.Entities;

public sealed class DataSource : Entity
{
    private DataSource()
    {
    }

    public DataSource(string name, string sourceType, string owner)
    {
        Name = name;
        SourceType = sourceType;
        Owner = owner;
    }

    public string Name { get; private set; } = string.Empty;
    public string SourceType { get; private set; } = string.Empty;
    public string Owner { get; private set; } = string.Empty;
    public string? Endpoint { get; private set; }
    public bool IsActive { get; private set; } = true;

    public void SetEndpoint(string? endpoint, DateTimeOffset changedAt)
    {
        Endpoint = endpoint;
        MarkUpdated(changedAt);
    }
}

public sealed class ImportBatch : Entity
{
    private ImportBatch()
    {
    }

    public ImportBatch(Guid dataSourceId, string importType, string fileName)
    {
        DataSourceId = dataSourceId;
        ImportType = importType;
        FileName = fileName;
    }

    public Guid DataSourceId { get; private set; }
    public string ImportType { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public ImportStatus Status { get; private set; } = ImportStatus.Pending;
    public int TotalRecords { get; private set; }
    public int SuccessfulRecords { get; private set; }
    public int FailedRecords { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public void Start(DateTimeOffset startedAt)
    {
        Status = ImportStatus.Processing;
        StartedAt = startedAt;
        MarkUpdated(startedAt);
    }

    public void Complete(int totalRecords, int successfulRecords, int failedRecords, DateTimeOffset completedAt)
    {
        TotalRecords = totalRecords;
        SuccessfulRecords = successfulRecords;
        FailedRecords = failedRecords;
        Status = failedRecords > 0 ? ImportStatus.CompletedWithErrors : ImportStatus.Completed;
        CompletedAt = completedAt;
        MarkUpdated(completedAt);
    }

    public void Fail(int totalRecords, int failedRecords, DateTimeOffset completedAt)
    {
        TotalRecords = totalRecords;
        FailedRecords = failedRecords;
        Status = ImportStatus.Failed;
        CompletedAt = completedAt;
        MarkUpdated(completedAt);
    }
}

public sealed class ImportError : Entity
{
    private ImportError()
    {
    }

    public ImportError(Guid importBatchId, int rowNumber, string fieldName, string errorMessage)
    {
        ImportBatchId = importBatchId;
        RowNumber = rowNumber;
        FieldName = fieldName;
        ErrorMessage = errorMessage;
    }

    public Guid ImportBatchId { get; private set; }
    public int RowNumber { get; private set; }
    public string FieldName { get; private set; } = string.Empty;
    public string ErrorMessage { get; private set; } = string.Empty;
    public string? RawValue { get; private set; }
    public bool IsResolved { get; private set; }

    public void SetRawValue(string? rawValue)
    {
        RawValue = rawValue;
    }

    public void Resolve(DateTimeOffset changedAt)
    {
        IsResolved = true;
        MarkUpdated(changedAt);
    }
}

public sealed class DataReconciliationIssue : Entity
{
    private DataReconciliationIssue()
    {
    }

    public DataReconciliationIssue(string entityType, string sourceRecordId, string issueType, string description)
    {
        EntityType = entityType;
        SourceRecordId = sourceRecordId;
        IssueType = issueType;
        Description = description;
    }

    public string EntityType { get; private set; } = string.Empty;
    public string SourceRecordId { get; private set; } = string.Empty;
    public string IssueType { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsResolved { get; private set; }
    public string? ResolutionNotes { get; private set; }

    public void Resolve(string resolutionNotes, DateTimeOffset changedAt)
    {
        ResolutionNotes = resolutionNotes;
        IsResolved = true;
        MarkUpdated(changedAt);
    }
}
