using MediatR;
using Restore.Application.Queries;
using Restore.Core.Pagination;
using Restore.Application.Responses;
using FluentValidation;
using Restore.API.Handlers;
using Restore.API.Extensions;
using Restore.Application.Commands;
using Restore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Restore.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Restore.API.Endpoints;

public static class ProductsModule
{
    public static IEndpointRouteBuilder AddProductsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/products",
            async (HttpContext context, IMediator mediator, [AsParameters] ProductParams productParams) =>
            {
                try
                {
                    var query = new GetProductsQuery(productParams);
                    var result = await mediator.Send(query);
                    context.Response.AddPaginationHeader(result.MetaData);
                    result.MetaData = null; // passing metadata in header
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("GetProducts")
            .WithOpenApi()
            .Produces<PagedList<ProductResponse>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        endpoints.MapGet("/api/products/{id}",
                async (HttpContext context, IMediator mediator, IValidationExceptionHandler validationExceptionHandler, int id) =>
                {
                    try
                    {
                        var query = new GetProductByIdQuery(id);
                        var result = await mediator.Send(query);
                        if (result == null)
                        {
                            return Results.Problem(title: $"Product with id {id} not found", statusCode: StatusCodes.Status404NotFound);
                            // return Results.NotFound();
                        }

                        return Results.Ok(result);
                    }
                    catch (ValidationException ex)
                    {
                        var problemDetails = validationExceptionHandler.Handle(ex);
                        return Results.Problem(title: problemDetails.Title, statusCode: problemDetails.Status, detail: problemDetails.Detail);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
                        // return Results.BadRequest(ex.Message);
                    }
                })
                .WithName("GetProduct")
                .WithOpenApi()
                .Produces<ProductResponse>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .Produces<string>(StatusCodes.Status404NotFound);

        // create get endpointthat returns 
        // var brands = await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
        // var types = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();
        // return Ok(new { brands, types });
        endpoints.MapGet("/api/products/filters",
            async (IMediator mediator) =>
            {
                try
                {
                    var query = new GetProductsFiltersQuery();
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("GetProductFilters")
            .WithOpenApi()
            .Produces<ProductsFiltersResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        // create post endpoint that accepts a product and returns the created product
        endpoints.MapPost("/api/products",
            [Authorize(Roles = "Admin")] async (HttpContext context,
            IMediator mediator,
            IValidationExceptionHandler validationExceptionHandler,
            Func<IFormFile, IFormFileService> formFileServiceFactory) =>
            {
                try
                {
                    if (context == null)
                    {
                        return Results.Problem(title: "Context is null", statusCode: StatusCodes.Status500InternalServerError);
                    }

                    if ((context!.User?.Identity?.IsAuthenticated ?? false) == false)
                    {
                        return Results.Problem(title: "User is not authenticated", statusCode: StatusCodes.Status401Unauthorized);
                    }

                    var form = await context.Request.ReadFormAsync();
                    if (form is null)
                    {
                        return Results.Problem(title: "Form is null", statusCode: StatusCodes.Status400BadRequest);
                    }
                    var file = form.Files.GetFile("File");

                    if (file is null || file.Length == 0 || string.IsNullOrWhiteSpace(file.FileName))
                    {
                        return Results.Problem(title: "File is required", statusCode: StatusCodes.Status400BadRequest);
                    }

                    var createProductDto = new CreateProductDto
                    {
                        Name = form["Name"],
                        Description = form["Description"],
                        Price = long.Parse(form["Price"]),
                        Type = form["Type"],
                        Brand = form["Brand"],
                        QuantityInStock = int.Parse(form["QuantityInStock"]),
                        File = file
                    };

                    IFormFileService? formFileService = formFileServiceFactory(createProductDto.File);

                    var command = new CreateProductCommand(
                        createProductDto.Name,
                        createProductDto.Description,
                        createProductDto.Price,
                        createProductDto.Type,
                        createProductDto.Brand,
                        createProductDto.QuantityInStock,
                        file: formFileService
                    );

                    var result = await mediator.Send(command);
                    if (!result.IsSuccess || result.Value == null)
                    {
                        return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                    }
                    return Results.CreatedAtRoute("GetProduct", new { result.Value.Id }, result.Value);
                }
                catch (ValidationException ex)
                {
                    var problemDetails = validationExceptionHandler.Handle(ex);
                    return Results.Problem(title: problemDetails.Title, statusCode: problemDetails.Status, detail: problemDetails.Detail);
                }
                catch (Exception ex)
                {
                    return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
                }
            })
            .WithName("CreateProduct")
            .WithOpenApi()
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest);

        // create put endpoint that accepts a product and returns the updated product
        endpoints.MapPut("/api/products",
            [Authorize(Roles = "Admin")] async (HttpContext context,
            IMediator mediator,
            IValidationExceptionHandler validationExceptionHandler,
            Func<IFormFile, IFormFileService> formFileServiceFactory) =>
            {
                try
                {
                    if (context == null)
                    {
                        return Results.Problem(title: "Context is null", statusCode: StatusCodes.Status500InternalServerError);
                    }

                    if ((context!.User?.Identity?.IsAuthenticated ?? false) == false)
                    {
                        return Results.Problem(title: "User is not authenticated", statusCode: StatusCodes.Status401Unauthorized);
                    }

                    var form = await context.Request.ReadFormAsync();
                    if (form is null)
                    {
                        return Results.Problem(title: "Form is null", statusCode: StatusCodes.Status400BadRequest);
                    }

                    var file = form.Files.GetFile("File");
                    IFormFileService? formFileService = null;
                    if (file is not null && file.Length > 0 && !string.IsNullOrWhiteSpace(file.FileName))
                    {
                        formFileService = formFileServiceFactory(file);
                    }

                    var updateProductDto = new UpdateProductDto
                    {
                        Id = int.Parse(form["Id"]),
                        Name = form["Name"],
                        Description = form["Description"],
                        Price = long.Parse(form["Price"]),
                        Type = form["Type"],
                        Brand = form["Brand"],
                        QuantityInStock = int.Parse(form["QuantityInStock"]),
                        File = file
                    };

                    var command = new UpdateProductCommand(
                        updateProductDto.Id,
                        updateProductDto.Name,
                        updateProductDto.Description,
                        updateProductDto.Price,
                        updateProductDto.Type,
                        updateProductDto.Brand,
                        updateProductDto.QuantityInStock,
                        file: formFileService
                    );

                    var result = await mediator.Send(command);
                    if (!result.IsSuccess || result.Value == null)
                    {
                        return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                    }
                    return Results.Ok(result.Value);
                }
                catch (ValidationException ex)
                {
                    var problemDetails = validationExceptionHandler.Handle(ex);
                    return Results.Problem(title: problemDetails.Title, statusCode: problemDetails.Status, detail: problemDetails.Detail);
                }
                catch (Exception ex)
                {
                    return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
                }
            })
            .WithName("UpdateProduct")
            .WithOpenApi()
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        // create delete endpoint that accepts an id and returns the deleted product
        endpoints.MapDelete("/api/products/{id}",
            [Authorize(Roles = "Admin")] async (IMediator mediator, int id) =>
            {
                try
                {
                    var command = new DeleteProductCommand(id);
                    var result = await mediator.Send(command);
                    if (!result.IsSuccess || result.Value == null)
                    {
                        return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                    }
                    return Results.Ok(result.Value);
                }
                catch (Exception ex)
                {
                    return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
                }
            })
            .WithName("DeleteProduct")
            .WithOpenApi()
            .Produces<NoContentResult>(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}