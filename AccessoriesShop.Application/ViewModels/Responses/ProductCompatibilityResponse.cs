namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class ProductCompatibilityResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? Note { get; set; }
    }
}
