using Learning.Domain.Common;
using Learning.Domain.Enums;

namespace Learning.Domain.Entities;

public sealed class RiskRule : Entity
{
    private RiskRule()
    {
    }

    public RiskRule(string name, string conditionExpression, RiskLevel severity, int version)
    {
        Name = name;
        ConditionExpression = conditionExpression;
        Severity = severity;
        Version = version;
    }

    public string Name { get; private set; } = string.Empty;
    public string ConditionExpression { get; private set; } = string.Empty;
    public RiskLevel Severity { get; private set; }
    public int Version { get; private set; }
    public bool IsActive { get; private set; }

    public void Activate(DateTimeOffset changedAt)
    {
        IsActive = true;
        MarkUpdated(changedAt);
    }

    public void Deactivate(DateTimeOffset changedAt)
    {
        IsActive = false;
        MarkUpdated(changedAt);
    }
}
