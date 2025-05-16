
using CRMSolution.Grpc.Orders;
using FluentValidation;
using OrderService.Data.Repository.OrderResp;
using OrderService.DTO.Requests;

namespace OrderService.Data.Validators.Order;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    
    public CreateOrderValidator()
    {
        RuleFor(r => r.TotalAmount)
            .NotEmpty()
            .WithMessage("Invalid total amount.")
            .NotNull()
            .WithMessage("Invalid total amount.");
    }
}