# Developer Notes

## Create API project with dotnet

```powershell
dotnet -h

dotnet new list

# create solution
dotnet new sln -n Restore
# create API webapi project
dotnet new webapi -o API
# add project to solution
dotnet sln add API
```

## Refactor

### Controllers to Minimal APIs

[YouTube Video](Fix Your Controllers By Refactoring To Minimal APIs)

Add Carter Nuget Package to extend minimal APIs.

### Refactor API into clean architecture structure

\server\Services\Restore

- Restore.API - webapi
- Restore.Application - classlibrary
- Restore.Core - classlibrary
- Restore.Infrastructure - classlibrary

## Nuget Packages

### Carter

[Documentation / Examples](https://github.com/CarterCommunity/Carter)

[Nuget Package](https://www.nuget.org/packages/Carter)

[HttpResponse with Carter](https://andrewlock.net/adding-content-negotiation-to-minimal-apis-with-carter/)

## dotnet tools

```powershell
dotnet tool list -g
```

### dotnet-ef - Use for Migrations

Install globally

```powershell
dotnet tool install --global dotnet-ef --version 8.0.1
```

#### dotnet ef migrations (option 1)

When setting up development for first time with SqlServer container, run manual migration from command line in solution folder.

```powershell
dotnet ef migrations add InitialCreate -o Data/Migrations -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api
```

Where InitialCreate is the name that we will give our migration, you can change this name with details of what your migration refers to, such as changing a field in a table, adding or removing fields, by convention we try to detail the update that the migration will do.

-p (project) we pass the name of the solution that contains our DbContext in the case of Infrastructure

-s (solution) we pass our main project in the case of the API

If everything went well after running the command you will get a message like this: 'Done. To undo this action, use ‘ef migrations remove’'

##### Migrations remove command

The migrations remove command is used to remove the created migration if it is not as you wanted.

```powershell
dotnet ef migrations remove -o Data/Migrations -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api
```

##### Apply pending migrations to the database

To apply any pending migrations to the database, effectively updating the database schema. If no migrations are pending, this command has no effect.

-s server/Services/Restore/Restore.Api: The -s option is used to specify the startup project, which is the project that the tools build and run to get the DbContext and its configuration. In this case, the startup project is located at server/Services/Restore/Restore.Api.

```powershell
dotnet ef database update -s server/Services/Restore/Restore.Api
```

#### Package Manager Console Commands (option 2)

Open Package Manager Console and select > Restore.Infrastructure project

Run command:

> Add-Migration InitialCreate

### converts C# models to TypeScript

`CSharpToTypeScript.CliTool` is a .NET Core global tool that converts C# models to TypeScript. Here's how you can install and use it:

1. Install the tool globally using the dotnet tool install command:

```powershell
dotnet tool install --global CSharpToTypeScript.CliTool
```

2. Once installed, you can use the `csharptotypescript` command to convert C# files to TypeScript. Here's a basic usage example:

```powershell
csharptotypescript convert -i ./path/to/csharp/files -o ./path/to/output/typescript/files
```

In this command, `-i` specifies the input directory containing the C# files, and `-o` specifies the output directory for the TypeScript files.

Please replace `./path/to/csharp/files` and `./path/to/output/typescript/files` with your actual directories.

Remember to consult the tool's documentation or use the `--help` option for more detailed usage instructions and available options:

```powershell
csharptotypescript --help
```
