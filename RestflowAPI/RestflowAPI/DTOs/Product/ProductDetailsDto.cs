namespace RestflowAPI.DTOs.Product
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal SellingPrice { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public List<ProductIngredientDto> Ingredients { get; set; } = new();
    }
}
