using Learning.Domain.Common;

namespace Learning.Domain.Entities;

public sealed class LearningProgram : Entity
{
    private LearningProgram()
    {
    }

    public LearningProgram(string code, string name, bool isMandatory, string complianceCategory)
    {
        Code = code;
        Name = name;
        IsMandatory = isMandatory;
        ComplianceCategory = complianceCategory;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsMandatory { get; private set; }
    public string ComplianceCategory { get; private set; } = string.Empty;
}
