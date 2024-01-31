using Microsoft.AspNetCore.Identity;

namespace Restore.Core.Entities;

public class User : IdentityUser<int>
{
    public UserAddress Address { get; set; }
}
