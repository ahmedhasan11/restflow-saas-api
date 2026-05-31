namespace RestflowAPI.Validators.InventoryItem
{
    using FluentValidation;
    using RestflowAPI.DTOs.InventoryItems;

    public class CreateInventoryItemDtoValidator
        : AbstractValidator<CreateInventoryItemDto>
    {
        public CreateInventoryItemDtoValidator()
        {
            RuleFor(x => x.ItemName)
            .NotEmpty();

            RuleFor(x => x.CategoryId).NotEmpty();

            RuleFor(x => x.UnitOfMeasure)
                .NotEmpty();

            RuleFor(x => x.CurrentQuantity)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MinimumQuantity)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CostPerUnit)
                .GreaterThanOrEqualTo(0);
        }
    }
}
