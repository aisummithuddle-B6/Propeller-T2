using Learning.Domain.Common;
using Learning.Domain.Enums;

namespace Learning.Domain.Entities;

public sealed class RiskAssessment : Entity
{
    private RiskAssessment()
    {
    }

    public RiskAssessment(Guid employeeId, Guid programId, RiskLevel riskLevel, string reason, DateTimeOffset assessedAt)
    {
        EmployeeId = employeeId;
        ProgramId = programId;
        RiskLevel = riskLevel;
        Reason = reason;
        AssessedAt = assessedAt;
    }

    public Guid EmployeeId { get; private set; }
    public Guid ProgramId { get; private set; }
    public Guid? CompetencyId { get; private set; }
    public RiskLevel RiskLevel { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset AssessedAt { get; private set; }
    public string SourceDataSnapshot { get; private set; } = "{}";
}

public sealed class RiskAssessmentTrigger : Entity
{
    private RiskAssessmentTrigger()
    {
    }

    public RiskAssessmentTrigger(Guid riskAssessmentId, Guid riskRuleId, string ruleName, int ruleVersion)
    {
        RiskAssessmentId = riskAssessmentId;
        RiskRuleId = riskRuleId;
        RuleName = ruleName;
        RuleVersion = ruleVersion;
    }

    public Guid RiskAssessmentId { get; private set; }
    public Guid RiskRuleId { get; private set; }
    public string RuleName { get; private set; } = string.Empty;
    public int RuleVersion { get; private set; }
    public string TriggerDetails { get; private set; } = "{}";
}

public sealed class InterventionNote : Entity
{
    private InterventionNote()
    {
    }

    public InterventionNote(Guid interventionId, Guid authorUserId, string note)
    {
        InterventionId = interventionId;
        AuthorUserId = authorUserId;
        Note = note;
    }

    public Guid InterventionId { get; private set; }
    public Guid AuthorUserId { get; private set; }
    public string Note { get; private set; } = string.Empty;
}

public sealed class InterventionOutcome : Entity
{
    private InterventionOutcome()
    {
    }

    public InterventionOutcome(Guid interventionId, string outcome, string measurement)
    {
        InterventionId = interventionId;
        Outcome = outcome;
        Measurement = measurement;
    }

    public Guid InterventionId { get; private set; }
    public string Outcome { get; private set; } = string.Empty;
    public string Measurement { get; private set; } = string.Empty;
    public DateOnly RecordedOn { get; private set; }
}
