using Restore.Core.Entities;
using Restore.Core.Results;

namespace Restore.Core.Repositories;

public interface IUserRepository
{
    Task<Result<User>> GetUserAsync(string username);
    Task<Result<User>> LoginAsync(string username, string password);
    Task<Result<User>> RegisterAsync(string username, string password, string email);
    Task<User?> ReadAsync(string username);
    Task<User?> ReadUserAddressAsync(string username);
    Task UpdateAsync(User user);
}
