using System.Diagnostics.CodeAnalysis;

namespace Restore.Core.Exceptions;

[ExcludeFromCodeCoverage]
public class UnauthorizedException : Exception
{
    public UnauthorizedException() { }

    public UnauthorizedException(string? message) : base(message) { }

    public UnauthorizedException(string? message, Exception? innerException)
        : base(message, innerException) { }
}