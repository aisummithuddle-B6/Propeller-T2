# Architectural Patterns Used

This document lists the architectural patterns used in the Corporate Learning Progress, Intervention & Compliance Tracking System and the reason for each pattern.

## 1. Modular Monolith

**Where used**

- Single backend application under `Learning.Api`
- Feature areas separated into Learner Profiles, Data Ingestion, Risk, Interventions, Reporting, Audit, and Administration

**Reason**

The product is still early-stage and the modules are strongly related. A modular monolith keeps deployment simple while still giving clear internal boundaries. It avoids premature microservice complexity.

## 2. Clean Architecture

**Where used**

- `Learning.Domain`
- `Learning.Application`
- `Learning.Infrastructure`
- `Learning.Api`

**Reason**

Keeps business concepts independent from frameworks, databases, and API transport. This makes the system easier to test, change, and extend.

## 3. Layered Architecture

**Where used**

- API layer: HTTP controllers
- Application layer: service contracts and DTOs
- Domain layer: entities and enums
- Infrastructure layer: EF Core, SQL Server/PostgreSQL access, external adapters

**Reason**

Creates clear responsibility boundaries. API controllers do not directly own persistence logic, and infrastructure does not define business intent.

## 4. API-First Architecture

**Where used**

- Learner APIs under `/api/learners`
- Data ingestion APIs under `/api/data-ingestion`
- Swagger/OpenAPI enabled in development

**Reason**

The product depends on integrations with LMS, assessment systems, HR systems, and frontend dashboards. API-first design creates clear contracts for internal and external consumers.

## 5. Database-Backed Enterprise Application

**Where used**

- SQL Server LocalDB for local development
- Configurable database provider for SQL Server or PostgreSQL
- EF Core `LearningDbContext`

**Reason**

The product needs strong relational consistency for employees, attendance, assessments, competencies, interventions, audit logs, and compliance reports.

## 6. Configurable Infrastructure Architecture

**Where used**

- `Database:Provider`
- `ConnectionStrings:SqlServer`
- `ConnectionStrings:Postgres`

**Reason**

Allows the same application to run on different database providers without changing API or business code. This supports local development, enterprise SQL Server, or cloud PostgreSQL deployments.

## 7. Service-Oriented Internal Architecture

**Where used**

- `ILearnerProfileService`
- `IDataIngestionService`
- `EfLearnerProfileService`
- `EfDataIngestionService`

**Reason**

Each major capability has a service boundary. This keeps controllers thin and allows features to evolve independently inside the modular monolith.

## 8. Domain-Centric Architecture

**Where used**

- Domain entities such as `Employee`, `LearningProgram`, `Competency`, `RiskRule`, `Intervention`, `ImportBatch`, and `AuditLogEntry`

**Reason**

The code model reflects business concepts from the problem statement. This makes the system easier for product owners, architects, and developers to reason about.

## 9. Integration-Centric Architecture

**Where used**

- `DataSource`
- `ImportBatch`
- `ImportError`
- `DataReconciliationIssue`
- Data ingestion API endpoints

**Reason**

The problem statement says learning data is fragmented across LMS platforms, assessment systems, and trainer notes. Integration is therefore a core architectural concern, not an afterthought.

## 10. Eventual Processing / Background-Ready Architecture

**Where used**

- `Learning.Workers`
- Import batch model
- Risk recalculation readiness
- Report generation readiness
- Notification readiness

**Reason**

Imports, risk recalculation, report generation, and notifications can become long-running or scheduled operations. The architecture prepares for background processing without blocking user requests.

## 11. Read Model Aggregation

**Where used**

- Learner profile API aggregates attendance, assessments, competencies, risk, compliance, interventions, and drill-down records.
- Data ingestion dashboard aggregates sources, imports, errors, and reconciliation issues.

**Reason**

Dashboards and profile pages need data from multiple tables. Read model aggregation gives the frontend useful, task-oriented responses instead of forcing it to call many low-level endpoints.

