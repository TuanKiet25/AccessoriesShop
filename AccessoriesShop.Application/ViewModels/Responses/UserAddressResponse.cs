namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class UserAddressResponse
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public string StreetAddress { get; set; }
        public bool IsDefault { get; set; }
    }
}
