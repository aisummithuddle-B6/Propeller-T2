namespace Learning.Application.Learners;

public interface ILearnerProfileService
{
    Task<IReadOnlyCollection<LearnerProfileSummaryDto>> GetLearnersAsync(CancellationToken cancellationToken);

    Task<LearnerProfileDto?> GetProfileAsync(Guid employeeId, CancellationToken cancellationToken);

    Task<AttendanceSummaryDto?> GetAttendanceSummaryAsync(Guid employeeId, CancellationToken cancellationToken);

    Task<AssessmentSummaryDto?> GetAssessmentSummaryAsync(Guid employeeId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CompetencyProgressDto>?> GetCompetencyProgressAsync(
        Guid employeeId,
        CancellationToken cancellationToken);

    Task<RiskStatusDto?> GetRiskStatusAsync(Guid employeeId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<InterventionHistoryItemDto>?> GetInterventionHistoryAsync(
        Guid employeeId,
        CancellationToken cancellationToken);

    Task<ComplianceReadinessDto?> GetComplianceReadinessAsync(Guid employeeId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RiskHistoryItemDto>?> GetRiskHistoryAsync(
        Guid employeeId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AttendanceRecordDto>?> GetAttendanceRecordsAsync(
        Guid employeeId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AssessmentRecordDto>?> GetAssessmentRecordsAsync(
        Guid employeeId,
        CancellationToken cancellationToken);
}
