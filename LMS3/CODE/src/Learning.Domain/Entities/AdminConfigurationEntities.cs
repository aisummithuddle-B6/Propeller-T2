using Learning.Domain.Common;

namespace Learning.Domain.Entities;

public sealed class LookupValue : Entity
{
    private LookupValue()
    {
    }

    public LookupValue(string lookupType, string code, string displayName)
    {
        LookupType = lookupType;
        Code = code;
        DisplayName = displayName;
    }

    public string LookupType { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
}

public sealed class SystemConfiguration : Entity
{
    private SystemConfiguration()
    {
    }

    public SystemConfiguration(string key, string value, string category)
    {
        Key = key;
        Value = value;
        Category = category;
    }

    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public bool IsSecretReference { get; private set; }
}

public sealed class ReportSubscription : Entity
{
    private ReportSubscription()
    {
    }

    public ReportSubscription(Guid reportTemplateId, Guid subscriberUserId, string scheduleExpression)
    {
        ReportTemplateId = reportTemplateId;
        SubscriberUserId = subscriberUserId;
        ScheduleExpression = scheduleExpression;
    }

    public Guid ReportTemplateId { get; private set; }
    public Guid SubscriberUserId { get; private set; }
    public string ScheduleExpression { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
}
