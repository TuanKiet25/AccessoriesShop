namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateProductVariantRequest
    {
        public Guid ProductId { get; set; }
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal Price { get; set; }
    }
}
