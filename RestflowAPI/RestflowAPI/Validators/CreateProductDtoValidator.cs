using FluentValidation;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required");

        RuleFor(x => x.SellingPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be >= 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Ingredients)
            .NotEmpty()
            .WithMessage("At least one ingredient is required");

        RuleForEach(x => x.Ingredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.InventoryItemId)
                    .NotEmpty();

                ingredient.RuleFor(i => i.QuantityRequired)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");
            });
    }
}