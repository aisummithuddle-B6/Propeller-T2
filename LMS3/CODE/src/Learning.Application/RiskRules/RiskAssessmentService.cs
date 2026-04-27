using Learning.Domain.Entities;
using Learning.Domain.Enums;

namespace Learning.Application.RiskRules;

public sealed class RiskAssessmentService
{
    public RiskAssessmentResult Assess(Guid employeeId, Guid programId, IEnumerable<RiskRule> activeRules)
    {
        var triggeredRules = activeRules
            .Where(rule => rule.IsActive)
            .OrderByDescending(rule => rule.Severity)
            .ToArray();

        if (triggeredRules.Length == 0)
        {
            return new RiskAssessmentResult(
                employeeId,
                programId,
                RiskLevel.None,
                Array.Empty<string>(),
                "No active risk rules were triggered.");
        }

        var highestSeverity = triggeredRules[0].Severity;

        return new RiskAssessmentResult(
            employeeId,
            programId,
            highestSeverity,
            triggeredRules.Select(rule => rule.Name).ToArray(),
            "Risk level is based on the highest active triggered rule.");
    }
}
