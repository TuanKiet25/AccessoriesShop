using AccessoriesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IAuthentication
{
    public interface IJwtProvider
    {
        string Generate(Account account);
    }
}
