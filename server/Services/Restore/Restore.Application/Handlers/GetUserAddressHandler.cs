using AutoMapper;
using MediatR;
using Restore.Application.Abstractions.Authentication;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;

namespace Restore.Application.Handlers;

public class GetUserAddressHandler : IRequestHandler<GetUserAddressQuery, AddressResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetUserAddressHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AddressResponse?> Handle(GetUserAddressQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.ReadUserAddressAsync(request.Username);

        if (result?.Address is null)
            return null;

        var addressResponse = new AddressResponse
        {
            FullName = result.Address.FullName,
            Address1 = result.Address.Address1,
            Address2 = result.Address.Address2,
            City = result.Address.City,
            State = result.Address.State,
            Zip = result.Address.Zip,
            Country = result.Address.Country
        };

        return addressResponse;
    }
}
