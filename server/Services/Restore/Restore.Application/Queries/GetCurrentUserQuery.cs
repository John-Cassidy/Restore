using MediatR;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.Application.Queries;

public class GetCurrentUserQuery : IRequest<Result<UserResponse>>
{
    public string Username { get; }

    public GetCurrentUserQuery(string username)
    {
        Username = username;
    }
}