## 12. CQRS-Lite

**Where used**

- Learner profile reads are optimized for display.
- Data ingestion writes focus on validation, persistence, and import tracking.

**Reason**

Separating read concerns from write concerns keeps complex dashboard/profile queries from polluting ingestion workflows. It gives CQRS benefits without introducing separate databases or messaging too early.

## 13. Audit-Oriented Architecture

**Where used**

- `AuditLogEntry`
- Import batch history
- Risk assessment history
- Intervention history
- Report request tracking

**Reason**

The product must support compliance-ready reporting. Auditability is necessary to prove what data was ingested, what rules triggered, what interventions happened, and what reports were generated.

## 14. Validation and Reconciliation Architecture

**Where used**

- Import error tracking
- Reconciliation issue tracking
- Validation inside `EfDataIngestionService`

**Reason**

External learning data can be incomplete, duplicated, or inconsistent. The system needs a structured way to reject, flag, review, and resolve bad records.

## 15. Role-Based Access Architecture

**Where used**

- `SystemUser`
- `Role`
- `UserRoleAssignment`
- Planned RBAC/scoped authorization in API

**Reason**

Learner data is sensitive. Different users need different access: learners, trainers, managers, L&D administrators, compliance officers, and system administrators.

## 16. Dashboard and Reporting Architecture

**Where used**

- `DashboardMetricSnapshot`
- `ReportTemplate`
- `ReportRequest`
- `ReportSubscription`
- Frontend dashboard sections

**Reason**

The system must support operational dashboards and compliance-ready reports. Separating dashboard/reporting concepts prepares the app for scheduled reports, exports, and evidence generation.

## 17. Health Check Architecture

**Where used**

- `/health` endpoint
- ASP.NET Core health checks

**Reason**

Supports local and production readiness checks. Health endpoints are useful for deployment platforms, monitoring tools, and troubleshooting.

## 18. Frontend Single Page Application Architecture

**Where used**

- React frontend under `frontend`
- Module navigation
- Learner Profile and Data Ingestion sections

**Reason**

The product is dashboard-heavy and workflow-oriented. A SPA gives a smoother user experience for profile review, ingestion monitoring, and future admin workflows.

## 19. Component-Based Frontend Architecture

**Where used**

- `MetricCard`
- `Section`
- `ModulePlaceholder`

**Reason**

Reusable UI components keep the frontend easier to extend as more modules are implemented.

## 20. Incremental Delivery Architecture

**Where used**

- Implemented modules: Learner Profile and Data Ingestion
- Linked placeholders for remaining modules

**Reason**

Supports staged delivery. The team can demonstrate an end-to-end product slice while keeping the full product roadmap visible.

## 21. Local-First Development Architecture

**Where used**

- SQL Server LocalDB
- Local React dev server
- Local ASP.NET Core API

**Reason**

Developers can build, run, test, seed data, and verify workflows on a local machine before moving to cloud deployment.

## 22. Cloud-Ready Deployment Architecture

**Where used**

- Dockerfile
- Docker Compose scaffold
- Azure Container Apps recommendation
- Externalized configuration

**Reason**

The app can move from local development to managed container deployment with fewer structural changes.

## 23. Observability-Ready Architecture

**Where used**

- Health endpoint
- Structured service boundaries
- Planned OpenTelemetry/Application Insights support

**Reason**

Production systems need logs, metrics, traces, and health checks to diagnose ingestion failures, API issues, and background processing problems.

## 24. Compliance-Ready Architecture

**Where used**

- Compliance report concepts
- Audit logs
- Risk history
- Intervention outcomes
- Source data tracking

**Reason**

The product’s purpose includes compliance tracking. The architecture preserves evidence and traceability needed for audits.

## Summary

The system currently uses a **modular monolith with clean architecture**, API-first integration, SQL-backed persistence, read-model aggregation, audit-ready data modeling, and a React SPA frontend. This gives a practical balance between delivery speed, maintainability, compliance readiness, and future scalability.
