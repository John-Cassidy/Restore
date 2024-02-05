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

## Identity

- Setting up ASP.NET Identity
- Using EF with Identity
- JWT Tokens
- Using Forms in React
- Validating form inputs
- App initialization
- Transfering anonymous basket to logged in user

### Nuget Packages

Add Nuget Package to Core project:

- Microsoft.Extensions.Identity.Stores

Create User.cs file in the Restore.Core project

```csharp
using Microsoft.AspNetCore.Identity;

namespace Restore.Core
{
    public class User : IdentityUser<int>
    {
        public UserAddress Address { get; set; }
    }
}
```

Add Nuget Packages to Infrastructure project:

- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Identity.EntityFrameworkCore

Create AccountRepository.cs file in the Restore.Infrastructure project

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Restore.Core;

namespace Restore.Infrastructure
{
    public class AccountRepository
    {
        private readonly IdentityDbContext<User> _context;

        public AccountRepository(IdentityDbContext<User> context)
        {
            _context = context;
        }

        // Add methods for interacting with the User entities in the database here
    }
}
```

### Identity Migrations

```powershell

# add migration
dotnet ef migrations add IdentityAdded -o Data/Migrations -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# review and then if needed, remove this migration in order to adjust entities and their relations.
dotnet ef migrations remove -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# apply pending migration
dotnet ef database update -s server/Services/Restore/Restore.Api
```

## Checkout

install nuget package in Restore.Core project to allow for ef relationships:

- Microsoft.EntityFrameworkCore.Abstractions

### Create OrderAggregate Entities

- Order
- OrderItem
- OrderStatus
- ProductItemOrdered
- ShippingAddress

```csharp
// Update StoreContext: protected override void OnModelCreating(ModelBuilder builder)

 builder.Entity<User>()
            .HasOne(a => a.Address)
            .WithOne()
            .HasForeignKey<UserAddress>(a => a.Id)
            .OnDelete(DeleteBehavior.Cascade);

```

### Order Status

The OrderStatus enum in OrderStatus.cs represents the different stages an order can go through in an e-commerce application. Here's what each status means:

Pending: The order has been created but payment has not yet been processed. This is usually the initial status after a customer places an order.

PaymentReceived: The payment for the order has been successfully received. This status is typically set after the payment gateway confirms the transaction.

PaymentFailed: The payment for the order failed. This could be due to insufficient funds, a declined credit card, or other issues with the payment gateway.

OrderShipped: The order has been shipped to the customer. This status is usually set after the order has been packaged and handed over to a shipping carrier.

OrderDelivered: The order has been delivered to the customer. This status is typically set after the shipping carrier confirms delivery.

These statuses help track the lifecycle of an order and can be used to update customers about the status of their orders.

```csharp
namespace Restore.Core;

public enum OrderStatus
{
    Pending,
    PaymentReceived,
    PaymentFailed
}
```

#### Proposed Additional Statuses

Here are some potential additional OrderStatus values that could be useful in an e-commerce application:

Processing: This status could be used after PaymentReceived to indicate that the order is being prepared for shipment. This could involve picking the items from the warehouse, packaging them, and preparing the shipping label.

Cancelled: This status could be used to indicate that the order has been cancelled. This could happen if the customer decides to cancel the order, or if the order cannot be fulfilled for some reason.

Refunded: This status could be used to indicate that the customer has been refunded for the order. This could happen if the order is returned, or if there was an issue with the order that warranted a refund.

Returned: This status could be used to indicate that the order has been returned by the customer.

FailedDelivery: This status could be used to indicate that the delivery of the order failed. This could happen if the customer was not available to receive the order, or if there was an issue with the shipping address.

Here's how you could add these to the OrderStatus enum:

```csharp
namespace Restore.Core;

