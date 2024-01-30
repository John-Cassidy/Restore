using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Restore.API.DTOs;
using Restore.API.Extensions;
using Restore.API.Handlers;
using Restore.Application.Commands;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Entities;
using Restore.Core.Results;

namespace Restore.API.Endpoints;

public static class AccountModule
{
    public static IEndpointRouteBuilder AddAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/account/login",
            async (HttpContext context, IMediator mediator, LoginDto loginDto) =>
            {
                try
                {
                    LoginCommand loginCommand = new LoginCommand(loginDto.Username, loginDto.Password);
                    var userResult = await mediator.Send(loginCommand);
                    if (!userResult.IsSuccess)
                    {
                        return Results.Problem(title: userResult.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                    }

                    // get basket                    

                    var basketQuery = new GetBasketQuery(userResult.Value.Username);
                    var userBasket = await mediator.Send(basketQuery);

                    Result<BasketResponse>? anonBasket = null;
                    string buyerId = context.Request.Cookies["buyerId"];
                    if (buyerId is not null)
                    {
                        basketQuery = new GetBasketQuery(buyerId);
                        anonBasket = await mediator.Send(basketQuery);
                    }

                    if (anonBasket is not null && anonBasket.Value.Id > 0)
                    {
                        if (userBasket.Value.Id > 0)
                        {
                            var deleteCommand = new DeleteBasketCommand(userBasket.Value.BuyerId);
                            var deleteResult = await mediator.Send(deleteCommand);
                            if (!deleteResult.IsSuccess)
                            {
                                return Results.Problem(title: deleteResult.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                            }
                        }

                        var updateCommand =
                            new UpdateBasketCommand(anonBasket.Value.BuyerId, userResult.Value.Username);
                        var updateResult = await mediator.Send(updateCommand);
                        if (!updateResult.IsSuccess)
                        {
                            return Results.Problem(title: updateResult.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                        }
                        anonBasket.Value.BuyerId = userResult.Value.Username;

                        context.Response.Cookies.Delete("buyerId");
                    }

                    var userDto = new UserDto(
                        userResult.Value.Email,
                        userResult.Value.Token,
                        anonBasket?.Value?.Id > 0 ? anonBasket.Value.MapBasketToDto() : userBasket.Value.MapBasketToDto()
                    );

                    return Results.Ok(userDto);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("Login")
            .WithOpenApi()
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        endpoints.MapPost("/api/account/register",
            async (HttpContext context, IMediator mediator, IValidationExceptionHandler validationExceptionHandler, RegisterDto registerDto) =>
            {
                try
                {
                    var command = new RegisterCommand(registerDto.Username, registerDto.Password, registerDto.Email);
                    Result<Unit>? result = await mediator.Send(command);

                    if (!result.IsSuccess)
                    {
                        var validationException = validationExceptionHandler.CreateValidationExceptionFromErrorMessage(result.ErrorMessage);
                        Restore.Core.ProblemDetails? problemDetails = validationExceptionHandler.Handle(validationException);
                        return Results.Problem(title: problemDetails.Title, statusCode: problemDetails.Status, detail: problemDetails.Detail);
                    }

                    return Results.Created("/api/account/login", result.Value);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("Register")
            .WithOpenApi()
            .Produces<BasketDto>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest);


        /*
        create GetCurrentUser endpoint that returns a UserDto with Microsoft.AspNetCore.Authorization attribute to ensure that the user is authenticated. 
        Use User.Identity to get the username and pass it to the GetBasketQuery to get the basket for the user. 
        If the user is not authenticated, return a 401 Unauthorized response.
        */
        endpoints.MapGet("/api/account/current",
         [Authorize] async (HttpContext context, IMediator mediator) =>
            {
                try
                {
                    var username = context.User.Identity?.Name;
                    if (string.IsNullOrEmpty(username))
                    {
                        return Results.Unauthorized();
                    }

                    var query = new GetCurrentUserQuery(username);
                    var user = await mediator.Send(query);
                    if (!user.IsSuccess)
                    {
                        return Results.Problem(title: user.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                    }

                    var basketQuery = new GetBasketQuery(username);
                    var basketResult = await mediator.Send(basketQuery);
                    if (!basketResult.IsSuccess)
                    {
                        return Results.Problem(title: basketResult.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                    }

                    var userDto = new UserDto(
                        user.Value.Email,
                        user.Value.Token,
                        basketResult.Value.MapBasketToDto()
                    );

                    return Results.Ok(userDto);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("GetCurrentUser")
            .WithOpenApi()
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}