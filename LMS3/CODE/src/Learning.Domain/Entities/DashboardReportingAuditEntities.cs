using Learning.Domain.Common;
using Learning.Domain.Enums;

namespace Learning.Domain.Entities;

public sealed class DashboardMetricSnapshot : Entity
{
    private DashboardMetricSnapshot()
    {
    }

    public DashboardMetricSnapshot(string metricName, string scope, decimal value, DateTimeOffset capturedAt)
    {
        MetricName = metricName;
        Scope = scope;
        Value = value;
        CapturedAt = capturedAt;
    }

    public string MetricName { get; private set; } = string.Empty;
    public string Scope { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public DateTimeOffset CapturedAt { get; private set; }
    public string DimensionsJson { get; private set; } = "{}";
}

public sealed class ReportTemplate : Entity
{
    private ReportTemplate()
    {
    }

    public ReportTemplate(string code, string name, string reportType)
    {
        Code = code;
        Name = name;
        ReportType = reportType;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string ReportType { get; private set; } = string.Empty;
    public string DefinitionJson { get; private set; } = "{}";
    public bool IsActive { get; private set; } = true;
}

public sealed class ReportRequest : Entity
{
    private ReportRequest()
    {
    }

    public ReportRequest(Guid reportTemplateId, Guid requestedByUserId, string filtersJson)
    {
        ReportTemplateId = reportTemplateId;
        RequestedByUserId = requestedByUserId;
        FiltersJson = filtersJson;
    }

    public Guid ReportTemplateId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public string FiltersJson { get; private set; } = "{}";
    public ReportStatus Status { get; private set; } = ReportStatus.Requested;
    public DateTimeOffset? CompletedAt { get; private set; }
    public string? OutputUri { get; private set; }
}

public sealed class AuditLogEntry : Entity
{
    private AuditLogEntry()
    {
    }

    public AuditLogEntry(Guid actorUserId, string action, string entityType, string entityId)
    {
        ActorUserId = actorUserId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
    }

    public Guid ActorUserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string BeforeJson { get; private set; } = "{}";
    public string AfterJson { get; private set; } = "{}";
    public string? CorrelationId { get; private set; }
    public string? IpAddress { get; private set; }
}