public enum OrderStatus
{
    Pending,
    PaymentReceived,
    PaymentFailed,
    Processing,
    OrderShipped,
    OrderDelivered,
    Cancelled,
    Refunded,
    Returned,
    FailedDelivery
}
```

#### Actions Triggered by Different OrderStatus

Different OrderStatus values can trigger various actions in an e-commerce application. Here are some common examples:

Pending: At this stage, the system might send an order confirmation email to the customer, providing details about the order and next steps.

PaymentReceived: Once payment is confirmed, the system might trigger the process of preparing the order for shipment. This could involve actions like updating inventory, notifying the warehouse, etc.

PaymentFailed: In case of payment failure, the system might send an email to the customer notifying them about the issue and possibly prompting them to try the payment again.

Processing: While the order is being processed, the system might send updates to the customer about the status of their order.

OrderShipped: When the order is shipped, the system could send an email to the customer with tracking information. It might also update the order information with the estimated delivery date.

OrderDelivered: Upon delivery, the system might send a confirmation email to the customer. It could also trigger a process to ask for customer feedback or reviews.

Cancelled: If an order is cancelled, the system might send an email to the customer confirming the cancellation. It might also trigger a refund process if the customer has already been charged.

Refunded: When a refund is issued, the system could send a confirmation email to the customer. It might also update the order information to reflect the refund.

Returned: If an order is returned, the system might trigger a process to handle the return, which could involve inspecting the returned item, restocking it, and issuing a refund.

FailedDelivery: In case of a failed delivery, the system might send a notification to the customer and attempt to reschedule the delivery.

#### Common Email Notifications that an e-commerce application can send based on different OrderStatus values

Based on different OrderStatus values, an e-commerce application can send the following common email notifications to customers:

Pending: An order confirmation email, which includes details about the order and next steps.

PaymentReceived: An email notifying that the payment has been received and the order is being prepared for shipment.

PaymentFailed: An email alerting the customer that the payment has failed, possibly with instructions on how to retry the payment.

Processing: An email update about the status of the order, such as that it's being prepared for shipment.

OrderShipped: An email with the shipping confirmation and tracking information, possibly with an estimated delivery date.

OrderDelivered: An email confirming that the order has been delivered, possibly with a request for feedback or a review.

Cancelled: An email confirming that the order has been cancelled, possibly with details about a refund if the customer has already been charged.

Refunded: An email confirming that a refund has been issued, possibly with details about when and how the refund was processed.

Returned: An email confirming that the order has been returned and a refund is being processed, possibly with details about the return process.

FailedDelivery: An email notifying the customer that delivery failed, possibly with instructions on how to reschedule the delivery.

#### Common ways an e-commerce application can handle returns and refunds

Handling returns and refunds is a crucial part of customer service in e-commerce. Here are some common ways an e-commerce application can handle these processes:

Return Initiation: The customer initiates a return through the application, often by filling out a form that includes the order number, the item(s) to be returned, and the reason for the return.

Return Approval: The system checks the return request against the company's return policy (e.g., return window, condition of the item). If the return is approved, the customer is notified and provided with return instructions.

Return Shipping: The customer ships the item back. Some companies provide a prepaid return shipping label, while others require the customer to pay for return shipping.

Return Inspection: Once the returned item is received, it's inspected to ensure it's in the appropriate condition. This process can be automated or manual, depending on the nature of the goods.

Refund Processing: If the return is accepted, the system triggers a refund to the original payment method. This process can involve communicating with the payment gateway to reverse the charge.

Notification: The customer is notified about the status of the return and refund via email or in-app notifications.

Order Status Update: The order status is updated to Returned or Refunded in the system.

Here's a simplified example of how this could be implemented in code:

```csharp
public class ReturnService
{
    public bool InitiateReturn(Order order, ReturnRequest request)
    {
        if (order.CanBeReturned() && request.IsValid())
        {
            order.Status = OrderStatus.Returned;
            SendReturnInstructionsEmail(order);
            return true;
        }
        return false;
    }

    public void ProcessReturn(Order order)
    {
        if (InspectReturn(order))
        {
            order.Status = OrderStatus.Refunded;
            ProcessRefund(order);
            SendRefundConfirmationEmail(order);
        }
    }

    // Other methods not shown...
}
```

### Update User and Role to use int as primary key

```csharp
// Update StoreContext:
public class StoreContext : IdentityDbContext<User, Role, int>

 builder.Entity<Role>()
            .HasData(
                new Role {Id = 1, Name = "Member", NormalizedName = "MEMBER" },
                new Role {Id = 2, Name = "Admin", NormalizedName = "ADMIN" }
            );

```

### Checkout Migrations

```powershell
# Drop database to delete store.db file
dotnet ef database drop -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# add migration
dotnet ef migrations add IdentityAndOrderEntityAdded -o Data/Migrations -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# review and then if needed, remove this migration in order to adjust entities and their relations.
dotnet ef migrations remove -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# apply pending migration
dotnet ef database update -s server/Services/Restore/Restore.Api

