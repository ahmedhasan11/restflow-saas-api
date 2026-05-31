using FluentValidation;
using RestflowAPI.DTOs.Orders;

namespace RestflowAPI.Validators.OrdersValidators
{
    public class CreateOrderDtoValidator
     : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Order must contain at least one item");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateOrderItemDtoValidator());
        }
    }
}
