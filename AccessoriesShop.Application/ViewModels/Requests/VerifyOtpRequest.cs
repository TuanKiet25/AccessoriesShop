namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class VerifyOtpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}
