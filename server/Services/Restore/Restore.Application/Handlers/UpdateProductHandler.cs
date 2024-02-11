using AutoMapper;
using MediatR;
using Restore.Application.Commands;
using Restore.Application.Responses;
using Restore.Application.Services;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public UpdateProductHandler(IUnitOfWork unitOfWork, IImageService imageService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
    }

    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
        if (product == null) return Result<ProductResponse>.Failure("Product not found");

        if (request.File != null)
        {
            var fileName = await _imageService.UpdateImageAsync(request.File, product.PictureUrl);
            if (!fileName.IsSuccess)
            {
                return Result<ProductResponse>.Failure(fileName.ErrorMessage);
            }
            product.PictureUrl = fileName.Value;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Brand = request.Brand;
        product.Type = request.Type;

        await _unitOfWork.ProductRepository.UpdateAsync(product);
        var result = await _unitOfWork.CompleteAsync() > 0;
        if (!result)
        {
            return Result<ProductResponse>.Failure("Failed to update product");
        }
        return Result<ProductResponse>.Success(_mapper.Map<ProductResponse>(product));

    }
}
