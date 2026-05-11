namespace RestflowAPI.DTOs.MenuCategory
{
    public class CreateMenuCategoryDto
    {
        public string? CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
