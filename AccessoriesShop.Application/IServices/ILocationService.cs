using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccessoriesShop.Application.ViewModels.Responses.AddressResponse;

namespace AccessoriesShop.Application.IServices
{
    public interface ILocationService
    {
        Dictionary<string, ProvinceResponse> GetAllLocations();
        (string ProvinceName, string DistrictName, string WardName) GetLocationNames(string provinceCode, string districtCode, string wardCode);
    }
}
