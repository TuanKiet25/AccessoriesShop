namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class CustomOrderFileResponse
    {
        public Guid Id { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? FileName { get; set; }
    }

    public class CustomOrderResponse
    {
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? ProductBaseId { get; set; }
        public string? Color { get; set; }
        public string? Material { get; set; }
        public string? TextContent { get; set; }
        public string? Note { get; set; }
        public int Quantity { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? Status { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public List<CustomOrderFileResponse>? Files { get; set; }
    }
}
