using Learning.Domain.Common;

namespace Learning.Domain.Entities;

public sealed class Employee : Entity
{
    private Employee()
    {
    }

    public Employee(string employeeNumber, string fullName, string email, string department, string jobRole)
    {
        EmployeeNumber = employeeNumber;
        FullName = fullName;
        Email = email;
        Department = department;
        JobRole = jobRole;
    }

    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public string JobRole { get; private set; } = string.Empty;
    public Guid? ManagerId { get; private set; }

    public void AssignManager(Guid managerId, DateTimeOffset changedAt)
    {
        ManagerId = managerId;
        MarkUpdated(changedAt);
    }

    public void UpdateProfile(string fullName, string email, string department, string jobRole, DateTimeOffset changedAt)
    {
        FullName = fullName;
        Email = email;
        Department = department;
        JobRole = jobRole;
        MarkUpdated(changedAt);
    }
}
