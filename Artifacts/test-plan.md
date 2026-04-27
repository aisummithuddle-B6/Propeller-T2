# Detailed Test Plan

## 1. Purpose

This detailed test plan defines the approach for validating the Corporate Learning Progress, Intervention & Compliance Tracking System across implemented, partially implemented, and planned modules. It covers functional correctness, data integrity, user experience, traceability, operational readiness, and demo suitability.

## 2. Product Areas Covered

| Module | Coverage Objective | Current Status |
|---|---|---|
| Learner Profile | Validate learner identity, profile switching, summaries, and drilldowns. | Implemented |
| Data Ingestion | Validate API, UI form, Excel upload, import batches, validation errors, and reconciliation issues. | Implemented / Partially Implemented |
| Competency Tracking | Validate milestone display, milestone update behavior, and overdue visibility. | Implemented |
| Risk Rules and Risk Assessment | Validate rule configuration, risk calculation, explainability, and risk history. | Partially Implemented |
| Intervention Management | Validate creation, notes, outcomes, status, and profile visibility. | Partially Implemented |
| Dashboard | Validate metric snapshots and planned dashboard views. | Implemented / Planned |
| Compliance Reporting | Validate report templates, report requests, and planned evidence output. | Implemented / Planned |
| Notifications and Escalations | Validate seeded notification and escalation data and future event flows. | Seeded |
| Administration and Access Control | Validate users, roles, configuration, and data source administration. | Implemented |
| Audit and Operations | Validate audit logs, health checks, setup, and deployment verification. | Implemented / Partially Implemented |

## 3. Test Strategy

| Test Type | Objective | Examples | Owner |
|---|---|---|---|
| Unit Testing | Validate isolated business and mapping logic. | Risk assessment logic, validation helpers, DTO mapping. | Developers |
| API Testing | Validate endpoint contracts and response behavior. | `GET /api/learners`, ingestion endpoints, health endpoint. | Backend QA |
| Integration Testing | Validate database-backed behavior and service orchestration. | EF Core queries, LocalDB seed data, ingestion persistence. | Backend QA |
| UI Functional Testing | Validate user interactions and module screens. | Learner dropdown, module navigation, ingestion forms. | Frontend QA |
| End-to-End Testing | Validate complete business workflows. | Excel upload updates profile; failed assessment creates risk; intervention outcome appears. | QA Lead |
| Regression Testing | Validate existing functionality after changes. | Learner profile and ingestion remain working after new module changes. | QA Team |
| Smoke Testing | Validate build and runtime readiness. | API starts, frontend loads, health endpoint returns success. | Release Owner |
| Exploratory Testing | Identify usability and edge-case gaps. | Empty data states, invalid file upload, network failures. | QA/Product |

## 4. Test Environment

| Environment Item | Configuration |
|---|---|
| Frontend | React + TypeScript + Vite |
| Backend | ASP.NET Core Web API |
| Application Projects | `Learning.Api`, `Learning.Application`, `Learning.Domain`, `Learning.Infrastructure`, `Learning.Workers` |
| Database | SQL Server LocalDB for POC; PostgreSQL compatibility for future deployment |
| Data Access | EF Core |
| Seed Data | 13 learners plus programs, sessions, attendance, assessments, milestones, risks, interventions, reports, users, roles, and audit data |
| Browser | Latest Microsoft Edge or Chrome |
| API Documentation | Swagger/OpenAPI in development |
| Local URLs | API: `http://localhost:5080`; Frontend: `http://127.0.0.1:5173` |

## 5. Entry Criteria

- Source code is available and dependencies are restored.
- SQL Server LocalDB or selected database provider is available.
- Database schema is created and seed data is loaded.
- Backend build succeeds.
- Frontend build succeeds.
- API health endpoint is reachable.
- Test cases and requirement traceability matrix are available.
- Known limitations are identified for planned or partially implemented modules.

## 6. Exit Criteria

- All must-have implemented module test cases pass.
- No open critical or high defects block the POC demo.
- All failed tests have documented defect IDs or known limitation notes.
- Requirement coverage is traceable from DFR IDs to use cases and tests.
- Smoke test passes from a clean start.
- Demo script can be executed using seeded and ingested data.

## 7. Test Data Plan

