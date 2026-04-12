using System.Diagnostics.CodeAnalysis;

namespace Restore.Core.Exceptions;

[ExcludeFromCodeCoverage]
public class BadRequestException : Exception
{
    public BadRequestException()
    {
    }

    public BadRequestException(string message)
        : base(message)
    {
    }

    public BadRequestException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
