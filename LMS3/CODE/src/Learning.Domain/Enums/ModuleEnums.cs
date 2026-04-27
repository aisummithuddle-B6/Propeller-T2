namespace Learning.Domain.Enums;

public enum AttendanceStatus
{
    Present = 0,
    Absent = 1,
    PartiallyAttended = 2,
    Excused = 3,
    Pending = 4
}

public enum AssessmentStatus
{
    Pending = 0,
    Passed = 1,
    Failed = 2,
    Waived = 3
}

public enum MilestoneStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Overdue = 3,
    Failed = 4,
    Waived = 5,
    Exempted = 6
}

public enum ImportStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    CompletedWithErrors = 3,
    Failed = 4
}

public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2,
    Cancelled = 3
}

public enum ReportStatus
{
    Requested = 0,
    Generating = 1,
    Completed = 2,
    Failed = 3
}
