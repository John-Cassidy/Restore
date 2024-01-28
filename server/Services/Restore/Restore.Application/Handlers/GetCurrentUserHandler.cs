using MediatR;
using Restore.Application.Abstractions.Authentication;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public GetCurrentUserHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<UserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetUserAsync(request.Username);
        if (!result.IsSuccess)
        {
            return Result<UserResponse>.Failure(result.ErrorMessage);
        }

        UserResponse userResponse = new UserResponse
        {
            Username = result.Value.UserName!,
            Email = result.Value.Email!,
            Token = await _tokenService.GenerateToken(result.Value)
        };

        return Result<UserResponse>.Success(userResponse);
    }
}
