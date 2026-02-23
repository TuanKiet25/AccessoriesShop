namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid BrandId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
