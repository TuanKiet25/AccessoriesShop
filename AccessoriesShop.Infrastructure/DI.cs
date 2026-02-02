using AccessoriesShop.Application;
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
            
            #endregion
            // Đăng ký services
            #region services
            
            #endregion
            //Đăng ký auto mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Đăng ký JWT authentication
            //var jwtSettings = new JwtSettings();
            //configuration.Bind(JwtSettings.SectionName, jwtSettings);
            //services.AddSingleton(jwtSettings); // Dùng AddSingleton vì cài đặt không thay đổi
            // Cấu hình dịch vụ Authentication của .NET Core

            // Đăng ký MailSettings
            //services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
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
