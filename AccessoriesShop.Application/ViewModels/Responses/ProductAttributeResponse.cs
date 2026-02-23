namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class ProductAttributeResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid AttributeId { get; set; }
        public string? AttributeName { get; set; }
        public string? Value { get; set; }
    }
}
