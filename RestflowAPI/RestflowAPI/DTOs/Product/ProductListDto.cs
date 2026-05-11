namespace RestflowAPI.DTOs.Product
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
}
