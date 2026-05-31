using FluentValidation;
using RestflowAPI.DTOs.InventoryItems;

namespace RestflowAPI.Validators.InventoryItem
{

    public class UpdateInventoryItemDtoValidator
        : AbstractValidator<UpdateInventoryItemDto>
    {
        public UpdateInventoryItemDtoValidator()
        {

            RuleFor(x => x.ItemName)
                .NotEmpty();

            RuleFor(x => x.CategoryId)
                .NotEmpty();

            RuleFor(x => x.UnitOfMeasure)
                .NotEmpty();

            RuleFor(x => x.MinimumQuantity)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CostPerUnit)
                .GreaterThanOrEqualTo(0);

            
        }
    }
}
