using Learning.Domain.Common;
using Learning.Domain.Enums;

namespace Learning.Domain.Entities;

public sealed class Intervention : Entity
{
    private Intervention()
    {
    }

    public Intervention(Guid employeeId, Guid programId, string interventionType, Guid ownerId, DateOnly dueDate)
    {
        EmployeeId = employeeId;
        ProgramId = programId;
        InterventionType = interventionType;
        OwnerId = ownerId;
        DueDate = dueDate;
    }

    public Guid EmployeeId { get; private set; }
    public Guid ProgramId { get; private set; }
    public string InterventionType { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }
    public DateOnly DueDate { get; private set; }
    public InterventionStatus Status { get; private set; } = InterventionStatus.Planned;
    public string? Outcome { get; private set; }

    public void RecordOutcome(string outcome, DateTimeOffset changedAt)
    {
        Outcome = outcome;
        Status = InterventionStatus.OutcomeRecorded;
        MarkUpdated(changedAt);
    }
}
