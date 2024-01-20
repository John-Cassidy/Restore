namespace Restore.Core.Pagination;

public class PagedList<T> where T : class
{
    public PagedList()
    {
    }

    public PagedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
        Data = items;
    }
    public MetaData MetaData { get; set; }
    public IReadOnlyList<T> Data { get; set; }
}
