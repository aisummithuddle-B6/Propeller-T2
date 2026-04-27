using Learning.Domain.Common;
using Learning.Domain.Enums;

namespace Learning.Domain.Entities;

public sealed class NotificationMessage : Entity
{
    private NotificationMessage()
    {
    }

    public NotificationMessage(Guid recipientUserId, string channel, string subject, string body)
    {
        RecipientUserId = recipientUserId;
        Channel = channel;
        Subject = subject;
        Body = body;
    }

    public Guid RecipientUserId { get; private set; }
    public string Channel { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; } = NotificationStatus.Pending;
    public DateTimeOffset? SentAt { get; private set; }
}

public sealed class EscalationRule : Entity
{
    private EscalationRule()
    {
    }

    public EscalationRule(string name, string triggerExpression, string escalationTargetRole)
    {
        Name = name;
        TriggerExpression = triggerExpression;
        EscalationTargetRole = escalationTargetRole;
    }

    public string Name { get; private set; } = string.Empty;
    public string TriggerExpression { get; private set; } = string.Empty;
    public string EscalationTargetRole { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
}

public sealed class SystemUser : Entity
{
    private SystemUser()
    {
    }

    public SystemUser(string externalSubjectId, string displayName, string email)
    {
        ExternalSubjectId = externalSubjectId;
        DisplayName = displayName;
        Email = email;
    }

    public string ExternalSubjectId { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
}

public sealed class Role : Entity
{
    private Role()
    {
    }

    public Role(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
}

public sealed class UserRoleAssignment : Entity
{
    private UserRoleAssignment()
    {
    }

    public UserRoleAssignment(Guid userId, Guid roleId, string scopeType, string scopeValue)
    {
        UserId = userId;
        RoleId = roleId;
        ScopeType = scopeType;
        ScopeValue = scopeValue;
    }

    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public string ScopeType { get; private set; } = string.Empty;
    public string ScopeValue { get; private set; } = string.Empty;
}
