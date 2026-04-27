using Learning.Application.RiskRules;
using Learning.Domain.Entities;
using Learning.Domain.Enums;
using Xunit;

namespace Learning.Tests;

public sealed class RiskAssessmentServiceTests
{
    [Fact]
    public void Assess_ReturnsHighestActiveRiskRuleSeverity()
    {
        var service = new RiskAssessmentService();
        var mediumRule = new RiskRule("Low assessment score", "score < 70", RiskLevel.Medium, version: 1);
        var highRule = new RiskRule("Overdue competency", "dueDate < today", RiskLevel.High, version: 1);

        mediumRule.Activate(DateTimeOffset.UtcNow);
        highRule.Activate(DateTimeOffset.UtcNow);

        var result = service.Assess(Guid.NewGuid(), Guid.NewGuid(), new[] { mediumRule, highRule });

        Assert.Equal(RiskLevel.High, result.RiskLevel);
        Assert.Contains("Overdue competency", result.TriggeredRuleNames);
    }
}
