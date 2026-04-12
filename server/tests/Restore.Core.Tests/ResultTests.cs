using Restore.Core.Results;

namespace Restore.Core.Tests;

public class ResultTests
{
    [Fact]
    public void Success_WithValue_SetsSuccessState()
    {
        var result = Result<string>.Success("ok");

        Assert.True(result.IsSuccess);
        Assert.Equal("ok", result.Value);
        Assert.Equal(200, result.StatusCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Failure_WithErrorAndStatusCode_SetsFailureState()
    {
        var result = Result<string>.Failure("bad request", 400);

        Assert.False(result.IsSuccess);
        Assert.Equal("bad request", result.ErrorMessage);
        Assert.Equal(400, result.StatusCode);
    }
}