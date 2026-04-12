# Unit Test Projects

This repository now includes one xUnit test project for each backend service project in the solution:

- `Restore.Core.Tests` for `Restore.Core`
- `Restore.Application.Tests` for `Restore.Application`
- `Restore.Infrastructure.Tests` for `Restore.Infrastructure`
- `Restore.API.Tests` for `Restore.API`

All test projects are located under `server/tests` and are included in `Restore.sln`.

## Test Categories

Tests are organized into two categories:

- **Unit Tests** — Fast, isolated tests with no external dependencies. Run without Docker.
- **Integration Tests** — End-to-end API tests using [Testcontainers](https://testcontainers.com/) to spin up a PostgreSQL container. Require Docker to be running.

Integration tests are tagged with `[Trait("Category", "Integration")]`.

## Test File Naming Convention

Test files are named for the type under test and each file contains tests for that specific type only.

## Run Tests

### All Tests (requires Docker)

```powershell
dotnet test Restore.sln --verbosity normal
```

### Unit Tests Only (no Docker needed)

```powershell
dotnet test Restore.sln --filter "Category!=Integration" --verbosity normal
```

### Integration Tests Only (requires Docker)

```powershell
dotnet test Restore.sln --filter "Category=Integration" --verbosity normal
```

### Per-Project with TRX Logging

## Prerequisite for Generating Code Coverage Report

Install (one-time, global tool)

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

## Generate HTML Coverage Report

Run Code Coverage From Repository Root

Use the following command from the repository root to collect XPlat code coverage across all test projects:

```powershell
dotnet test .\Restore.sln --collect:"XPlat Code Coverage"
```

Coverage reports are written under each test project's `TestResults/{guid}/coverage.cobertura.xml`.

Run from the repository root after collecting coverage:

```powershell
reportgenerator `
  -reports:"**/TestResults/**/coverage.cobertura.xml" `
  -targetdir:"coveragereport" `
  -reporttypes:Html `
  -filefilters:"-**/obj/**"
```

The glob pattern `**/TestResults/**/coverage.cobertura.xml` picks up every Cobertura file produced anywhere under the repo in a single pass and merges them into one report.

Open `coveragereport/index.html` in a browser to view the merged report:

```powershell
Start-Process coveragereport/index.html
```

## Clean Up Coverage Artifacts

Remove the generated report and all `TestResults` folders from the repository root:

```powershell
Remove-Item -Recurse -Force coveragereport
Get-ChildItem -Path . -Recurse -Filter TestResults -Directory | Remove-Item -Recurse -Force
```

## Integration Test Infrastructure

Integration tests use the following packages and patterns:

- **Testcontainers.PostgreSql** — Spins up a `postgres:16-alpine` container shared across all integration test classes via `ICollectionFixture<PostgresContainerFixture>`
- **Microsoft.AspNetCore.Mvc.Testing** — `WebApplicationFactory<Program>` creates an in-memory test server with the real app pipeline
- **CustomWebApplicationFactory** — Overrides the DbContext connection string to point at the Testcontainer and stubs external services (Stripe, Cloudinary)
- **IntegrationTestBase** — Abstract base class providing `AuthenticateAsync()` helper that logs in as the seeded "bob" user

### Key files

| File                                    | Purpose                                      |
| --------------------------------------- | -------------------------------------------- |
| `Fixtures/PostgresContainerFixture.cs`  | Manages PostgreSQL container lifecycle       |
| `Fixtures/IntegrationTestCollection.cs` | Collection definition for shared container   |
| `CustomWebApplicationFactory.cs`        | WebApplicationFactory with service overrides |
| `IntegrationTestBase.cs`                | Base class with auth helper and client setup |
