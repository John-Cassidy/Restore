using MediatR;
using Restore.Application.Responses;

namespace Restore.API.DTOs;

public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public long Price { get; set; }
    public IFormFile File { get; set; }
    public string Type { get; set; }
    public string Brand { get; set; }
    public int QuantityInStock { get; set; }
}
