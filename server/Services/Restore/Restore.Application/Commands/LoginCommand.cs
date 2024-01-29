using MediatR;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class LoginCommand : IRequest<Result<UserResponse>>
{
    public string Username { get; set; }
    public string Password { get; set; }

    public LoginCommand(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
