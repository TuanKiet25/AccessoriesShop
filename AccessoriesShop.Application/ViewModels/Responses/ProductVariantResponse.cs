namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class ProductVariantResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal Price { get; set; }
    }
}
