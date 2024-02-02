using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;
using Restore.Infrastructure.Data;

namespace Restore.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly StoreContext _context;

    public UserRepository(UserManager<User> userManager, StoreContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Result<User>> GetUserAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return Result<User>.Failure("User not found");

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return Result<User>.Failure("Unauthorized");

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> RegisterAsync(string username, string password, string email)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user != null)
            return Result<User>.Failure("username: Username already exists");

        user = new User
        {
            UserName = username,
            Email = email
        };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            IEnumerable<string>? errors = result.Errors.Select((error, index) => $"{(string)("password" + index)}: {error.Description}");
            return Result<User>.Failure(string.Join(", ", errors));
        }

        await _userManager.AddToRoleAsync(user, "Member");

        return Result<User>.Success(user);
    }

    public async Task<User?> ReadAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<User?> ReadUserAddressAsync(string username)
    {
        var user = await _context.Users.
                Include(a => a.Address)
                .FirstOrDefaultAsync(x => x.UserName == username);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var userToUpdate = await _context.Users.
                Include(a => a.Address)
                .FirstOrDefaultAsync(x => x.Id == user.Id);

        if (userToUpdate == null) throw new Exception("User not found");
        if (userToUpdate.Address == null) throw new Exception("User address not found");

        userToUpdate.Address = user.Address;
        _context.Users.Update(userToUpdate);
    }
}
