using Microsoft.AspNetCore.Http;
using Restore.API.Extensions;
using Restore.Core.Pagination;

namespace Restore.API.Tests;

public class HttpExtensionsTests
{
    [Fact]
    public void AddPaginationHeader_AddsPaginationAndExposeHeaders()
    {
        var response = new DefaultHttpContext().Response;
        var metaData = new MetaData
        {
            CurrentPage = 1,
            TotalPages = 4,
            PageSize = 10,
            TotalCount = 40
        };

        response.AddPaginationHeader(metaData);

        Assert.True(response.Headers.ContainsKey("Pagination"));
        Assert.Contains("\"currentPage\":1", response.Headers["Pagination"].ToString());
        Assert.Equal("Pagination", response.Headers["Access-Control-Expose-Headers"].ToString());
    }
}