using AccessoriesShop.Application;
using AccessoriesShop.Application.IAuthentication;
using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.Services;
using AccessoriesShop.Infrastructure.Authentication;
using AccessoriesShop.Infrastructure.Repositories;
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
            #endregion
            // Đăng ký services
            #region Services
            services.AddScoped<IAuthService, AuthService>();
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
            #endregion
            //Đăng ký auto mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
            //services.AddHttpContextAccessor();
            return services;
        }
    }
}
