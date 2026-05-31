using FluentValidation;
using RestflowAPI.DTOs.InventoryCategory;
using RestflowAPI.DTOs.Inventory;

namespace RestflowAPI.Validators.Inventory
{
    public class CreateInventoryCategoryValidator : AbstractValidator<CreateInventoryCategoryDto>
    {
        public CreateInventoryCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
