using AccessoriesShop.Application;
using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Payment> GetByOrderCodeAsync(string code, string gateWay)
        {
            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(gateWay))
                return null!;

            // Start from base queryable and include Order navigation
            var query = _db.Include(p => p.Order);

            var codeNormalized = code?.Trim();
            var gatewayNormalized = gateWay?.Trim();

            var payment = await query.FirstOrDefaultAsync(p =>
                (
                    (!string.IsNullOrEmpty(p.TransactionRef) && string.Equals(p.TransactionRef, codeNormalized, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrEmpty(p.TransactionCode) && string.Equals(p.TransactionCode, codeNormalized, StringComparison.OrdinalIgnoreCase))
                )
                &&
                (
                    (!string.IsNullOrEmpty(p.PaymentMethod) && string.Equals(p.PaymentMethod, gatewayNormalized, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrEmpty(p.BankCode) && string.Equals(p.BankCode, gatewayNormalized, StringComparison.OrdinalIgnoreCase))
                )
            );
            return payment!;
        }
    }
}
