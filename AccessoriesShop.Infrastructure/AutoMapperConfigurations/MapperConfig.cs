using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.AutoMapperConfigurations
{
    public class MapperConfig : Profile
    {
        //register all AutoMapper profiles
        public MapperConfig()
        {
            CreateMap<RegisterRequest, Account>().ReverseMap();
        }

    }
}
