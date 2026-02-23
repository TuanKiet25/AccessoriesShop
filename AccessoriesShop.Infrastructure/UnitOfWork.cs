using AccessoriesShop.Application;
using AccessoriesShop.Application.IRepositories;

namespace AccessoriesShop.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IAccountRepository Accounts { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }
        public IBrandRepository Brands { get; }
        public IDeviceRepository Devices { get; }
        public IProductCompatibilityRepository ProductCompatibilities { get; }
        public IProductRepository Products { get; }
        public IAttributesRepository Attributes { get; }
        public IProductAttributeRepository ProductAttributes { get; }
        public IProductVariantRepository ProductVariants { get; }
        public ICategoryRepository Categories { get; }

        public UnitOfWork(
            AppDbContext context,
            IAccountRepository accounts,
            IOrderRepository orders,
            IOrderItemRepository orderItems,
            IBrandRepository brands,
            IDeviceRepository devices,
            IProductCompatibilityRepository productCompatibilities,
            IProductRepository products,
            IAttributesRepository attributes,
            IProductAttributeRepository productAttributes,
            IProductVariantRepository productVariants,
            ICategoryRepository categories)
        {
            _context = context;
            Accounts = accounts;
            Orders = orders;
            OrderItems = orderItems;
            Brands = brands;
            Devices = devices;
            ProductCompatibilities = productCompatibilities;
            Products = products;
            Attributes = attributes;
            ProductAttributes = productAttributes;
            ProductVariants = productVariants;
            Categories = categories;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
