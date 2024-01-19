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

### dotnet-ef

```powershell
dotnet tool install --global dotnet-ef --version 8.0.1
```

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
