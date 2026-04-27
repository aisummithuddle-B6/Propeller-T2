using Learning.Domain.Enums;

namespace Learning.Application.RiskRules;

public sealed record RiskAssessmentResult(
    Guid EmployeeId,
    Guid ProgramId,
    RiskLevel RiskLevel,
    IReadOnlyCollection<string> TriggeredRuleNames,
    string Reason);
