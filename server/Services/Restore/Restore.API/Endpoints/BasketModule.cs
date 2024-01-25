using MediatR;
using Restore.API.DTOs;
using Restore.API.Extensions;
using Restore.Application.Commands;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.API;

public static class BasketModule
{
    // endpoints: 
    // 1. GetBasket
    // 2. AddItemToBasket
    // 3. RemoveItemFromBasket

    public static IEndpointRouteBuilder AddBasketEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/basket",
            async (HttpContext context, IMediator mediator) =>
            {
                try
                {
                    var buyerId = GetBuyerId(context);
                    var query = new GetBasketQuery(buyerId);
                    Result<BasketResponse>? result = await mediator.Send(query);
                    if (!result.IsSuccess)
                    {
                        return Results.Problem(title: $"Basket with id {buyerId} not found", statusCode: StatusCodes.Status404NotFound);
                    }
                    return Results.Ok(result.Value.MapBasketToDto());
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("GetBasket")
            .WithOpenApi()
            .Produces<BasketDto>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        endpoints.MapPost("/api/basket",
        async (HttpContext context, IMediator mediator, int productId, int quantity) =>
        {
            try
            {
                var buyerId = GetBuyerId(context);
                var command = new AddItemToBasketCommand(buyerId, productId, quantity);
                var result = await mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                }

                // return createdAtEndpoint with basket GetBasket
                return Results.CreatedAtRoute("GetBasket", new { }, result.Value.MapBasketToDto());
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("AddItemToBasket")
        .WithOpenApi()
        .Produces<BasketDto>(StatusCodes.Status201Created)
        .Produces<string>(StatusCodes.Status400BadRequest);

        endpoints.MapDelete("/api/basket", async (HttpContext context, IMediator mediator, int productId, int quantity) =>
        {
            try
            {
                var buyerId = context.Request.Cookies["buyerId"];
                if (buyerId == null)
                {
                    return Results.Problem(title: "No Basket Found", statusCode: StatusCodes.Status404NotFound);
                }

                var command = new RemoveItemFromBasketCommand(buyerId, productId, quantity);
                var result = await mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("RemoveItemFromBasket")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces<string>(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status400BadRequest);

        return endpoints;
    }

    private static string GetBuyerId(HttpContext context)
    {
        var buyerId = context.Request.Cookies["buyerId"];
        if (buyerId == null)
        {
            buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
            context.Response.Cookies.Append("buyerId", buyerId, cookieOptions);
        }
        return buyerId;
    }
}
