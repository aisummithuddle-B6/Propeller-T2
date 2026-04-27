using Learning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Learning.Infrastructure.Persistence;

public sealed class LearningDbContext : DbContext
{
    public LearningDbContext(DbContextOptions<LearningDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LearningProgram> LearningPrograms => Set<LearningProgram>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<AssessmentResult> AssessmentResults => Set<AssessmentResult>();
    public DbSet<Competency> Competencies => Set<Competency>();
    public DbSet<CompetencyMilestone> CompetencyMilestones => Set<CompetencyMilestone>();
    public DbSet<ProgramCompetency> ProgramCompetencies => Set<ProgramCompetency>();
    public DbSet<DataSource> DataSources => Set<DataSource>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();
    public DbSet<ImportError> ImportErrors => Set<ImportError>();
    public DbSet<DataReconciliationIssue> DataReconciliationIssues => Set<DataReconciliationIssue>();
    public DbSet<RiskRule> RiskRules => Set<RiskRule>();
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();
    public DbSet<RiskAssessmentTrigger> RiskAssessmentTriggers => Set<RiskAssessmentTrigger>();
    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<InterventionNote> InterventionNotes => Set<InterventionNote>();
    public DbSet<InterventionOutcome> InterventionOutcomes => Set<InterventionOutcome>();
    public DbSet<NotificationMessage> NotificationMessages => Set<NotificationMessage>();
    public DbSet<EscalationRule> EscalationRules => Set<EscalationRule>();
    public DbSet<DashboardMetricSnapshot> DashboardMetricSnapshots => Set<DashboardMetricSnapshot>();
    public DbSet<ReportTemplate> ReportTemplates => Set<ReportTemplate>();
    public DbSet<ReportRequest> ReportRequests => Set<ReportRequest>();
    public DbSet<ReportSubscription> ReportSubscriptions => Set<ReportSubscription>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();
    public DbSet<SystemUser> SystemUsers => Set<SystemUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();
    public DbSet<LookupValue> LookupValues => Set<LookupValue>();
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasIndex(employee => employee.EmployeeNumber)
            .IsUnique();

        modelBuilder.Entity<LearningProgram>()
            .HasIndex(program => program.Code)
            .IsUnique();

        modelBuilder.Entity<TrainingSession>()
            .HasIndex(session => new { session.ProgramId, session.SessionCode })
            .IsUnique();

        modelBuilder.Entity<AttendanceRecord>()
            .Property(record => record.Status)
            .HasConversion<string>();

        modelBuilder.Entity<AttendanceRecord>()
            .HasIndex(record => new { record.EmployeeId, record.ProgramId, record.SessionId });

        modelBuilder.Entity<AssessmentResult>()
            .Property(result => result.Status)
            .HasConversion<string>();

        modelBuilder.Entity<AssessmentResult>()
            .HasIndex(result => new { result.EmployeeId, result.ProgramId, result.CompetencyId });

        modelBuilder.Entity<Competency>()
            .HasIndex(competency => competency.Code)
            .IsUnique();

        modelBuilder.Entity<CompetencyMilestone>()
            .HasIndex(milestone => new { milestone.EmployeeId, milestone.ProgramId, milestone.CompetencyCode });

        modelBuilder.Entity<ProgramCompetency>()
            .HasIndex(programCompetency => new { programCompetency.ProgramId, programCompetency.CompetencyId })
            .IsUnique();

        modelBuilder.Entity<DataSource>()
            .HasIndex(source => source.Name)
            .IsUnique();

        modelBuilder.Entity<ImportBatch>()
            .Property(batch => batch.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ImportError>()
            .HasIndex(error => error.ImportBatchId);

        modelBuilder.Entity<RiskRule>()
            .Property(rule => rule.Severity)
            .HasConversion<string>();

        modelBuilder.Entity<RiskAssessment>()
            .Property(assessment => assessment.RiskLevel)
            .HasConversion<string>();

        modelBuilder.Entity<RiskAssessment>()
            .HasIndex(assessment => new { assessment.EmployeeId, assessment.ProgramId, assessment.AssessedAt });

        modelBuilder.Entity<RiskAssessmentTrigger>()
            .HasIndex(trigger => trigger.RiskAssessmentId);

        modelBuilder.Entity<Intervention>()
            .Property(intervention => intervention.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Intervention>()
            .HasIndex(intervention => new { intervention.EmployeeId, intervention.ProgramId, intervention.Status });

        modelBuilder.Entity<NotificationMessage>()
            .Property(notification => notification.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ReportRequest>()
            .Property(request => request.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ReportTemplate>()
            .HasIndex(template => template.Code)
            .IsUnique();

        modelBuilder.Entity<AuditLogEntry>()
            .HasIndex(entry => new { entry.EntityType, entry.EntityId });

        modelBuilder.Entity<SystemUser>()
            .HasIndex(user => user.ExternalSubjectId)
            .IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(role => role.Code)
            .IsUnique();

        modelBuilder.Entity<UserRoleAssignment>()
            .HasIndex(assignment => new { assignment.UserId, assignment.RoleId, assignment.ScopeType, assignment.ScopeValue })
            .IsUnique();

        modelBuilder.Entity<LookupValue>()
            .HasIndex(value => new { value.LookupType, value.Code })
            .IsUnique();

        modelBuilder.Entity<SystemConfiguration>()
            .HasIndex(configuration => configuration.Key)
            .IsUnique();
    }
}
