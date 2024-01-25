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

[Parameter Binding](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0#parameter-binding)

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

### Automapper

[Automapper](https://automapper.org/)
[Documentation to create manual IMapper for Minimal APIs](https://github.com/AutoMapper/AutoMapper)

### Fluent Validation

The FluentValidation.AspNetCore package is no longer being maintained and is now unsupported. We encourage users move away from this package and use the core FluentValidation package with a manual validation approach as detailed at
[https://docs.fluentvalidation.net/en/latest/aspnet.html](https://docs.fluentvalidation.net/en/latest/aspnet.html)

Install 2 Nuget Packages:

- FluentValidation
- FluentValidation.AspNetCore

Setup manual validation approach.

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

##### Migrations database drop command

Drop / Delete database associated with the DbContext in the Restore.Api project. This is typically done when you want to completely reset the database, for example during development when you want to recreate the database from scratch.

```powershell
dotnet ef database drop -p server/Services/Restore/Restore.Api
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

### Convert C# models to TypeScript

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

### Bogus for .NET - load databases, UI and apps with fake data for your testing needs

[Github](https://github.com/bchavez/Bogus)

```powershell
dotnet add package Bogus
```

## Implement Error Handling

[Youtube Video](https://www.youtube.com/watch?v=uOEDM0c9BNI)

### Option 1: IExceptionHandler Interface for handling unhandled exceptions globally

[Youtube Video](https://www.youtube.com/watch?v=f4zMGR3m70Y)

```csharp
using Microsoft.AspNetCore.Diagnostics;
using Restore.Core.Exceptions;

namespace Restore.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (int statusCode, string errorMessage) = exception switch
        {
            InvalidCastException invalidCastException => (StatusCodes.Status400BadRequest, invalidCastException.Message),
            AggregateException aggregateException => (StatusCodes.Status400BadRequest, aggregateException.Message),
            ArgumentNullException argumentNullException => (StatusCodes.Status400BadRequest, argumentNullException.Message),
            ArgumentException argumentException => (StatusCodes.Status400BadRequest, argumentException.Message),
            // ValidationException validationException => (StatusCodes.Status400BadRequest, validationException.Message),
            KeyNotFoundException keyNotFoundException => (StatusCodes.Status400BadRequest, keyNotFoundException.Message),
            FormatException formatException => (StatusCodes.Status400BadRequest, formatException.Message),
            // ForbidException forbidException => (StatusCodes.Status403Forbidden, "Forbidden"),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Bad request"),
            NotFoundException notFoundException => (StatusCodes.Status404NotFound, notFoundException.Message),
            _ => (500, "An error occured @" + exception.Message)
        };
        _logger.LogError(exception, "Exception occured: {Message}", exception.Message);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(errorMessage);

        return true;
        // return new ValueTask<bool>(true);
    }
}
```

### Option 2: Result Pattern for handling both handled/unhandled expceptions

[Article](https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern)

```csharp
// Utility, Shared, Domain or Core project:
namespace Restore.Core.ResultPattern;

public class Result<T>
{
    public T Value { get; set; }
    public bool IsSuccess { get; set; }
    public string Error { get; set; }

    public static Result<T> Success(T value) => new Result<T> { Value = value, IsSuccess = true };
    public static Result<T> CreateFailure(string error) => new Result<T> { Error = error, IsSuccess = false };
}

namespace Restore.Core.ResultPattern;

public class AppException
{
    public AppException(int statusCode, string message, string? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }

}

// Handler classes
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<Product>>
{
    // ...

    public async Task<Result<Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            return Result<Product>.Failure("Product not found");
        }

        return Result<Product>.Success(product);
    }
}





// API project Program.cs:
app.UseMiddleware<ExceptionHandlingMiddleware>();
// before app.UseEndPoints...
```

## Shopping Cart

Implement ability for users to add/update/remove items from and view basket

```powershell
# add migration
dotnet ef migrations add BasketEntityAdded -o Data/Migrations -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# review and then remove this migration in order to adjust entities and their relations.
dotnet ef migrations remove -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# apply pending migration
dotnet ef database update -s server/Services/Restore/Restore.Api

Build started...
Build succeeded.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (8ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT COUNT(*) FROM "sqlite_master" WHERE "name" = '__EFMigrationsHistory' AND "type" = 'table';
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT COUNT(*) FROM "sqlite_master" WHERE "name" = '__EFMigrationsHistory' AND "type" = 'table';
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "MigrationId", "ProductVersion"
      FROM "__EFMigrationsHistory"
      ORDER BY "MigrationId";
info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20240124161403_BasketEntityAdded'.
Applying migration '20240124161403_BasketEntityAdded'.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      CREATE TABLE "Baskets" (
          "Id" INTEGER NOT NULL CONSTRAINT "PK_Baskets" PRIMARY KEY AUTOINCREMENT,
          "BuyerId" TEXT NULL
      );
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      CREATE TABLE "BasketItems" (
          "Id" INTEGER NOT NULL CONSTRAINT "PK_BasketItems" PRIMARY KEY AUTOINCREMENT,
          "Quantity" INTEGER NOT NULL,
          "ProductId" INTEGER NOT NULL,
          "BasketId" INTEGER NOT NULL,
          CONSTRAINT "FK_BasketItems_Baskets_BasketId" FOREIGN KEY ("BasketId") REFERENCES "Baskets" ("Id") ON DELETE CASCADE,
          CONSTRAINT "FK_BasketItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
      );
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      CREATE INDEX "IX_BasketItems_BasketId" ON "BasketItems" ("BasketId");
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      CREATE INDEX "IX_BasketItems_ProductId" ON "BasketItems" ("ProductId");
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
      VALUES ('20240124161403_BasketEntityAdded', '8.0.1');
Done.
```
