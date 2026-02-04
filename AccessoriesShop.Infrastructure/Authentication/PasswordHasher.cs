using AccessoriesShop.Application.IAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
