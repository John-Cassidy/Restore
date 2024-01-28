using Restore.Core.Entities;
using Restore.Core.Results;

namespace Restore.Core.Repositories;

public interface IUserRepository
{
    Task<Result<User>> LoginAsync(string username, string password);
}
