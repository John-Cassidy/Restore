using Carter;

namespace API.Endpoints;

public class ProductsModule : CarterModule
{
    public ProductsModule() : base("/api/products")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetProducts).WithName(nameof(GetProducts));
        app.MapGet("/{id}", (int id) => $"Product {id}").WithName("GetProductById");
    }

    private static async Task<IResult> GetProducts()
    {
        try
        {
            var products = await Task.FromResult(new[] { "product 1", "product 2" });
            return Results.Ok(products);
        }
        catch (Exception ex)
        {
            // Handle the exception here
            return Results.BadRequest(ex.Message);
        }
    }
}
