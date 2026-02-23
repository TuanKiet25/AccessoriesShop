namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateProductAttributeRequest
    {
        public Guid ProductId { get; set; }
        public Guid AttributeId { get; set; }
        public string? Value { get; set; }
    }
}
