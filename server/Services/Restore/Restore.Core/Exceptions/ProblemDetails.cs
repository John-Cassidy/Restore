namespace Restore.Core;

public class ProblemDetails(int status, string title, string? detail = null)
{
    public int Status => status;
    public string Title => title;
    public string? Detail => detail;
}
