public class CreateProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public decimal SellingPrice { get; set; }
    public bool IsAvailable { get; set; } = true;

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public List<CreateProductIngredientDto> Ingredients { get; set; } = new();
}
