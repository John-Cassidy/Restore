using MediatR;
using Restore.Application.Queries;
using Restore.Core.Entities;
using Restore.Core.Pagination;
using Restore.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Restore.Application.Responses;
using FluentValidation.Results;
using FluentValidation;
using System.Text.Json;
using Restore.API.Handlers;

namespace Restore.API.Endpoints;

public static class ProductsModule
{
    public static IEndpointRouteBuilder AddProductsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/products",
            async (IMediator mediator, [AsParameters] ProductParams productParams) =>
            {
                try
                {
                    var query = new GetProductsQuery(productParams);
                    var result = await mediator.Send(query);

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
                async (HttpContext context, IMediator mediator, IValidationExceptionHandler validationExceptionHandler, int id) => {
                    try {
                        var query = new GetProductByIdQuery(id);
                        var result = await mediator.Send(query);
                        if (result == null) {
                            return Results.Problem(title: $"Product with id {id} not found", statusCode: StatusCodes.Status404NotFound);
                            // return Results.NotFound();
                        }

                        return Results.Ok(result);
                    } catch (ValidationException ex) {
                        var problemDetails = validationExceptionHandler.Handle(ex);
                        return Results.Problem(title: problemDetails.Title, statusCode: problemDetails.Status, detail: problemDetails.Detail);
                    } catch (Exception ex) {
                        return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
                        // return Results.BadRequest(ex.Message);
                    }
                })
                .WithName("GetProduct")
                .WithOpenApi()
                .Produces<ProductResponse>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .Produces<string>(StatusCodes.Status404NotFound);


        return endpoints;
    }
}
