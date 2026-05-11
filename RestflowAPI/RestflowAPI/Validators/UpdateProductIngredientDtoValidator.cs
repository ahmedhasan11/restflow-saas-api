using FluentValidation;
using RestflowAPI.DTOs.Product;

public class UpdateProductIngredientDtoValidator
    : AbstractValidator<UpdateProductIngredientDto>
{
    public UpdateProductIngredientDtoValidator()
    {
        When(x => x.QuantityRequired.HasValue, () =>
        {
            RuleFor(x => x.QuantityRequired!.Value)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");
        });
    }
}