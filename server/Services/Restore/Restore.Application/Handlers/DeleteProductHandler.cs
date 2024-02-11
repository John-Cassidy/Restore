using MediatR;
using Restore.Application.Commands;
using Restore.Application.Services;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageService _imageService;

    public DeleteProductHandler(IUnitOfWork unitOfWork, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
        if (product == null) return Result<bool>.Failure("Product not found");

        if (product.PictureUrl != null)
        {
            var result = await _imageService.DeleteImageAsync(product.PictureUrl);
            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.ErrorMessage);
            }
        }

        await _unitOfWork.ProductRepository.DeleteAsync(product);
        var resultDelete = await _unitOfWork.CompleteAsync() > 0;
        if (!resultDelete)
        {
            return Result<bool>.Failure("Failed to delete product");
        }
        return Result<bool>.Success(true);
    }
}