```

## Payments

### Overview

Use Stripe to manage payment processing
Use Webhooks to communicate between Stripe and Restore.API

Payment Card Industry Data Security Standard: (PCI Compliance)

- Set of industry standards
- Deisgned to protect payment card data
- Increased protection for customers and reduced risk of data breaches involving personal card data

PCI Compliance refers to the Payment Card Industry Data Security Standard (PCI DSS), a set of security standards designed to ensure that all companies that accept, process, store or transmit credit card information maintain a secure environment. The key principles of PCI Compliance are:

1. Build and Maintain a Secure Network and Systems

- Install and maintain a firewall configuration to protect cardholder data.
- Do not use vendor-supplied defaults for system passwords and other security parameters.

2. Protect Cardholder Data

- Protect stored cardholder data.
- Encrypt transmission of cardholder data across open, public networks.

3. Maintain a Vulnerability Management Program

- Protect all systems against malware and regularly update anti-virus software or programs.
- Develop and maintain secure systems and applications.

4. Implement Strong Access Control Measures

- Restrict access to cardholder data by business need to know.
- Identify and authenticate access to system components.
- Restrict physical access to cardholder data.

5. Regularly Monitor and Test Networks

- Track and monitor all access to network resources and cardholder data.
- Regularly test security systems and processes.

6. Maintain an Information Security Policy

- Maintain a policy that addresses information security for all personnel.

Please note that this is a simplified summary. The actual PCI DSS is a detailed and comprehensive document that should be thoroughly understood and followed by any organization that handles cardholder data.

### Stripe Setup

- Setup Stripe Development Account
- Add public / secret keys as environment variables to api project
- Add Nuget Package: Stripe.net

> https://github.com/stripe/stripe-dotnet

#### Migrations

Add Properties to Basket Entity:

- public string PaymentIntentId { get; set; }
- public string ClientSecret { get; set; }

Add Property to Order Entity:

- public string PaymentIntentId { get; set; }

Add Migration:

```powershell
# Drop database to delete store.db file
dotnet ef database drop -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# add migration
dotnet ef migrations add PaymentIntentAdded -o Data/Migrations -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# review and then if needed, remove this migration in order to adjust entities and their relations.
dotnet ef migrations remove -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# apply pending migration
dotnet ef database update -s server/Services/Restore/Restore.Api
```

### Payment Service

Create PaymentService - used by api to notify stripe to create a payment intent for a basket

Create webook - used by stripe to notify api that client payment succeeded for a particular basket

[Stripe Webhook Documentation](https://stripe.com/docs/webhooks)

#### Webhooks

[Documentation](https://stripe.com/docs/testing#webhooks)
[Developer Documentation](https://dashboard.stripe.com/test/webhooks)

How to set up your webhook integration
To start receiving webhook events in your app, create and register a webhook endpoint by following the steps below. You can register and create one endpoint to handle several different event types at once, or set up individual endpoints for specific events.

- Identify which events you want to monitor.
- Develop a webhook endpoint function to receive event data POST requests.
- Test your webhook endpoint function locally using the Stripe CLI.
- Register your endpoint within Stripe using the Webhooks Dashboard or the API.
- Secure your webhook endpoint.

[Download, Install, and Use Stripe CLI to test webhook configuration](https://stripe.com/docs/stripe-cli)

```powershell
# Version
.\.developernotes\stripe_1.19.2_windows_x86_64\stripe.exe --version
stripe version 1.19.2

# Login using browser
.\.developernotes\stripe_1.19.2_windows_x86_64\stripe.exe login

# Get Webhook signing secret:
.\.developernotes\stripe_1.19.2_windows_x86_64\stripe.exe listen

# All commands
.\.developernotes\stripe_1.19.2_windows_x86_64\stripe.exe
The official command-line tool to interact with Stripe.

Before using the CLI, you'll need to login:

  $ stripe login

If you're working on multiple projects, you can run the login command with the
--project-name flag:

  $ stripe login --project-name rocket-rides

Usage:
  stripe [command]

Webhook commands:
  listen                             Listen for webhook events
  trigger                            Trigger test webhook events

Stripe commands:
  logs                               Interact with Stripe API request logs

