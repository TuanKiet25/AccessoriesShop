namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateAddressRequest
    {
        public Guid AccountId { get; set; }
        public string ProvinceCode { get; set; } = string.Empty;
        public string DistrictCode { get; set; } = string.Empty;
        public string WardCode { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
