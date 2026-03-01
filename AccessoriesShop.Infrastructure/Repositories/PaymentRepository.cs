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
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(gateWay))
                return null!;

            var codeNormalized = code.Trim().ToLower();
            var gatewayNormalized = gateWay.Trim().ToLower();

            // First, filter by gateway in the database to narrow results
            // This reduces the amount of data fetched
            var paymentsByGateway = await _db.Include(p => p.Order)
                .Where(p =>
                    (p.PaymentMethod != null && p.PaymentMethod.ToLower() == gatewayNormalized)
                    || (p.BankCode != null && p.BankCode.ToLower() == gatewayNormalized)
                )
                .ToListAsync();

            // Then perform case-insensitive code comparison in memory
            var payment = paymentsByGateway.FirstOrDefault(p =>
                (!string.IsNullOrEmpty(p.TransactionRef) && p.TransactionRef.ToLower() == codeNormalized)
                || (!string.IsNullOrEmpty(p.TransactionCode) && p.TransactionCode.ToLower() == codeNormalized)
            );

            return payment!;
        }
    }
}
