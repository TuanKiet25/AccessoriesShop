using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class AddressResponse
    {
        public class WardResponse
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("path_with_type")]
            public string FullPath { get; set; }
        }

        public class DistrictResponse
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("xa-phuong")]
            public Dictionary<string, WardResponse> Wards { get; set; }
        }

        public class ProvinceResponse
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("quan-huyen")]
            public Dictionary<string, DistrictResponse> Districts { get; set; }
        }
    }
}
