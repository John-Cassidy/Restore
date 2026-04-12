using System.Diagnostics.CodeAnalysis;

namespace Restore.Core.Entities;

[ExcludeFromCodeCoverage]
public class UserAddress : Address
{
    public int Id { get; set; }
}
