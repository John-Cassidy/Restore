using MediatR;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class RegisterCommand : IRequest<Result<Unit>>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }

    public RegisterCommand(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }
}
