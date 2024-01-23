namespace Restore.Core;

public class ProblemDetails(int status, string? detail, string title)
{
    public int Status => status;
    public string? Detail => detail;
    public string Title => title;
}
