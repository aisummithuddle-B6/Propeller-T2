using Learning.Domain.Enums;

namespace Learning.Application.Learners;

public sealed class InMemoryLearnerProfileService : ILearnerProfileService
{
    private static readonly Guid SampleLearnerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly LearnerProfileDto SampleProfile = new(
        SampleLearnerId,
        "EMP-0001",
        "Sample Learner",
        "Learning and Development",
        "Associate",
        "Priya Manager",
        new[] { "Security Awareness 2026", "Data Privacy Essentials", "Code of Conduct" },
        new AttendanceSummaryDto(
            TotalSessions: 12,
            AttendedSessions: 9,
            AttendancePercentage: 75.0m,
            MissedSessions: 3,
            LastAttendanceDate: new DateOnly(2026, 4, 20)),
        new AssessmentSummaryDto(
            TotalAssessments: 4,
            AverageScore: 72.5m,
            PassedAssessments: 3,
            FailedAssessments: 1,
            LastAssessmentDate: new DateOnly(2026, 4, 18)),
        new[]
        {
            new CompetencyProgressDto(
                "SEC-101",
                "Security Awareness",
                "Completed",
                new DateOnly(2026, 4, 15),
                new DateOnly(2026, 4, 12)),
            new CompetencyProgressDto(
                "PRV-201",
                "Data Privacy Handling",
                "InProgress",
                new DateOnly(2026, 4, 30),
                CompletedOn: null),
            new CompetencyProgressDto(
                "ETH-301",
                "Code of Conduct",
                "Overdue",
                new DateOnly(2026, 4, 10),
                CompletedOn: null)
        },
        new RiskStatusDto(
            RiskLevel.Medium,
            "Learner has one overdue competency and attendance is close to the minimum threshold.",
            new[] { "Overdue competency", "Attendance near threshold" },
            new DateTimeOffset(2026, 4, 26, 20, 0, 0, TimeSpan.Zero)),
        new ComplianceReadinessDto(
            IsCompliant: false,
            RequiredCompetencies: 3,
            CompletedRequiredCompetencies: 1,
            OverdueCompetencies: 1,
            "Learner has pending required competencies."),
        new[]
        {
            new InterventionHistoryItemDto(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "Coaching",
                "Aarav Trainer",
                "InProgress",
                new DateOnly(2026, 4, 29),
                Outcome: null),
            new InterventionHistoryItemDto(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                "Remedial Training",
                "Meera L&D Admin",
                "OutcomeRecorded",
                new DateOnly(2026, 4, 22),
                "Assessment score improved from 62 to 76.")
        },
        new[]
        {
            new RiskHistoryItemDto(
                Guid.Parse("44444444-4444-4444-4444-444444444444"),
                RiskLevel.Medium,
                "Failed privacy assessment.",
                new DateTimeOffset(2026, 4, 26, 20, 0, 0, TimeSpan.Zero),
                new[] { "Failed assessment" })
        },
        new[]
        {
            new AttendanceRecordDto(
                Guid.Parse("55555555-5555-5555-5555-555555555555"),
                "Security Awareness 2026",
                "Security Basics",
                "Present",
                100m,
                new DateOnly(2026, 4, 10))
        },
        new[]
        {
            new AssessmentRecordDto(
                Guid.Parse("66666666-6666-6666-6666-666666666666"),
                "Data Privacy Essentials",
                "Data Privacy Handling",
                "Quiz",
                62m,
                "Failed",
                1,
                new DateOnly(2026, 4, 18))
        });

    public Task<LearnerProfileDto?> GetProfileAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<LearnerProfileDto?>(SampleProfile with { EmployeeId = employeeId });
    }

    public Task<IReadOnlyCollection<LearnerProfileSummaryDto>> GetLearnersAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<LearnerProfileSummaryDto> learners = new[]
        {
            new LearnerProfileSummaryDto(
                SampleLearnerId,
                SampleProfile.EmployeeNumber,
                SampleProfile.FullName,
                SampleProfile.Department,
                SampleProfile.JobRole,
                SampleProfile.RiskStatus.CurrentRiskLevel,
                SampleProfile.InterventionHistory.Count,
                SampleProfile.CompetencyProgress.Count(competency => competency.Status == "Completed"),
                SampleProfile.CompetencyProgress.Count(competency => competency.Status == "Overdue"))
        };

        return Task.FromResult(learners);
    }

    public async Task<AttendanceSummaryDto?> GetAttendanceSummaryAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.AttendanceSummary;
    }

    public async Task<AssessmentSummaryDto?> GetAssessmentSummaryAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.AssessmentSummary;
    }

    public async Task<IReadOnlyCollection<CompetencyProgressDto>?> GetCompetencyProgressAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.CompetencyProgress;
    }

    public async Task<RiskStatusDto?> GetRiskStatusAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.RiskStatus;
    }

    public async Task<IReadOnlyCollection<InterventionHistoryItemDto>?> GetInterventionHistoryAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.InterventionHistory;
    }

    public async Task<ComplianceReadinessDto?> GetComplianceReadinessAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.ComplianceReadiness;
    }

    public async Task<IReadOnlyCollection<RiskHistoryItemDto>?> GetRiskHistoryAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.RiskHistory;
    }

    public async Task<IReadOnlyCollection<AttendanceRecordDto>?> GetAttendanceRecordsAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.AttendanceRecords;
    }

    public async Task<IReadOnlyCollection<AssessmentRecordDto>?> GetAssessmentRecordsAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(employeeId, cancellationToken);
        return profile?.AssessmentRecords;
    }
}
