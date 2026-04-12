using Restore.Application.Handlers;
using Restore.Application.Queries;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class GetUserAddressHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserOrAddressIsNull_ReturnsNull()
    {
        var userRepository = new StubUserRepository { UserResult = null };
        var handler = new GetUserAddressHandler(userRepository);

        var result = await handler.Handle(
            new GetUserAddressQuery("unknown"),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenUserHasNoAddress_ReturnsNull()
    {
        var userRepository = new StubUserRepository
        {
            UserResult = new User { UserName = "john", Address = null! }
        };
        var handler = new GetUserAddressHandler(userRepository);

        var result = await handler.Handle(
            new GetUserAddressQuery("john"),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenAddressExists_ReturnsMappedAddressResponse()
    {
        var userRepository = new StubUserRepository
        {
            UserResult = new User
            {
                UserName = "john",
                Address = new UserAddress
                {
                    FullName = "John Doe",
                    Address1 = "123 Main St",
                    Address2 = "Apt 4",
                    City = "Portland",
                    State = "OR",
                    Zip = "97201",
                    Country = "US"
                }
            }
        };
        var handler = new GetUserAddressHandler(userRepository);

        var result = await handler.Handle(
            new GetUserAddressQuery("john"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("123 Main St", result.Address1);
        Assert.Equal("Apt 4", result.Address2);
        Assert.Equal("Portland", result.City);
        Assert.Equal("OR", result.State);
        Assert.Equal("97201", result.Zip);
        Assert.Equal("US", result.Country);
    }

    private sealed class StubUserRepository : IUserRepository
    {
        public User? UserResult { get; set; }

        public Task<User?> ReadUserAddressAsync(string username) =>
            Task.FromResult(UserResult);

        public Task<Result<User>> LoginAsync(string username, string password) =>
            Task.FromResult(Result<User>.Failure("not used"));

        public Task<Result<User>> GetUserAsync(string username) =>
            Task.FromResult(Result<User>.Failure("not used"));

        public Task<Result<User>> RegisterAsync(string username, string password, string email) =>
            Task.FromResult(Result<User>.Failure("not used"));

        public Task<User?> ReadAsync(string username) => Task.FromResult<User?>(null);
        public Task UpdateAsync(User user) => Task.CompletedTask;
    }
}
