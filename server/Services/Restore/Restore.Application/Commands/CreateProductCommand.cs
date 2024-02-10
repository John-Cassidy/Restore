using MediatR;
using Restore.Application.Responses;
using Restore.Application.Services;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class CreateProductCommand : IRequest<Result<ProductResponse>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public long Price { get; set; }
    public string Type { get; set; }
    public string Brand { get; set; }
    public int QuantityInStock { get; set; }
    public IFormFileService File { get; set; }


    public CreateProductCommand(string name, string description, long price, string type, string brand, int quantityInStock, IFormFileService file)
    {
        Name = name;
        Description = description;
        Price = price;
        Type = type;
        Brand = brand;
        QuantityInStock = quantityInStock;
        File = file;
    }
}