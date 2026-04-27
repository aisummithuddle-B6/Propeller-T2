using Learning.Domain.Common;
using Learning.Domain.Enums;

namespace Learning.Domain.Entities;

public sealed class TrainingSession : Entity
{
    private TrainingSession()
    {
    }

    public TrainingSession(Guid programId, string sessionCode, string title, DateTimeOffset startsAt, DateTimeOffset endsAt)
    {
        ProgramId = programId;
        SessionCode = sessionCode;
        Title = title;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public Guid ProgramId { get; private set; }
    public string SessionCode { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public string? DeliveryMode { get; private set; }
    public string? TrainerName { get; private set; }
}

public sealed class AttendanceRecord : Entity
{
    private AttendanceRecord()
    {
    }

    public AttendanceRecord(Guid employeeId, Guid programId, Guid sessionId, AttendanceStatus status, decimal attendancePercentage)
    {
        EmployeeId = employeeId;
        ProgramId = programId;
        SessionId = sessionId;
        Status = status;
        AttendancePercentage = attendancePercentage;
    }

    public Guid EmployeeId { get; private set; }
    public Guid ProgramId { get; private set; }
    public Guid SessionId { get; private set; }
    public AttendanceStatus Status { get; private set; }
    public decimal AttendancePercentage { get; private set; }
    public DateOnly AttendanceDate { get; private set; }
    public string SourceSystem { get; private set; } = string.Empty;
    public string? SourceRecordId { get; private set; }

    public void SetSource(string sourceSystem, string? sourceRecordId, DateOnly attendanceDate)
    {
        SourceSystem = sourceSystem;
        SourceRecordId = sourceRecordId;
        AttendanceDate = attendanceDate;
        MarkUpdated(DateTimeOffset.UtcNow);
    }
}

public sealed class AssessmentResult : Entity
{
    private AssessmentResult()
    {
    }

    public AssessmentResult(Guid employeeId, Guid programId, Guid competencyId, decimal score, AssessmentStatus status)
    {
        EmployeeId = employeeId;
        ProgramId = programId;
        CompetencyId = competencyId;
        Score = score;
        Status = status;
    }

    public Guid EmployeeId { get; private set; }
    public Guid ProgramId { get; private set; }
    public Guid CompetencyId { get; private set; }
    public string AssessmentType { get; private set; } = string.Empty;
    public decimal Score { get; private set; }
    public string ScoreType { get; private set; } = "Percentage";
    public AssessmentStatus Status { get; private set; }
    public int AttemptNumber { get; private set; } = 1;
    public DateOnly AssessmentDate { get; private set; }
    public string SourceSystem { get; private set; } = string.Empty;
    public string? SourceRecordId { get; private set; }

    public void SetMetadata(
        string assessmentType,
        string scoreType,
        int attemptNumber,
        DateOnly assessmentDate,
        string sourceSystem,
        string? sourceRecordId)
    {
        AssessmentType = assessmentType;
        ScoreType = scoreType;
        AttemptNumber = attemptNumber;
        AssessmentDate = assessmentDate;
        SourceSystem = sourceSystem;
        SourceRecordId = sourceRecordId;
        MarkUpdated(DateTimeOffset.UtcNow);
    }
}

public sealed class Competency : Entity
{
    private Competency()
    {
    }

    public Competency(string code, string name, string category)
    {
        Code = code;
        Name = name;
        Category = category;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsComplianceRelevant { get; private set; }
}

public sealed class ProgramCompetency : Entity
{
    private ProgramCompetency()
    {
    }

    public ProgramCompetency(Guid programId, Guid competencyId, bool isRequired)
    {
        ProgramId = programId;
        CompetencyId = competencyId;
        IsRequired = isRequired;
    }

    public Guid ProgramId { get; private set; }
    public Guid CompetencyId { get; private set; }
    public bool IsRequired { get; private set; }
    public int SequenceNumber { get; private set; }
}
