﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Restore.API.DTOs;
using Restore.API.Extensions;
using Restore.Application.Commands;

namespace Restore.API.Endpoints;

public static class PaymentsModule
{
    /* endpoints:
     * 1. CreateOrUpdatePaymentIntent POST
     * 2. webhook POST
     */
    public static IEndpointRouteBuilder AddPaymentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/payments",
            [Authorize] async (HttpContext context, IMediator mediator) =>
            {
                try
                {
                    var validStatusCodes = new List<int>
                    {
                        StatusCodes.Status200OK,
                        StatusCodes.Status400BadRequest,
                        StatusCodes.Status401Unauthorized,
                        StatusCodes.Status404NotFound,
                        StatusCodes.Status500InternalServerError,
                    };

                    var buyerId = context.GetBuyerId();
                    var command = new CreateOrUpdatePaymentIntentCommand(buyerId);
                    var result = await mediator.Send(command);
                    if (!result.IsSuccess)
                    {
                        if (!validStatusCodes.Contains(result.StatusCode))
                        {
                            result.StatusCode = StatusCodes.Status400BadRequest;
                        }
                        return Results.Problem(title: result.ErrorMessage, statusCode: result.StatusCode);
                    }
                    return Results.Ok(result.Value.MapBasketToDto());
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("CreateOrUpdatePaymentIntent")
            .WithOpenApi()
            .WithOpenApi()
            .Produces<BasketDto>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        endpoints.MapPost("/api/payments/webhook", async (HttpContext context, IMediator mediator) =>
        {
            var stripeEventJson = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var stripeSignature = context.Request.Headers["Stripe-Signature"];

            var command = new VerifyPaymentCommand(stripeEventJson, stripeSignature);
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
            }

            return Results.Ok();
        })
        .AllowAnonymous();

        return endpoints;
    }

}
