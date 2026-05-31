
using FluentValidation;
using RestflowAPI.DTOs.StockTransaction;
namespace RestflowAPI.Validators.StockMovement
{

    public class CreateStockMovementDtoValidator
        : AbstractValidator<CreateStockMovementDto>
    {
        public CreateStockMovementDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0);

            RuleFor(x => x.TransactionType)
                .IsInEnum();
        }
    }
}
