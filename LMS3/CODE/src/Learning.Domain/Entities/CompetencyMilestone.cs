using Learning.Domain.Common;

namespace Learning.Domain.Entities;

public sealed class CompetencyMilestone : Entity
{
    private CompetencyMilestone()
    {
    }

    public CompetencyMilestone(Guid employeeId, Guid programId, string competencyCode, DateOnly dueDate)
    {
        EmployeeId = employeeId;
        ProgramId = programId;
        CompetencyCode = competencyCode;
        DueDate = dueDate;
    }

    public Guid EmployeeId { get; private set; }
    public Guid ProgramId { get; private set; }
    public string CompetencyCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = "NotStarted";
    public DateOnly DueDate { get; private set; }
    public DateOnly? CompletedOn { get; private set; }

    public void Complete(DateOnly completedOn, DateTimeOffset changedAt)
    {
        Status = "Completed";
        CompletedOn = completedOn;
        MarkUpdated(changedAt);
    }

    public void UpdateStatus(string status, DateOnly? completedOn, DateTimeOffset changedAt)
    {
        Status = status;
        CompletedOn = completedOn;
        MarkUpdated(changedAt);
    }
}
