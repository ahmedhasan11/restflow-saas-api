using FluentValidation;
using RestflowAPI.DTOs.Product;

public class AddProductIngredientDtoValidator
    : AbstractValidator<AddProductIngredientDto>
{
    public AddProductIngredientDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.InventoryItemId)
            .NotEmpty();

        RuleFor(x => x.QuantityRequired)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
    }
}