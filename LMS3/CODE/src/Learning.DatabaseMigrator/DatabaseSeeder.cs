using Learning.Domain.Entities;
using Learning.Domain.Enums;
using Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learning.DatabaseMigrator;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(LearningDbContext dbContext)
    {
        if (await dbContext.Employees.AnyAsync())
        {
            var insertedLearners = await EnsureAdditionalSampleLearnersAsync(dbContext);
            Console.WriteLine(insertedLearners == 0
                ? "Test data already exists. No additional sample learners were inserted."
                : $"Inserted additional sample learning data for {insertedLearners} learners.");
            return;
        }

        var adminUser = new SystemUser("local-admin", "Anika L&D Admin", "anika.admin@example.com");
        var trainerUser = new SystemUser("local-trainer", "Aarav Trainer", "aarav.trainer@example.com");
        var managerUser = new SystemUser("local-manager", "Priya Manager", "priya.manager@example.com");
        var complianceUser = new SystemUser("local-compliance", "Meera Compliance", "meera.compliance@example.com");

        var adminRole = new Role("LND_ADMIN", "L&D Administrator");
        var trainerRole = new Role("TRAINER", "Trainer");
        var managerRole = new Role("MANAGER", "Manager");
        var complianceRole = new Role("COMPLIANCE_OFFICER", "Compliance Officer");

        var learnerOne = new Employee("EMP-0001", "Sample Learner", "sample.learner@example.com", "Engineering", "Associate Engineer");
        var learnerTwo = new Employee("EMP-0002", "Riya Sharma", "riya.sharma@example.com", "Operations", "Operations Analyst");
        var learnerThree = new Employee("EMP-0003", "Dev Menon", "dev.menon@example.com", "Finance", "Finance Associate");

        learnerOne.AssignManager(managerUser.Id, DateTimeOffset.UtcNow);
        learnerTwo.AssignManager(managerUser.Id, DateTimeOffset.UtcNow);
        learnerThree.AssignManager(managerUser.Id, DateTimeOffset.UtcNow);

        var securityProgram = new LearningProgram("SEC-AWARE-2026", "Security Awareness 2026", isMandatory: true, "Security");
        var privacyProgram = new LearningProgram("DATA-PRIV-2026", "Data Privacy Essentials", isMandatory: true, "Privacy");

        var securityCompetency = new Competency("SEC-101", "Security Awareness", "Security");
        var privacyCompetency = new Competency("PRV-201", "Data Privacy Handling", "Privacy");
        var conductCompetency = new Competency("ETH-301", "Code of Conduct", "Ethics");

        var securitySessionOne = new TrainingSession(
            securityProgram.Id,
            "SEC-S1",
            "Security Basics",
            DateTimeOffset.UtcNow.AddDays(-20),
            DateTimeOffset.UtcNow.AddDays(-20).AddHours(2));
        var securitySessionTwo = new TrainingSession(
            securityProgram.Id,
            "SEC-S2",
            "Secure Data Handling",
            DateTimeOffset.UtcNow.AddDays(-10),
            DateTimeOffset.UtcNow.AddDays(-10).AddHours(2));
        var privacySessionOne = new TrainingSession(
            privacyProgram.Id,
            "PRV-S1",
            "Privacy Fundamentals",
            DateTimeOffset.UtcNow.AddDays(-8),
            DateTimeOffset.UtcNow.AddDays(-8).AddHours(2));

        var attendanceRecords = new[]
        {
            new AttendanceRecord(learnerOne.Id, securityProgram.Id, securitySessionOne.Id, AttendanceStatus.Present, 100m),
            new AttendanceRecord(learnerOne.Id, securityProgram.Id, securitySessionTwo.Id, AttendanceStatus.PartiallyAttended, 50m),
            new AttendanceRecord(learnerTwo.Id, securityProgram.Id, securitySessionOne.Id, AttendanceStatus.Present, 100m),
            new AttendanceRecord(learnerTwo.Id, privacyProgram.Id, privacySessionOne.Id, AttendanceStatus.Absent, 0m),
            new AttendanceRecord(learnerThree.Id, privacyProgram.Id, privacySessionOne.Id, AttendanceStatus.Present, 100m)
        };

        var assessmentResults = new[]
        {
            new AssessmentResult(learnerOne.Id, securityProgram.Id, securityCompetency.Id, 76m, AssessmentStatus.Passed),
            new AssessmentResult(learnerOne.Id, privacyProgram.Id, privacyCompetency.Id, 62m, AssessmentStatus.Failed),
            new AssessmentResult(learnerTwo.Id, securityProgram.Id, securityCompetency.Id, 84m, AssessmentStatus.Passed),
            new AssessmentResult(learnerThree.Id, privacyProgram.Id, privacyCompetency.Id, 91m, AssessmentStatus.Passed)
        };

        var competencyMilestones = new[]
        {
            new CompetencyMilestone(learnerOne.Id, securityProgram.Id, "SEC-101", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5))),
            new CompetencyMilestone(learnerOne.Id, privacyProgram.Id, "PRV-201", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))),
            new CompetencyMilestone(learnerTwo.Id, securityProgram.Id, "SEC-101", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10))),
            new CompetencyMilestone(learnerThree.Id, privacyProgram.Id, "PRV-201", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)))
        };
        competencyMilestones[0].Complete(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-6)), DateTimeOffset.UtcNow);

        var dataSourceLms = new DataSource("Corporate LMS", "LMS", "Learning Operations");
        var dataSourceAssessment = new DataSource("Assessment Platform", "Assessment", "Learning Operations");
        var dataSourceHr = new DataSource("HR System", "HR", "People Operations");
        var importBatch = new ImportBatch(dataSourceLms.Id, "Attendance", "attendance-april.csv");
        var importError = new ImportError(importBatch.Id, rowNumber: 14, "employee_id", "Employee reference not found");
        var reconciliationIssue = new DataReconciliationIssue(
            "AttendanceRecord",
            "ATT-EXT-404",
            "MissingEmployee",
            "Attendance source referenced an employee that does not exist in HR data.");

        var lowAttendanceRule = new RiskRule("Low attendance", "attendancePercentage < 75", RiskLevel.High, version: 1);
        var failedAssessmentRule = new RiskRule("Failed assessment", "latestAssessmentStatus == 'Failed'", RiskLevel.Medium, version: 1);
        var overdueCompetencyRule = new RiskRule("Overdue competency", "milestoneStatus == 'Overdue'", RiskLevel.Critical, version: 1);
        lowAttendanceRule.Activate(DateTimeOffset.UtcNow);
        failedAssessmentRule.Activate(DateTimeOffset.UtcNow);
        overdueCompetencyRule.Activate(DateTimeOffset.UtcNow);

        var riskAssessment = new RiskAssessment(
            learnerOne.Id,
            privacyProgram.Id,
            RiskLevel.Medium,
            "Learner has a failed privacy assessment and needs intervention.",
            DateTimeOffset.UtcNow);
        var riskTrigger = new RiskAssessmentTrigger(
            riskAssessment.Id,
            failedAssessmentRule.Id,
            failedAssessmentRule.Name,
            failedAssessmentRule.Version);

        var intervention = new Intervention(
            learnerOne.Id,
            privacyProgram.Id,
            "Coaching",
            trainerUser.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));
        var interventionNote = new InterventionNote(
            intervention.Id,
            trainerUser.Id,
            "Scheduled coaching session for data privacy handling gaps.");
        var interventionOutcome = new InterventionOutcome(
            intervention.Id,
            "Partially improved",
            "Learner completed coaching and is scheduled for reassessment.");

        var reportTemplate = new ReportTemplate("MANDATORY-COMPLIANCE", "Mandatory Training Compliance", "Compliance");
        var reportRequest = new ReportRequest(
            reportTemplate.Id,
            complianceUser.Id,
            "{\"program\":\"Security Awareness 2026\",\"department\":\"All\"}");
        var reportSubscription = new ReportSubscription(reportTemplate.Id, complianceUser.Id, "0 8 * * MON");

        var notification = new NotificationMessage(
            trainerUser.Id,
            "Email",
            "At-risk learner requires coaching",
            "Sample Learner has triggered the failed assessment risk rule.");
        var escalationRule = new EscalationRule(
            "Escalate critical overdue competency",
            "riskLevel == 'Critical' && daysOverdue > 7",
            "MANAGER");

        var dashboardSnapshots = new[]
        {
            new DashboardMetricSnapshot("AtRiskLearners", "Organization", 1m, DateTimeOffset.UtcNow),
            new DashboardMetricSnapshot("ComplianceCompletionRate", "Organization", 66.67m, DateTimeOffset.UtcNow),
            new DashboardMetricSnapshot("OpenInterventions", "Organization", 1m, DateTimeOffset.UtcNow)
        };

        var auditEntry = new AuditLogEntry(
            adminUser.Id,
            "SeedTestData",
            "Database",
            "learning");

        var lookupValues = new[]
        {
            new LookupValue("RiskLevel", "LOW", "Low"),
            new LookupValue("RiskLevel", "MEDIUM", "Medium"),
            new LookupValue("RiskLevel", "HIGH", "High"),
            new LookupValue("RiskLevel", "CRITICAL", "Critical"),
            new LookupValue("InterventionType", "COACHING", "Coaching"),
            new LookupValue("InterventionType", "REMEDIAL_TRAINING", "Remedial Training")
        };

        var configurations = new[]
        {
            new SystemConfiguration("RiskAssessment.Schedule", "0 */4 * * *", "Jobs"),
            new SystemConfiguration("Reports.DefaultRetentionDays", "2555", "Compliance"),
            new SystemConfiguration("Notifications.DefaultChannel", "Email", "Notifications")
        };

        dbContext.AddRange(adminUser, trainerUser, managerUser, complianceUser);
        dbContext.AddRange(adminRole, trainerRole, managerRole, complianceRole);
        dbContext.AddRange(
            new UserRoleAssignment(adminUser.Id, adminRole.Id, "Organization", "*"),
            new UserRoleAssignment(trainerUser.Id, trainerRole.Id, "Program", securityProgram.Code),
            new UserRoleAssignment(managerUser.Id, managerRole.Id, "Department", "Engineering"),
            new UserRoleAssignment(complianceUser.Id, complianceRole.Id, "Organization", "*"));
        dbContext.AddRange(learnerOne, learnerTwo, learnerThree);
        dbContext.AddRange(securityProgram, privacyProgram);
        dbContext.AddRange(securityCompetency, privacyCompetency, conductCompetency);
        dbContext.AddRange(
            new ProgramCompetency(securityProgram.Id, securityCompetency.Id, isRequired: true),
            new ProgramCompetency(securityProgram.Id, conductCompetency.Id, isRequired: true),
            new ProgramCompetency(privacyProgram.Id, privacyCompetency.Id, isRequired: true));
        dbContext.AddRange(securitySessionOne, securitySessionTwo, privacySessionOne);
        dbContext.AddRange(attendanceRecords);
        dbContext.AddRange(assessmentResults);
        dbContext.AddRange(competencyMilestones);
        dbContext.AddRange(dataSourceLms, dataSourceAssessment, dataSourceHr, importBatch, importError, reconciliationIssue);
        dbContext.AddRange(lowAttendanceRule, failedAssessmentRule, overdueCompetencyRule, riskAssessment, riskTrigger);
        dbContext.AddRange(intervention, interventionNote, interventionOutcome);
        dbContext.AddRange(reportTemplate, reportRequest, reportSubscription);
        dbContext.AddRange(notification, escalationRule);
        dbContext.AddRange(dashboardSnapshots);
        dbContext.Add(auditEntry);
        dbContext.AddRange(lookupValues);
        dbContext.AddRange(configurations);

        await dbContext.SaveChangesAsync();
        var additionalLearners = await EnsureAdditionalSampleLearnersAsync(dbContext);
        Console.WriteLine("Inserted test data for learning profiles, ingestion, risk, interventions, reporting, audit, notifications, and admin configuration.");
        Console.WriteLine($"Inserted additional sample learning data for {additionalLearners} learners.");
    }

    private static async Task<int> EnsureAdditionalSampleLearnersAsync(LearningDbContext dbContext)
    {
        var existingEmployeeNumbers = await dbContext.Employees
            .Select(employee => employee.EmployeeNumber)
            .ToListAsync();
        var existingEmployeeNumberSet = existingEmployeeNumbers.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var managerUser = await GetOrCreateSystemUserAsync(
            dbContext,
            "local-manager",
            "Priya Manager",
            "priya.manager@example.com");
        var trainerUser = await GetOrCreateSystemUserAsync(
            dbContext,
            "local-trainer",
            "Aarav Trainer",
            "aarav.trainer@example.com");

        var securityProgram = await GetOrCreateProgramAsync(
            dbContext,
            "SEC-AWARE-2026",
            "Security Awareness 2026",
            isMandatory: true,
            "Security");
        var privacyProgram = await GetOrCreateProgramAsync(
            dbContext,
            "DATA-PRIV-2026",
            "Data Privacy Essentials",
            isMandatory: true,
            "Privacy");

        var securityCompetency = await GetOrCreateCompetencyAsync(
            dbContext,
            "SEC-101",
            "Security Awareness",
            "Security");
        var privacyCompetency = await GetOrCreateCompetencyAsync(
            dbContext,
            "PRV-201",
            "Data Privacy Handling",
            "Privacy");

        var securitySessionOne = await GetOrCreateSessionAsync(
            dbContext,
            securityProgram.Id,
            "SEC-S1",
            "Security Basics",
            DateTimeOffset.UtcNow.AddDays(-20),
            DateTimeOffset.UtcNow.AddDays(-20).AddHours(2));
        var securitySessionTwo = await GetOrCreateSessionAsync(
            dbContext,
            securityProgram.Id,
            "SEC-S2",
            "Secure Data Handling",
            DateTimeOffset.UtcNow.AddDays(-10),
            DateTimeOffset.UtcNow.AddDays(-10).AddHours(2));
        var privacySessionOne = await GetOrCreateSessionAsync(
            dbContext,
            privacyProgram.Id,
            "PRV-S1",
            "Privacy Fundamentals",
            DateTimeOffset.UtcNow.AddDays(-8),
            DateTimeOffset.UtcNow.AddDays(-8).AddHours(2));

        var learnerSamples = new[]
        {
            new LearnerSeed("EMP-0101", "Kabir Rao", "kabir.rao@example.com", "Engineering", "Software Engineer", 100m, 92m, 88m, 81m, false),
            new LearnerSeed("EMP-0102", "Ananya Iyer", "ananya.iyer@example.com", "Operations", "Operations Specialist", 75m, 100m, 73m, 86m, false),
            new LearnerSeed("EMP-0103", "Farhan Khan", "farhan.khan@example.com", "Finance", "Finance Analyst", 50m, 0m, 64m, 58m, true),
            new LearnerSeed("EMP-0104", "Neha Gupta", "neha.gupta@example.com", "Human Resources", "HR Executive", 100m, 100m, 91m, 94m, false),
            new LearnerSeed("EMP-0105", "Vikram Nair", "vikram.nair@example.com", "Sales", "Account Executive", 50m, 75m, 69m, 72m, true),
            new LearnerSeed("EMP-0106", "Ishita Bose", "ishita.bose@example.com", "Compliance", "Compliance Analyst", 100m, 100m, 96m, 90m, false),
            new LearnerSeed("EMP-0107", "Rohan Patel", "rohan.patel@example.com", "Customer Support", "Support Lead", 75m, 50m, 82m, 61m, true),
            new LearnerSeed("EMP-0108", "Sneha Kulkarni", "sneha.kulkarni@example.com", "Product", "Product Analyst", 100m, 75m, 87m, 78m, false),
            new LearnerSeed("EMP-0109", "Arjun Sen", "arjun.sen@example.com", "Engineering", "Senior Engineer", 0m, 50m, 55m, 67m, true),
            new LearnerSeed("EMP-0110", "Meghna Das", "meghna.das@example.com", "Legal", "Legal Associate", 75m, 100m, 79m, 89m, false)
        };

        var insertedLearners = 0;
        foreach (var sample in learnerSamples)
        {
            if (existingEmployeeNumberSet.Contains(sample.EmployeeNumber))
            {
                continue;
            }

            var learner = new Employee(
                sample.EmployeeNumber,
                sample.FullName,
                sample.Email,
                sample.Department,
                sample.JobRole);
            learner.AssignManager(managerUser.Id, DateTimeOffset.UtcNow);
            dbContext.Add(learner);

            var securityAttendanceOne = CreateAttendance(
                learner.Id,
                securityProgram.Id,
                securitySessionOne.Id,
                sample.SecurityAttendancePercentage,
                $"ATT-{sample.EmployeeNumber}-SEC-S1",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)));
            var securityAttendanceTwo = CreateAttendance(
                learner.Id,
                securityProgram.Id,
                securitySessionTwo.Id,
                sample.SecurityAttendancePercentage,
                $"ATT-{sample.EmployeeNumber}-SEC-S2",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)));
            var privacyAttendance = CreateAttendance(
                learner.Id,
                privacyProgram.Id,
                privacySessionOne.Id,
                sample.PrivacyAttendancePercentage,
                $"ATT-{sample.EmployeeNumber}-PRV-S1",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-8)));

            var securityAssessment = CreateAssessment(
                learner.Id,
                securityProgram.Id,
                securityCompetency.Id,
                sample.SecurityAssessmentScore,
                $"ASM-{sample.EmployeeNumber}-SEC",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-6)));
            var privacyAssessment = CreateAssessment(
                learner.Id,
                privacyProgram.Id,
                privacyCompetency.Id,
                sample.PrivacyAssessmentScore,
                $"ASM-{sample.EmployeeNumber}-PRV",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-4)));

            var securityMilestone = new CompetencyMilestone(
                learner.Id,
                securityProgram.Id,
                securityCompetency.Code,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(sample.RequiresIntervention ? -3 : 10)));
            var privacyMilestone = new CompetencyMilestone(
                learner.Id,
                privacyProgram.Id,
                privacyCompetency.Code,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(sample.RequiresIntervention ? -1 : 14)));

            if (sample.SecurityAssessmentScore >= 70m)
            {
                securityMilestone.Complete(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)), DateTimeOffset.UtcNow);
            }

            if (sample.PrivacyAssessmentScore >= 70m)
            {
                privacyMilestone.Complete(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)), DateTimeOffset.UtcNow);
            }

            dbContext.AddRange(
                securityAttendanceOne,
                securityAttendanceTwo,
                privacyAttendance,
                securityAssessment,
                privacyAssessment,
                securityMilestone,
                privacyMilestone);

            if (sample.RequiresIntervention)
            {
                var riskAssessment = new RiskAssessment(
                    learner.Id,
                    sample.PrivacyAssessmentScore < 70m ? privacyProgram.Id : securityProgram.Id,
                    sample.SecurityAttendancePercentage < 50m ? RiskLevel.Critical : RiskLevel.High,
                    "Seeded learner has low attendance or failed assessment and requires follow-up.",
                    DateTimeOffset.UtcNow);
                var intervention = new Intervention(
                    learner.Id,
                    sample.PrivacyAssessmentScore < 70m ? privacyProgram.Id : securityProgram.Id,
                    "Coaching",
                    trainerUser.Id,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));
                var interventionNote = new InterventionNote(
                    intervention.Id,
                    trainerUser.Id,
                    "Seeded follow-up action for low attendance or failed assessment.");

                dbContext.AddRange(riskAssessment, intervention, interventionNote);
            }

            existingEmployeeNumberSet.Add(sample.EmployeeNumber);
            insertedLearners++;
        }

        if (insertedLearners > 0)
        {
            dbContext.Add(new AuditLogEntry(
                managerUser.Id,
                "SeedAdditionalSampleLearners",
                "Employee",
                "EMP-0101..EMP-0110"));
        }

        await dbContext.SaveChangesAsync();
        return insertedLearners;
    }

    private static async Task<SystemUser> GetOrCreateSystemUserAsync(
        LearningDbContext dbContext,
        string externalSubjectId,
        string displayName,
        string email)
    {
        var user = await dbContext.SystemUsers
            .FirstOrDefaultAsync(systemUser => systemUser.ExternalSubjectId == externalSubjectId);

        if (user is not null)
        {
            return user;
        }

        user = new SystemUser(externalSubjectId, displayName, email);
        dbContext.Add(user);
        return user;
    }

    private static async Task<LearningProgram> GetOrCreateProgramAsync(
        LearningDbContext dbContext,
        string code,
        string name,
        bool isMandatory,
        string complianceCategory)
    {
        var program = await dbContext.LearningPrograms
            .FirstOrDefaultAsync(learningProgram => learningProgram.Code == code);

        if (program is not null)
        {
            return program;
        }

        program = new LearningProgram(code, name, isMandatory, complianceCategory);
        dbContext.Add(program);
        return program;
    }

    private static async Task<Competency> GetOrCreateCompetencyAsync(
        LearningDbContext dbContext,
        string code,
        string name,
        string category)
    {
        var competency = await dbContext.Competencies
            .FirstOrDefaultAsync(existingCompetency => existingCompetency.Code == code);

        if (competency is not null)
        {
            return competency;
        }

        competency = new Competency(code, name, category);
        dbContext.Add(competency);
        return competency;
    }

    private static async Task<TrainingSession> GetOrCreateSessionAsync(
        LearningDbContext dbContext,
        Guid programId,
        string sessionCode,
        string title,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt)
    {
        var session = await dbContext.TrainingSessions
            .FirstOrDefaultAsync(trainingSession => trainingSession.ProgramId == programId && trainingSession.SessionCode == sessionCode);

        if (session is not null)
        {
            return session;
        }

        session = new TrainingSession(programId, sessionCode, title, startsAt, endsAt);
        dbContext.Add(session);
        return session;
    }

    private static AttendanceRecord CreateAttendance(
        Guid employeeId,
        Guid programId,
        Guid sessionId,
        decimal attendancePercentage,
        string sourceRecordId,
        DateOnly attendanceDate)
    {
        var attendance = new AttendanceRecord(
            employeeId,
            programId,
            sessionId,
            attendancePercentage == 0m
                ? AttendanceStatus.Absent
                : attendancePercentage < 75m
                    ? AttendanceStatus.PartiallyAttended
                    : AttendanceStatus.Present,
            attendancePercentage);
        attendance.SetSource("SeedData", sourceRecordId, attendanceDate);
        return attendance;
    }

    private static AssessmentResult CreateAssessment(
        Guid employeeId,
        Guid programId,
        Guid competencyId,
        decimal score,
        string sourceRecordId,
        DateOnly assessmentDate)
    {
        var assessment = new AssessmentResult(
            employeeId,
            programId,
            competencyId,
            score,
            score >= 70m ? AssessmentStatus.Passed : AssessmentStatus.Failed);
        assessment.SetMetadata("Quiz", "Percentage", 1, assessmentDate, "SeedData", sourceRecordId);
        return assessment;
    }

    private sealed record LearnerSeed(
        string EmployeeNumber,
        string FullName,
        string Email,
        string Department,
        string JobRole,
        decimal SecurityAttendancePercentage,
        decimal PrivacyAttendancePercentage,
        decimal SecurityAssessmentScore,
        decimal PrivacyAssessmentScore,
        bool RequiresIntervention);
}
