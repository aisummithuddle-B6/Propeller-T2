using Learning.Domain.Enums;

namespace Learning.Application.Learners;

public sealed record LearnerProfileDto(
    Guid EmployeeId,
    string EmployeeNumber,
    string FullName,
    string Department,
    string JobRole,
    string ManagerName,
    IReadOnlyCollection<string> AssignedPrograms,
    AttendanceSummaryDto AttendanceSummary,
    AssessmentSummaryDto AssessmentSummary,
    IReadOnlyCollection<CompetencyProgressDto> CompetencyProgress,
    RiskStatusDto RiskStatus,
    ComplianceReadinessDto ComplianceReadiness,
    IReadOnlyCollection<InterventionHistoryItemDto> InterventionHistory,
    IReadOnlyCollection<RiskHistoryItemDto> RiskHistory,
    IReadOnlyCollection<AttendanceRecordDto> AttendanceRecords,
    IReadOnlyCollection<AssessmentRecordDto> AssessmentRecords);

public sealed record AttendanceSummaryDto(
    int TotalSessions,
    int AttendedSessions,
    decimal AttendancePercentage,
    int MissedSessions,
    DateOnly LastAttendanceDate);

public sealed record AssessmentSummaryDto(
    int TotalAssessments,
    decimal AverageScore,
    int PassedAssessments,
    int FailedAssessments,
    DateOnly LastAssessmentDate);

public sealed record CompetencyProgressDto(
    string CompetencyCode,
    string CompetencyName,
    string Status,
    DateOnly DueDate,
    DateOnly? CompletedOn);

public sealed record RiskStatusDto(
    RiskLevel CurrentRiskLevel,
    string Reason,
    IReadOnlyCollection<string> TriggeredRules,
    DateTimeOffset AssessedAt);

public sealed record InterventionHistoryItemDto(
    Guid InterventionId,
    string Type,
    string OwnerName,
    string Status,
    DateOnly DueDate,
    string? Outcome);

public sealed record ComplianceReadinessDto(
    bool IsCompliant,
    int RequiredCompetencies,
    int CompletedRequiredCompetencies,
    int OverdueCompetencies,
    string Summary);

public sealed record RiskHistoryItemDto(
    Guid RiskAssessmentId,
    RiskLevel RiskLevel,
    string Reason,
    DateTimeOffset AssessedAt,
    IReadOnlyCollection<string> TriggeredRules);

public sealed record AttendanceRecordDto(
    Guid AttendanceRecordId,
    string ProgramName,
    string SessionTitle,
    string Status,
    decimal AttendancePercentage,
    DateOnly AttendanceDate);

public sealed record AssessmentRecordDto(
    Guid AssessmentResultId,
    string ProgramName,
    string CompetencyName,
    string AssessmentType,
    decimal Score,
    string Status,
    int AttemptNumber,
    DateOnly AssessmentDate);

public sealed record LearnerProfileSummaryDto(
    Guid EmployeeId,
    string EmployeeNumber,
    string FullName,
    string Department,
    string JobRole,
    RiskLevel CurrentRiskLevel,
    int OpenInterventions,
    int CompletedCompetencies,
    int OverdueCompetencies);
