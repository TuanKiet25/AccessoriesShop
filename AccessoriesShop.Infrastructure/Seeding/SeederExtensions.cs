using AccessoriesShop.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Seeding
{
    /// <summary>
    /// Extension methods for database seeding in the application startup
    /// </summary>
    public static class SeederExtensions
    {
        /// <summary>
        /// Seeds the database with sample data (development only)
        /// Add this to Program.cs after building the app to seed on startup:
        /// 
        ///   var app = builder.Build();
        ///   await app.Services.SeedDatabaseAsync(); // Add this line
        ///   app.Run();
        ///   
        /// </summary>
        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var seeder = new DatabaseSeeder(context);

                try
                {
                    // your seeding option here
                    // await seeder._____

                    //await seeder.SeedDatabase();

                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database seeding failed: {ex}");
                    throw;
                }
            }
        }
    }
}
