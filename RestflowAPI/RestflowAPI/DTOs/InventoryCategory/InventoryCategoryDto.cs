namespace RestflowAPI.DTOs.Inventory
{
    public class InventoryCategoryDto
    {
        public Guid Id { get; set; }

        public string CategoryName { get; set; } = default!;
    }
}
