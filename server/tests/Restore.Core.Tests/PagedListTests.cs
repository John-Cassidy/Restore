using Restore.Core.Pagination;

namespace Restore.Core.Tests;

public class PagedListTests
{
    [Fact]
    public void Constructor_WithValidArguments_SetsMetadataAndData()
    {
        var items = new List<string> { "a", "b" };

        var pagedList = new PagedList<string>(items, count: 10, pageNumber: 2, pageSize: 5);

        Assert.Equal(2, pagedList.MetaData.CurrentPage);
        Assert.Equal(5, pagedList.MetaData.PageSize);
        Assert.Equal(10, pagedList.MetaData.TotalCount);
        Assert.Equal(2, pagedList.MetaData.TotalPages);
        Assert.Equal(items, pagedList.Data);
    }
}