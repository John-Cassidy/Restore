namespace Restore.Core.Pagination;

public class PaginationParams {
    private int _pageNumber = 1;
    private int _pageSize = 6;
    private const int maxPageSize = 50;

    public int? PageNumber { 
        get => _pageNumber;
        set => _pageNumber = value.HasValue && value > 0 ? value.Value : _pageNumber;
    }

    public int? PageSize {
        get => _pageSize;
        set => _pageSize = value.HasValue && value > maxPageSize ? maxPageSize : value ?? _pageSize;
    }
}
