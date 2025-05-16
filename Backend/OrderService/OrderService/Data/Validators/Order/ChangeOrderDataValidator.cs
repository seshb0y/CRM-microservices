using System.Data;
using CRMSolution.Grpc.Orders;
using FluentValidation;
using OrderService.Data.Repository.Interface;
using OrderService.DTO.Requests;

namespace OrderService.Data.Validators.Order;

public class ChangeOrderDataValidator : AbstractValidator<ChangeOrderDataRequest>
{
    
    IRepository<Models.Order> _orderRepository;
    
    public ChangeOrderDataValidator(IRepository<Models.Order> orderRepository)
    {
        _orderRepository = orderRepository;
        
        RuleFor(r => r.OrderId)
            .NotEmpty()
            .WithMessage("Invalid order ID.")
            .MustAsync(IsOrderExist)
            .WithMessage("Order with this ID is not found.");

        RuleFor(r => r.TotalAmount)
            .NotEmpty()
            .WithMessage("Invalid total amount.")
            .NotNull()
            .WithMessage("Invalid total amount.");
    }

    private async Task<bool> IsOrderExist(int id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetById(id);
        return order != null;
    }
}