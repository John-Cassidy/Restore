using AutoMapper;
using MediatR;
using Restore.Application.Commands;
using Restore.Application.Responses;
using Restore.Application.Services;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class CreateOrUpdatePaymentIntentHandler : IRequestHandler<CreateOrUpdatePaymentIntentCommand, Result<BasketResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;

    public CreateOrUpdatePaymentIntentHandler(IUnitOfWork unitOfWork, IPaymentService paymentService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _mapper = mapper;
    }

    public async Task<Result<BasketResponse>> Handle(CreateOrUpdatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        // get basket
        var basket = await _unitOfWork.BasketRepository.ReadAsync(request.BuyerId);

        if (basket == null || !basket.Items.Any())
        {
            return Result<BasketResponse>.Failure($"Basket with id {request.BuyerId} not found", 404); // StatusCodes.Status404NotFound,
        }
        var intent = await _paymentService.CreateOrUpdatePaymentIntent(basket);

        if (intent == null) return Result<BasketResponse>.Failure("Problem creating payment intent", 400); // StatusCodes.Status400BadRequest,

        basket.PaymentIntentId = basket.PaymentIntentId ?? intent.Id;
        basket.ClientSecret = basket.ClientSecret ?? intent.ClientSecret;

        await _unitOfWork.BasketRepository.UpdateAsync(basket);

        var result = await _unitOfWork.CompleteAsync() > 0;

        if (!result) return Result<BasketResponse>.Failure("Problem updating basket with intent", 400); // StatusCodes.Status400BadRequest,

        return Result<BasketResponse>.Success(_mapper.Map<BasketResponse>(basket));
    }
}
