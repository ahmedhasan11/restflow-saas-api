namespace RestflowAPI.DTOs.AI.Internal
{
    public class MenuEngineeringData
    {
        public List<ProductSalesMetric> TopSelling { get; set; } = [];

        public List<ProductSalesMetric> LowestSelling { get; set; } = [];
    }

    public class ProductSalesMetric
    {
        public string ProductName { get; set; } = "";

        public int TotalOrders { get; set; }
    }
}