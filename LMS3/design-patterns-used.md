# Design Patterns Used

This document lists the main design patterns used in the Corporate Learning Progress, Intervention & Compliance Tracking System scaffold.

## 1. Clean Architecture

**Where used**

- `Learning.Domain`
- `Learning.Application`
- `Learning.Infrastructure`
- `Learning.Api`
- `Learning.Workers`

**Reason**

Separates business rules, application contracts, infrastructure concerns, and API delivery. This keeps the system maintainable and allows database, API, or UI changes without rewriting core business logic.

## 2. Layered Architecture

**Where used**

- API layer: controllers
- Application layer: DTOs and service contracts
- Infrastructure layer: EF Core services and database context
- Domain layer: entities and enums

**Reason**

Provides a clear responsibility boundary. Controllers do not directly implement persistence logic, and infrastructure does not own product rules.

## 3. Repository-Like Data Access via EF Core DbContext

**Where used**

- `LearningDbContext`
- `EfLearnerProfileService`
- `EfDataIngestionService`

**Reason**

EF Core `DbContext` acts as the unit for querying and persisting entities. It avoids hand-written SQL for common CRUD/query operations while keeping database access centralized and testable.

## 4. Dependency Injection

**Where used**

- `Program.cs`
- `Learning.Application.DependencyInjection`
- `Learning.Infrastructure.DependencyInjection`

**Reason**

Allows services such as `ILearnerProfileService` and `IDataIngestionService` to be injected into controllers. This improves testability and keeps implementation choices replaceable.

## 5. Interface-Based Abstraction

**Where used**

- `ILearnerProfileService`
- `IDataIngestionService`

**Reason**

Controllers depend on contracts, not concrete classes. This allows swapping in-memory, EF-backed, mock, or future external-service implementations without changing API controllers.

## 6. DTO Pattern

**Where used**

- `LearnerProfileDto`
- `AttendanceSummaryDto`
- `AssessmentSummaryDto`
- `DataSourceDto`
- `ImportBatchDto`
- `IngestionResultDto`

**Reason**

DTOs shape API responses and requests without exposing domain entities directly. This protects the domain model and gives the frontend stable contracts.

## 7. Service Layer Pattern

**Where used**

- `EfLearnerProfileService`
- `EfDataIngestionService`
- `RiskAssessmentService`

**Reason**

Encapsulates business workflows such as learner profile aggregation, ingestion validation, import tracking, and risk assessment. This keeps controllers thin.

## 8. Unit of Work Pattern

**Where used**

- EF Core `LearningDbContext`
- Calls to `SaveChangesAsync`

**Reason**

Groups related database changes into a single transaction-like persistence operation. Useful for ingestion workflows where records, batches, errors, and reconciliation issues must be saved consistently.

## 9. Factory/Creation Method Style

**Where used**

- Entity constructors such as `Employee`, `LearningProgram`, `RiskRule`, `ImportBatch`, `Intervention`

**Reason**

Entities are created with required fields through constructors, reducing invalid object creation and making required business data explicit.

## 10. Encapsulation

**Where used**

- Private setters in domain entities
- Methods such as `AssignManager`, `UpdateProfile`, `Activate`, `Deactivate`, `Complete`, `UpdateStatus`, `Resolve`

**Reason**

Prevents arbitrary mutation of entity state. Changes happen through meaningful methods that describe business intent.

## 11. Strategy-Like Provider Selection

**Where used**

- `Learning.Infrastructure.DependencyInjection`
- `Database:Provider` configuration with `PostgreSql` or `SqlServer`

**Reason**

Allows switching database providers through configuration. The app can use PostgreSQL or SQL Server without changing API or application code.

## 12. Adapter Pattern

**Where used**

- Infrastructure services adapt EF Core/database access to application interfaces.
- Data ingestion service adapts incoming external records into internal entities.

**Reason**

Keeps external systems, database providers, and integration formats outside the core application contract.

## 13. Validation Pattern

**Where used**

- `EfDataIngestionService`
- Import error creation
- Reconciliation issue creation

**Reason**

Incoming records are checked for valid employee, program, session, competency, score range, attendance range, and duplicate source IDs before they are accepted.

## 14. Audit Trail Pattern

**Where used**

- `AuditLogEntry`
- Import batches and import errors
- Risk assessment history
- Intervention history

**Reason**

Supports compliance-ready traceability. The product must prove what changed, when, why, and from which source.

## 15. CQRS-Lite

**Where used**

- Learner profile read aggregation in `EfLearnerProfileService`
- Ingestion write operations in `EfDataIngestionService`

**Reason**

Read models are shaped for UI needs, while write operations focus on validating and persisting incoming records. This avoids forcing one model to serve both complex reads and writes.

## 16. Dashboard/Read Model Aggregation

**Where used**

- Learner profile aggregation
- Ingestion status aggregation
- `DashboardMetricSnapshot`

**Reason**

Dashboards need summarized data such as counts, statuses, risk, attendance, and compliance readiness. Aggregated read models make the UI simpler and faster to consume.

## 17. Idempotency/Duplicate Detection

**Where used**

- Data ingestion source record checks
- Duplicate `SourceRecordId` validation

**Reason**

Prevents repeated external imports from creating duplicate attendance or assessment records.

## 18. Error Queue Pattern

**Where used**

- `ImportError`
- `DataReconciliationIssue`
- Data ingestion dashboard

**Reason**

Invalid records are not silently discarded. They are captured for review, correction, and reprocessing.

## 19. Configuration Pattern

**Where used**

- `appsettings.json`
- `Database:Provider`
- connection strings
- `SystemConfiguration`

**Reason**

Keeps environment-specific behavior outside code and supports switching infrastructure or runtime settings safely.

## 20. Modular Monolith

**Where used**

- Single deployable backend with modules for Learners, Ingestion, Risk, Interventions, Reports, Audit, and Administration.

**Reason**

The product is still early-stage and the domains are related. A modular monolith gives clear boundaries without premature microservice complexity.

## 21. API Controller Pattern

**Where used**

- `LearnersController`
- `DataIngestionController`
- `HealthController`

**Reason**

Exposes product capabilities through HTTP endpoints while delegating business logic to application/infrastructure services.

## 22. Health Check Pattern

**Where used**

- `/health` endpoint
- `builder.Services.AddHealthChecks()`

**Reason**

Supports operational readiness checks for local, test, and future production deployment environments.

## 23. Frontend Component Pattern

**Where used**

- `MetricCard`
- `Section`
- `ModulePlaceholder`

**Reason**

Breaks the React UI into reusable components, keeping the dashboard easier to maintain and extend.

## 24. Frontend State and Effect Pattern

**Where used**

- React `useState`
- React `useEffect`
- API loading and refresh flows

**Reason**

Manages asynchronous API calls, loading state, error state, profile refresh, and ingestion dashboard refresh.

## 25. Feature Toggle Readiness

**Where used**

- Placeholder sections for modules not fully implemented yet

**Reason**

The UI exposes the full module map while allowing incremental implementation. This helps product demos and roadmap communication without pretending every module is complete.
