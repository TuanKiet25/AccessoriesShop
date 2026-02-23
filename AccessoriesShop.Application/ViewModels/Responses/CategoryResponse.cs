namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? ParentName { get; set; }
    }
}
