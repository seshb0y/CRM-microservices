using CRMSolution.Grpc.Orders;
using FluentValidation;
using OrderService.Data.Repository.Interface;
using OrderService.DTO.Requests;

namespace OrderService.Data.Validators.Order;

public class DeleteOrderValidator : AbstractValidator<DeleteOrderRequest>
{
    IRepository<Models.Order> _orderRepository;

    public DeleteOrderValidator(IRepository<Models.Order> orderRepository)
    {
        _orderRepository = orderRepository;
        
        RuleFor(r => r.OrderId)
            .NotEmpty()
            .WithMessage("Invalid order ID.")
            .MustAsync(IsOrderExist)
            .WithMessage("The order ID does not exist.");
    }

    private async Task<bool> IsOrderExist(int id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetById(id);
        return order != null;
    }
}