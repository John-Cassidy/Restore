using MediatR;
using Restore.Application.Queries;
using Restore.Core.Entities;
using Restore.Core.Pagination;
using Restore.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Restore.Application.Responses;

namespace Restore.API.Endpoints;

public static class ProductsModule
{
    public static IEndpointRouteBuilder AddProductsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/products",
        async (IMediator mediator, [AsParameters] ProductParams productParams) =>
        {
            try
            {
                var query = new GetProductsQuery(productParams);
                var result = await mediator.Send(query);

                #region GetSampleProducts
                // var products = result.Data.Select(index =>
                //         new Product
                //         {
                //             Id = index.Id,
                //             Name = index.Name,
                //             Description = index.Description,
                //             Price = index.Price,
                //             PictureUrl = index.PictureUrl,
                //             Type = index.Type,
                //             Brand = index.Brand,
                //             QuantityInStock = index.QuantityInStock
                //         })
                //     .ToList();
                #endregion

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                Console.WriteLine(ex.Message);
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("GetProducts")
        .WithOpenApi()
        .Produces<PagedList<ProductResponse>>(StatusCodes.Status200OK) // Specify the result type and the status code for a successful response
        .Produces<string>(StatusCodes.Status400BadRequest); // Specify the result type and the status code for a bad request;

        #region GetSampleProducts
        // endpoints.MapGet("/products/GetSampleProducts", () =>
        // {
        //     var products = Enumerable.Range(1, 5).Select(index =>
        //             new Product
        //             {
        //                 Id = index,
        //                 Name = $"Product {index}",
        //                 Description = $"Description {index}",
        //                 Price = 1000 * index,
        //                 PictureUrl = $"https://picsum.photos/seed/{index}/200/300",
        //                 Type = $"Type {index}",
        //                 Brand = $"Brand {index}",
        //                 QuantityInStock = 100 * index
        //             })
        //         .ToArray();
        //     return products;
        // })
        // .WithName("GetSampleProducts")
        // .WithOpenApi();
        #endregion

        return endpoints;
    }
}
