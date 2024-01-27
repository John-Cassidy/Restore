namespace Restore.Application.Responses;

public record ProductsFiltersResponse(List<string> Brands = default!, List<string> Types = default!);
