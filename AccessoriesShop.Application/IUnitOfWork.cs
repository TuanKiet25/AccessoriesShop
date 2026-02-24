using AccessoriesShop.Application.IRepositories;

namespace AccessoriesShop.Application
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IBrandRepository Brands { get; }
        IDeviceRepository Devices { get; }
        IProductCompatibilityRepository ProductCompatibilities { get; }
        IProductRepository Products { get; }
        IAttributesRepository Attributes { get; }
        IProductAttributeRepository ProductAttributes { get; }
        IProductVariantRepository ProductVariants { get; }
        ICategoryRepository Categories { get; }
        IOtpVerificationRepository OtpVerifications { get; }
        Task<int> SaveChangesAsync();
    }
}
