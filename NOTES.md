# Developer Notes

## dotnet

```powershell
dotnet -h

dotnet new list

# create solution
dotnet new sln -n Restore

```

## converts C# models to TypeScript

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