Resource commands:
  get                           Quickly retrieve resources from Stripe
  charges                       Make requests (capture, create, list, etc) on charges
  customers                     Make requests (create, delete, list, etc) on customers
  payment_intents               Make requests (cancel, capture, confirm, etc) on payment intents
  ...                           To see more resource commands, run `stripe resources help`

Other commands:
  community                          Chat with Stripe engineers and other developers
  completion                         Generate bash and zsh completion scripts
  config                             Manually change the config values for the CLI
  feedback                           Provide us with feedback on the CLI
  fixtures                           Run fixtures to populate your account with data
  help                               Help about any command
  login                              Login to your Stripe account
  logout                             Logout of your Stripe account
  open                               Quickly open Stripe pages
  samples                            Sample integrations built by Stripe
  serve                              Serve static files locally
  version                            Get the version of the Stripe CLI

Flags:
      --api-key string        Your API key to use for the command
      --color string          turn on/off color output (on, off, auto)
      --config string         config file (default is
                              $HOME/.config/stripe/config.toml)
      --device-name string    device name
  -h, --help                  help for stripe
      --log-level string      log level (debug, info, trace, warn, error)
                              (default "info")
  -p, --project-name string   the project name to read from for config
                              (default "default")
  -v, --version               Get the version of the Stripe CLI

Use "stripe [command] --help" for more information about a command.

```

## User Secrets

[Documenation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows)

run commands inside API project folder:

```powershell
dotnet user-secrets init

dotnet user-secrets set "StripeSettings:PublishableKey" "xxxxx"

dotnet user-secrets set "StripeSettings:SecretKey" "xxxxx"

dotnet user-secrets set "StripeSettings:WhSecret" "xxxxx"

dotnet user-secrets list
```

## Publishing

In this section:

- Create a Production BUild of the React App
- Host the React app on the API (Kestrel) Server
- Switch Database server to PostGreSQL
- \*Setup and configure Heroku (no longer free to use)

  - Publish to alternative cloud provider

### Setup Client to run in api folder's sub-folder: wwwroot

Once the client code is built and deployed to /wwwroot/ folder in Restore.API project folder

```csharp
// Add to Restore.API program.cs or startup file
app.UseDefaultFiles();
app.UseStaticFiles();

// Before        app.UseCors

// call after all the app.Use and app.Map calls
// i.e.        app.AddPaymentEndpoints();

app.MapFallback(async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(File.ReadAllText(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "index.html")));
});
```

### Configure Docker and Docker Compose

- add .dockerignore file to solutin folder
- Create DockerFile in Restore.API project
- Create docker-compose.yml and docker-compose.override.yml

```powershell
NOTE: REBUILD IMAGES TO INCLUDE CODE CHANGES AND START
docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build
NOTE: START CONTAINERS FROM EXISTING IMAGES WITHOUT REBUILDING
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
NOTE: STOP RUNNING CONTAINERS AND REMOVE CONTAINERS
docker-compose -f docker-compose.yml -f docker-compose.override.yml down
```

### Configure production DB Server using PostreSQL

Setup PostgreSQL database run inside Docker container

```json (appsettings)
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;User Id=appuser;Password=secret;Database=store;"
```

[Available Datbase Provider(s) Documentation](https://learn.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli)

- Install nuget package: Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
- Delete Migrations folder (migrations specific to SqlLite)
- Delete store.db and any associated file(s)
- DateTime.Now - change to DateTime.UtcNow in all entity classes that have it

  - i.e.: public DateTime OrderDate { get; set; } = DateTime.UtcNow;

```csharp
services.AddDbContext<StoreContext>(options =>
    options.UseNpgsql(connectionString), ServiceLifetime.Scoped);
```

Create New Migration

```powershell
# Drop database to delete store.db file
dotnet ef database drop -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# add migration
dotnet ef migrations add PostgresInitial -o Data/Migrations -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# review and then if needed, remove this migration in order to adjust entities and their relations.
dotnet ef migrations remove -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.Api

# apply pending migration
dotnet ef database update -s server/Services/Restore/Restore.Api
```

Run Restore.API in debug mode from VS Code using PostgreSQL DB running in Docker container:

```powershell
docker run --name restoredb-dev -e POSTGRES_USER=appuser -e POSTGRES_PASSWORD=secret -p 5432:5432 -d postgres:latest
```
