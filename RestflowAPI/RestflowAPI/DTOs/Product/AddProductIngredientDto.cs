namespace RestflowAPI.DTOs.Product
{
    public class AddProductIngredientDto
    {
        public Guid ProductId { get; set; }
        public Guid InventoryItemId { get; set; }
        public decimal QuantityRequired { get; set; }
    }
}
