using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static AccessoriesShop.Application.ViewModels.Responses.AddressResponse;

namespace AccessoriesShop.Infrastructure.Services
{
    public class LocationService : ILocationService
    {
        private Dictionary<string, ProvinceResponse> _locations;
        private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "Data", "tree.json");
        public LocationService()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var jsonString = File.ReadAllText(_filePath);
                _locations = JsonSerializer.Deserialize<Dictionary<string, ProvinceResponse>>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đọc file JSON: {ex.Message}");
                _locations = new Dictionary<string, ProvinceResponse>();
            }
        }
        public Dictionary<string, AddressResponse.ProvinceResponse> GetAllLocations()
        {
           return _locations;
        }

        public (string ProvinceName, string DistrictName, string WardName) GetLocationNames(string provinceCode, string districtCode, string wardCode)
        {
            string pName = "", dName = "", wName = "";

            if (!string.IsNullOrEmpty(provinceCode) && _locations.TryGetValue(provinceCode, out var province))
            {
                pName = province.Name;
                if (!string.IsNullOrEmpty(districtCode) && province.Districts.TryGetValue(districtCode, out var district))
                {
                    dName = district.Name;
                    if (!string.IsNullOrEmpty(wardCode) && district.Wards.TryGetValue(wardCode, out var ward))
                    {
                        wName = ward.Name;
                    }
                }
            }
            return (pName, dName, wName);
        }
    }
}
