namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateBrandRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
    }
}
