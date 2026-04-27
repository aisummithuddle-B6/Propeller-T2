# Corporate Learning Tracking - .NET Enterprise Scaffold

This is a base code structure for the selected **Option 3: .NET Enterprise** stack.

## Technology Stack

- Frontend: React + TypeScript
- Backend API: ASP.NET Core Web API
- Architecture: Modular monolith with Clean Architecture boundaries
- Database: PostgreSQL by default, configurable to SQL Server
- Cache/Queue support: Redis
- Background jobs: .NET Worker Service
- Identity target: Microsoft Entra ID / OIDC
- Secrets target: Azure Key Vault
- Observability target: OpenTelemetry + Application Insights
- Deployment target: Docker + Azure Container Apps or Azure App Service

## Structure

```text
CODE/
  frontend/
  src/
    Learning.Api/
    Learning.Application/
    Learning.Domain/
    Learning.Infrastructure/
    Learning.Workers/
  tests/
    Learning.Tests/
  docker-compose.yml
  Directory.Build.props
```

## Architecture Intent

- `Learning.Domain` contains core business entities and enums.
- `Learning.Application` contains use cases, abstractions, DTOs, and business orchestration.
- `Learning.Infrastructure` contains database, external integrations, persistence, and technical adapters.
- `Learning.Api` exposes REST APIs, authentication/authorization boundaries, and OpenAPI.
- `Learning.Workers` handles imports, risk recalculation, reports, and notifications.
- `frontend` is a React placeholder for dashboards, learner profiles, rules, interventions, and reports.

## First Implementation Priorities

1. Learner profile read model.
2. Attendance, assessment, and competency ingestion APIs.
3. Risk rule configuration and risk evaluation.
4. Intervention tracking.
5. Compliance report generation.
6. Audit history and role-based access.

## Create Local Database Schema

For Windows LocalDB with SQL Server provider:

```powershell
sqllocaldb start MSSQLLocalDB
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "IF DB_ID(N'learning') IS NULL CREATE DATABASE [learning];"
dotnet run --project src/Learning.DatabaseMigrator/Learning.DatabaseMigrator.csproj
```

The migrator uses EF Core `EnsureCreated` for the current scaffold schema. Replace this with EF migrations before production releases.

## Database Provider Configuration

The API can switch between PostgreSQL and SQL Server using configuration.

```json
{
  "Database": {
    "Provider": "PostgreSql"
  }
}
```

Supported values:

- `PostgreSql`
- `SqlServer`

For local Docker, PostgreSQL remains the default:

```yaml
Database__Provider: PostgreSql
```

To use SQL Server instead:

```yaml
Database__Provider: SqlServer
ConnectionStrings__SqlServer: Server=sqlserver,1433;Database=learning;User Id=sa;Password=Your_strong_password123;TrustServerCertificate=True
```

For Windows LocalDB development:

```json
{
  "Database": {
    "Provider": "SqlServer"
  },
  "ConnectionStrings": {
    "SqlServer": "Server=(localdb)\\MSSQLLocalDB;Database=learning;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```
