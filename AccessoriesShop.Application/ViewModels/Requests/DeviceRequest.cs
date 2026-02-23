namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateDeviceRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
