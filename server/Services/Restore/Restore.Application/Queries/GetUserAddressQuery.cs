using MediatR;
using Restore.Application.Responses;

namespace Restore.Application.Queries;

public class GetUserAddressQuery : IRequest<AddressResponse?>
{
    public string Username { get; }

    public GetUserAddressQuery(string username)
    {
        Username = username;
    }
}