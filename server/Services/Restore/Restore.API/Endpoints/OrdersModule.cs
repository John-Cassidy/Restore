using FluentValidation;
using MediatR;
using Restore.API.DTOs;
using Restore.API.Extensions;
using Restore.API.Handlers;
using Restore.Application.Commands;
using Restore.Application.Queries;
using Restore.Application.Requests;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.API.Endpoints;

public static class OrdersModule
{
    /* endpoints:
     * 1. GetOrders
     * 2. GetOrderById
     * 3. CreateOrder
     * 4. UpdateOrder
     * 5. DeleteOrder
     */
    public static IEndpointRouteBuilder AddOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/orders",
            async (HttpContext context, IMediator mediator) =>
            {
                try
                {
                    var buyerId = context.GetBuyerId();
                    var query = new GetOrdersQuery(buyerId);
                    var result = await mediator.Send(query);
                    if (!result.IsSuccess)
                    {
                        return Results.Problem(title: $"Orders for buyer with id {buyerId} not found", statusCode: StatusCodes.Status404NotFound);
                    }
                    return Results.Ok(result.Value.MapOrdersToDto());
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("GetOrders")
            .WithOpenApi()
            .Produces<IReadOnlyList<OrderDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        endpoints.MapGet("/api/orders/{orderId}",
            async (HttpContext context, IMediator mediator, int orderId) =>
            {
                try
                {
                    var buyerId = context.GetBuyerId();
                    var query = new GetOrderByIdQuery(buyerId, orderId);
                    Result<OrderResponse>? result = await mediator.Send(query);
                    if (!result.IsSuccess || result.Value == null)
                    {
                        return Results.Problem(title: $"Order with id {orderId} not found", statusCode: StatusCodes.Status404NotFound);
                    }
                    return Results.Ok(result.Value.MapOrderToDto());
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("GetOrderById")
            .WithOpenApi()
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        endpoints.MapPost("/api/orders",
            async (HttpContext context, IMediator mediator, IValidationExceptionHandler validationExceptionHandler, CreateOrderCommandRequest request) =>
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
                    var userName = context!.User?.Identity?.Name;
                    if (string.IsNullOrEmpty(userName))
                    {
                        return Results.Problem(title: "User name is not provided", statusCode: StatusCodes.Status400BadRequest);
                    }

                    var buyerId = context!.GetBuyerId();
                    var command = new CreateOrderCommand(buyerId, userName, request);
                    var result = await mediator.Send(command);
                    if (!result.IsSuccess || result.Value == null)
                    {
                        return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                    }
                    return Results.CreatedAtRoute("GetOrderById", new { orderId = result.Value.Id }, result.Value.MapOrderToDto());
                }
                catch (ValidationException ex)
                {
                    var problemDetails = validationExceptionHandler.Handle(ex);
                    return Results.Problem(title: problemDetails.Title, statusCode: problemDetails.Status, detail: problemDetails.Detail);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("CreateOrder")
            .WithOpenApi()
            .Produces<string>(statusCode: StatusCodes.Status500InternalServerError)
            .Produces<OrderDto>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}