| Data Category | Test Data Needed | Purpose |
|---|---|---|
| Learners | Standard learner, compliant learner, at-risk learner, learner without manager. | Validate profile and switching. |
| Attendance | Present, absent, partially attended, invalid percentage, duplicate source record. | Validate attendance summaries and ingestion rules. |
| Assessments | Passed, failed, reassessment, invalid score, duplicate source record. | Validate assessment summaries and risk input. |
| Competencies | Completed, not started, overdue, waived/exempted examples. | Validate milestone and compliance readiness logic. |
| Risk Rules | Low attendance, failed assessment, overdue competency. | Validate risk classification and explainability. |
| Interventions | Planned, in progress, completed, outcome recorded. | Validate intervention lifecycle. |
| Reports | Mandatory compliance report template and report request. | Validate reporting workflow. |
| Audit | Seeded and generated audit events. | Validate audit history. |

## 8. Module-Level Test Coverage

| Module | Key Scenarios | Priority |
|---|---|---|
| Learner Profile | Load profile, switch learner, view summaries, view drilldowns, handle empty learner data. | High |
| Data Ingestion | Submit valid API record, submit UI form, upload Excel, reject invalid record, detect duplicate, show reconciliation issue. | High |
| Competency Tracking | Show milestones, update milestone, calculate incomplete/overdue readiness. | High |
| Risk Rules and Risk Assessment | Create rule, activate/deactivate rule, recalculate risk, view triggers and history. | High |
| Intervention Management | Create intervention, add note, record outcome, validate status visibility. | High |
| Dashboard | Show metric snapshots, show planned risk/compliance dashboards, validate empty state. | Medium |
| Compliance Reporting | Show templates, create request, validate filters, trace audit event. | Medium |
| Notifications and Escalations | Validate seeded notification list, escalation rule setup, future trigger behavior. | Medium |
| Administration and Access Control | Show users, roles, scoped assignments, configurations, data sources. | Medium |
| Audit and Operations | Show audit logs, verify health endpoint, validate setup/build/run instructions. | High |

## 9. Defect Management

| Severity | Definition | Example |
|---|---|---|
| Critical | Blocks demo or corrupts business-critical learning/compliance data. | App cannot start; ingestion corrupts learner data. |
| High | Blocks must-have workflow. | Learner profile cannot load; Excel upload fails for valid file. |
| Medium | Impacts usability or secondary workflow without blocking demo. | Error message unclear; dashboard count delayed. |
| Low | Cosmetic or documentation issue. | Label typo; table column alignment issue. |

Defect lifecycle:

1. New
2. Triaged
3. In Progress
4. Ready for Retest
5. Closed
6. Deferred as Known Limitation

## 10. Test Execution Schedule

| Stage | Activity | Estimated Effort |
|---|---|---:|
| Stage 1 | Environment setup, seed data, and smoke tests. | 4 hours |
| Stage 2 | Learner profile, competency, and data ingestion functional testing. | 10 hours |
| Stage 3 | Risk, intervention, dashboard, reporting, audit, and admin testing. | 10 hours |
| Stage 4 | Regression, defect retest, and traceability review. | 6 hours |
| Stage 5 | Demo readiness validation and sign-off. | 4 hours |

## 11. Automation Strategy

| Automation Area | Recommended Coverage |
|---|---|
| Unit Tests | Risk logic, status calculations, validation helpers. |
| API Tests | Learner APIs, ingestion APIs, risk/intervention APIs, reporting APIs, health endpoint. |
| Integration Tests | EF Core database-backed services and seeded data scenarios. |
| UI Smoke Tests | Load app, switch learner, open each module, submit ingestion form. |
| CI Checks | Backend build, frontend build, unit tests, lint checks, smoke test script. |

## 12. Risks and Mitigation

| Risk | Impact | Mitigation |
|---|---|---|
| Excel files have inconsistent headers. | File ingestion fails or maps incorrectly. | Provide template and strict header validation. |
| UI and API contract drift. | UI fails after backend changes. | Maintain DTO/API contract tests. |
| Seed data becomes stale. | Demo flows fail or look incomplete. | Keep deterministic seed data and verify counts. |
| LocalDB and PostgreSQL behavior differ. | Deployment defects appear late. | Run provider compatibility tests. |
| Partially implemented modules are mistaken as complete. | Scope confusion in demo. | Mark status clearly in documentation and UI. |

## 13. Approvals

| Role | Approval Responsibility |
|---|---|
| Product Owner | Functional scope and acceptance coverage. |
| Solution Architect | Architecture, integration, and traceability coverage. |
| QA Lead | Test plan, test cases, execution, and defect closure. |
| Dev Lead | Build, code quality, and technical readiness. |
| Compliance Representative | Reporting, audit, and compliance evidence coverage. |
