namespace Learning.Application.Ingestion;

public interface IDataIngestionService
{
    Task<IngestionStatusDto> GetStatusAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<DataSourceDto>> GetDataSourcesAsync(CancellationToken cancellationToken);

    Task<DataSourceDto> CreateDataSourceAsync(CreateDataSourceRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ImportBatchDto>> GetImportBatchesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ImportErrorDto>> GetImportErrorsAsync(bool unresolvedOnly, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<DataReconciliationIssueDto>> GetReconciliationIssuesAsync(
        bool unresolvedOnly,
        CancellationToken cancellationToken);

    Task<ImportBatchDto> CreateImportBatchAsync(CreateImportBatchRequest request, CancellationToken cancellationToken);

    Task<IngestionResultDto> IngestEmployeeAsync(IngestEmployeeRequest request, CancellationToken cancellationToken);

    Task<IngestionResultDto> IngestAttendanceAsync(IngestAttendanceRequest request, CancellationToken cancellationToken);

    Task<IngestionResultDto> IngestAssessmentAsync(IngestAssessmentRequest request, CancellationToken cancellationToken);

    Task<IngestionResultDto> IngestCompetencyMilestoneAsync(
        IngestCompetencyMilestoneRequest request,
        CancellationToken cancellationToken);

    Task<bool> ResolveImportErrorAsync(Guid importErrorId, CancellationToken cancellationToken);

    Task<bool> ResolveReconciliationIssueAsync(
        Guid reconciliationIssueId,
        string resolutionNotes,
        CancellationToken cancellationToken);
}
