namespace RestflowAPI.DTOs.Product
{
    public class UpdateProductIngredientDto
    {
        public Guid? InventoryItemId { get; set; }
        public decimal? QuantityRequired { get; set; }
    }
}
