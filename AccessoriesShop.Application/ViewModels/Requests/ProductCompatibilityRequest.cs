namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateProductCompatibilityRequest
    {
        public Guid ProductId { get; set; }
        public Guid DeviceId { get; set; }
        public string? Note { get; set; }
    }
}
