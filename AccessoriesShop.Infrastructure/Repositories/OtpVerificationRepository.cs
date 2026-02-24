using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class OtpVerificationRepository : GenericRepository<OtpVerification>, IOtpVerificationRepository
    {
        public OtpVerificationRepository(AppDbContext context) : base(context)
        {
        }
    }
}
