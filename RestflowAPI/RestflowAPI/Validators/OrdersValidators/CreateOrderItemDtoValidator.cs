using FluentValidation;
using RestflowAPI.DTOs.Orders;

namespace RestflowAPI.Validators.OrdersValidators
{
    public class CreateOrderItemDtoValidator
    : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}
