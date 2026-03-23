namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateCustomOrderRequest
    {
        public Guid? AccountId { get; set; }
        public Guid? ProductBaseId { get; set; }
        public string? Color { get; set; }
        public string? Material { get; set; }
        public string? TextContent { get; set; }
        public string? Note { get; set; }
        public int Quantity { get; set; } = 1;
        public List<string>? ImageUrls { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
    }

    public class QuoteCustomOrderRequest
    {
        public decimal Price { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateCustomOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
