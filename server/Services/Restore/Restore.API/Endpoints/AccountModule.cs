using MediatR;
using Restore.API.DTOs;
using Restore.Application.Commands;
using Restore.Core.Entities;

namespace Restore.API.Endpoints;

public static class AccountModule
{
    public static IEndpointRouteBuilder AddAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/account/login",
            async (HttpContext context, IMediator mediator, [AsParameters] LoginDto loginDto) =>
            {
                try
                {
                    var command = new LoginCommand(loginDto.Username, loginDto.Password);
                    var result = await mediator.Send(command);
                    if (!result.IsSuccess)
                    {
                        return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
                    }

                    var userDto = new UserDto(result.Value.Email, result.Value.Token, null);

                    return Results.Ok(userDto);

                    // return Results.Ok(result.Value.MapBasketToDto());
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

        // endpoints.MapPost("/api/account/register",
        //     async (HttpContext context, IMediator mediator, RegisterDto registerDto) =>
        //     {
        //         try
        //         {
        //             var command = new RegisterCommand(registerDto);
        //             var result = await mediator.Send(command);
        //             if (!result.IsSuccess)
        //             {
        //                 return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
        //             }

        //             return Results.CreatedAtRoute("GetBasket", new { }, result.Value.MapBasketToDto());
        //         }
        //         catch (Exception ex)
        //         {
        //             return Results.BadRequest(ex.Message);
        //         }
        //     })
        //     .WithName("Register")
        //     .WithOpenApi()
        //     .Produces<BasketDto>(StatusCodes.Status201Created)
        //     .Produces<string>(StatusCodes.Status400BadRequest);

        // endpoints.MapPost("/api/account/logout",
        //     async (HttpContext context, IMediator mediator) =>
        //     {
        //         try
        //         {
        //             var command = new LogoutCommand();
        //             var result = await mediator.Send(command);
        //             if (!result.IsSuccess)
        //             {
        //                 return Results.Problem(title: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
        //             }

        //             return Results.Ok();
        //         }
        //         catch (Exception ex)
        //         {
        //             return Results.BadRequest(ex.Message);
        //         }
        //     })
        //     .WithName("Logout")
        //     .WithOpenApi()
        //     .Produces<string>(StatusCodes.Status200OK)
        //     .Produces<string>(StatusCodes.Status400BadRequest);

        // endpoints.MapGet("/api/account",
        //     async (HttpContext context, IMediator mediator) =>
        //     {
        //         try
        //         {
        //             var query = new GetAccountQuery();
        //             var result = await mediator.Send(query);

        return endpoints;
    }
}
