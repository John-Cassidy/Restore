# Unit Test Projects

This repository now includes one xUnit test project for each backend service project in the solution:

- `Restore.Core.Tests` for `Restore.Core`
- `Restore.Application.Tests` for `Restore.Application`
- `Restore.Infrastructure.Tests` for `Restore.Infrastructure`
- `Restore.API.Tests` for `Restore.API`

All test projects are located under `server/tests` and are included in `Restore.sln`.

## Test File Naming Convention

Test files are named for the type under test and each file contains tests for that specific type only.

## Run Tests

Run these commands from the repository root (`Restore` folder) to produce test result files per test project.

```powershell
dotnet test server/tests/Restore.Core.Tests/Restore.Core.Tests.csproj --logger "trx;LogFileName=Restore.Core.Tests.trx" --results-directory ./TestResults/Restore.Core.Tests

dotnet test server/tests/Restore.Application.Tests/Restore.Application.Tests.csproj --logger "trx;LogFileName=Restore.Application.Tests.trx" --results-directory ./TestResults/Restore.Application.Tests

dotnet test server/tests/Restore.Infrastructure.Tests/Restore.Infrastructure.Tests.csproj --logger "trx;LogFileName=Restore.Infrastructure.Tests.trx" --results-directory ./TestResults/Restore.Infrastructure.Tests

dotnet test server/tests/Restore.API.Tests/Restore.API.Tests.csproj --logger "trx;LogFileName=Restore.API.Tests.trx" --results-directory ./TestResults/Restore.API.Tests
```

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
  -reporttypes:Html
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
