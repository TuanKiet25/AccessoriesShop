namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public Guid BrandId { get; set; }
        public string? BrandName { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
