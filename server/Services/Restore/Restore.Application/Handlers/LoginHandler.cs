using AutoMapper;
using MediatR;
using Restore.Application.Abstractions.Authentication;
using Restore.Application.Commands;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class LoginHandler : IRequestHandler<LoginCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _jwtProvider;

    public LoginHandler(IUserRepository userRepository, ITokenService jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<UserResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.LoginAsync(request.Username, request.Password);

        if (!result.IsSuccess)
        {
            return Result<UserResponse>.Failure(result.ErrorMessage);
        }

        UserResponse userResponse = new UserResponse
        {
            Username = result.Value.UserName!,
            Email = result.Value.Email!,
            Token = await _jwtProvider.GenerateToken(result.Value)
        };

        return Result<UserResponse>.Success(userResponse);
    }
}
