using System.Diagnostics.CodeAnalysis;

namespace Restore.Core;

[ExcludeFromCodeCoverage]
public record ValidationError(string Problem1, string Problem2);