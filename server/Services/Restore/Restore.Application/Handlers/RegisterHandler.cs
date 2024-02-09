using MediatR;
using Restore.Application.Commands;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;

    public RegisterHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<Result<Unit>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.RegisterAsync(request.Username, request.Password, request.Email);

        if (!result.IsSuccess)
        {
            return Result<Unit>.Failure(result.ErrorMessage);
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
