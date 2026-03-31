using AccessoriesShop.Application.Interfaces;
using AccessoriesShop.Application.Common.Settings;
using AccessoriesShop.Application.Interfaces.Authentication;
using AccessoriesShop.Application.Interfaces.Repositories;
using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.Services;
using AccessoriesShop.Infrastructure.Authentication;
using AccessoriesShop.Infrastructure.Repositories;
using AccessoriesShop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.AddHttpContextAccessor();
            // Đăng ký AppDbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            //  đăng ký settings (DI đang ko chạy)
            services.Configure<PayOSSettings>(configuration.GetSection(PayOSSettings.SectionName));
            services.Configure<MailSettings>(configuration.GetSection(MailSettings.SectionName));
            services.Configure<ClientSettings>(configuration.GetSection(ClientSettings.SectionNam));
            services.Configure<GroqSettings>(configuration.GetSection(GroqSettings.SectionName));
            // Đăng ký repositiries
            #region Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IProductCompatibilityRepository, ProductCompatibilityRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAttributesRepository, AttributesRepository>();
            services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOtpVerificationRepository, OtpVerificationRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICustomOrderRepository, CustomOrderRepository>();
			services.AddScoped<IRatingRepository, RatingRepository>();
			services.AddScoped<IPromotionRepository, PromotionRepository>();
			#endregion
			// Đăng ký services
			#region Services
			services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAttributesService, AttributesService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IProductCompatibilityService, ProductCompatibilityService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPayOSService, PayOSService>();
            services.AddScoped<IStockReservationService, StockReservationService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<ICustomOrderService, CustomOrderService>();
			services.AddScoped<IRatingService, RatingService>();
			services.AddScoped<IPromotionService, PromotionService>();
			#endregion
			//Đăng ký auto mapper
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IAIVisionService, GroqService>();

            // Đăng ký CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                                  policy =>
                                  {
                                      // Cho phép origin của frontend được truy cập
                                      policy.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });

            //đăng ký HttpContextAccessor
            services.AddHttpContextAccessor();

            // Đăng ký HttpClient cho Groq
            services.AddHttpClient("Groq");
            return services;
        }
    }
}
