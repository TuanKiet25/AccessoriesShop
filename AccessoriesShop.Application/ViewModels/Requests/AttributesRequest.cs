namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateAttributesRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? DataType { get; set; }
    }
}
