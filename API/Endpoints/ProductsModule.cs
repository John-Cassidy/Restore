using Carter;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Endpoints;

public class ProductsModule : CarterModule
{
    public ProductsModule() : base("/api/products")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetProducts).WithName(nameof(GetProducts));
        app.MapGet("/{id:int}", GetProductById).WithName(nameof(GetProductById));

    }

    private static async Task<Results<Ok<IEnumerable<string>>, BadRequest<string>>> GetProducts()
    {
        try
        {
            var products = await Task.FromResult(new[] { "product 1", "product 2" });
            return TypedResults.Ok(products.AsEnumerable());
        }
        catch (Exception ex)
        {
            // Handle the exception here
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<Ok<string>, NotFound<string>>> GetProductById(int id)
    {
        try
        {
            var product = await Task.FromResult($"Product {id}");
            return TypedResults.Ok(product);
        }
        catch (Exception ex)
        {
            // Handle the exception here
            return TypedResults.NotFound(ex.Message);
        }
    }
}
