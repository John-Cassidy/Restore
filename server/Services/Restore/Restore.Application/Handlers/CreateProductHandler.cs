using MediatR;
using Restore.Application.Commands;
using Restore.Application.Mappers;
using Restore.Application.Responses;
using Restore.Application.Services;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductResponse>> {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageService _imageService;

    public CreateProductHandler(IUnitOfWork unitOfWork, IImageService imageService) {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<Result<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken) {
        var fileName = await _imageService.AddImageAsync(command.File);
        if (!fileName.IsSuccess) {
            return Result<ProductResponse>.Failure(fileName.ErrorMessage);
        }
        var product = new Product {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            PictureUrl = fileName.Value,
            Brand = command.Brand,
            Type = command.Type
        };

        await _unitOfWork.ProductRepository.AddAsync(product);
        var result = await _unitOfWork.CompleteAsync() > 0;
        if (!result) {
            return Result<ProductResponse>.Failure("Failed to create product");
        }
        return Result<ProductResponse>.Success(product.ToProductResponse());

    }
}
