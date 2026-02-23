namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateCategoryRequest
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
    }
}
