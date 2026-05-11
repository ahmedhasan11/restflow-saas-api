namespace RestflowAPI.DTOs.MenuCategoryDtos
{
    public class MenuCategoryDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
