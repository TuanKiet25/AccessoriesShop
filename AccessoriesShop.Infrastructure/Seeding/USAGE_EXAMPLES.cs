// ============================================================================
// DATABASE SEEDING - USAGE EXAMPLES (Reference Only)
// ============================================================================
// This file contains examples of how to use the DatabaseSeeder
// These are code snippets - not executable code
// Copy and adapt as needed for your project

/*

// ============================================================================
// OPTION 1: Automatic Seeding on Application Startup (Recommended)
// ============================================================================

In Program.cs:

using AccessoriesShop.Infrastructure;
using AccessoriesShop.Infrastructure.Seeding;

var builder = WebApplicationBuilder.CreateBuilder(args);
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // 🎯 SEED DATABASE ON STARTUP (Development only)
    await app.Services.SeedDatabaseAsync();
}

app.Run();


// ============================================================================
// OPTION 2: Seed Database from a Console Command
// ============================================================================

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
    })
    .Build();

using var scope = host.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var seeder = new DatabaseSeeder(context);

await seeder.NukeAndSeedAsync();


// ============================================================================
// OPTION 3: Create an Admin API Endpoint for Seeding
// ============================================================================

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("seed-database")]
        public async Task<IActionResult> SeedDatabase()
        {
            var seeder = new DatabaseSeeder(_context);
            
            try
            {
                await seeder.NukeAndSeedAsync();
                return Ok(new { message = "Database seeded successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Seeding failed", error = ex.Message });
            }
        }
    }
}

Endpoint: POST /api/admin/seed-database (Admin only)


// ============================================================================
// OPTION 4: Check and Seed if Empty
// ============================================================================

In Program.cs:

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

if (!context.Products.Any())
{
    Console.WriteLine("Database is empty. Seeding...");
    var seeder = new DatabaseSeeder(context);
    await seeder.NukeAndSeedAsync();
}


// ============================================================================
// SAMPLE CREDENTIALS (After Seeding)
// ============================================================================

Admin Account:
  Username: admin
  Password: admin123
  Email: admin@example.com

User Accounts:
  Username: user1 | Password: user123 | Email: user1@example.com
  Username: user2 | Password: user123 | Email: user2@example.com
  Username: user3 | Password: user123 | Email: user3@example.com


// ============================================================================
// SAMPLE DATA INCLUDED
// ============================================================================

Brands: Apple, Samsung, Anker, Belkin
Categories: Smartphones, Phone Cases, Screen Protectors, Chargers & Cables, Wireless Chargers
Devices: iPhone 15, 14, 13, Samsung Galaxy S24, S23, iPad Pro
Attributes: Color, Material, Weight, Capacity, Warranty

Products (7 total):
  - Premium iPhone 15 Case (2 variants: Black, Blue)
  - iPhone 15 Screen Protector (1 variant)
  - Fast USB-C Charger (1 variant)
  - Wireless Charging Pad (1 variant)
  - Samsung Galaxy Case (1 variant)
  - USB-C Cable 3m (1 variant)
  - Power Bank 20000mAh (1 variant)

Orders (3 total with items and payments):
  - Order 1: 2x iPhone Case + 1x Screen Protector → Pending
  - Order 2: 1x Wireless Charger → Confirmed
  - Order 3: 1x Power Bank + 1x USB Cable → Pending


// ============================================================================
// KEY FEATURES
// ============================================================================

✅ Nuke Option: Safely deletes all data in reverse dependency order
✅ Proper Ordering: Seeds data respecting all foreign key constraints
✅ Stock Integration: Creates ProductVariants with realistic stock quantities
✅ Payment Samples: Includes PayOS payment samples for testing
✅ Order Samples: Creates orders with OrderItems and Payments
✅ Password Hashing: All passwords encrypted with BCrypt
✅ Console Logging: Detailed progress output


// ============================================================================
// IMPORTANT NOTES
// ============================================================================

⚠️  WARNING:
  - Seeder DELETES ALL DATA - use only in development!
  - Never call in production
  - Always backup before seeding

Dependency Order:
  1. Categories
  2. Brands
  3. Devices
  4. Attributes
  5. Accounts
  6. Products
  7. ProductVariants
  8. ProductAttributes
  9. ProductCompatibilities
  10. Orders
  11. OrderItems
  12. Payments

Customization:
  - Edit DatabaseSeeder.cs to change seed data
  - Modify SeedBrands(), SeedProducts(), etc.
  - Add/remove sample data as needed

Testing Integration:
  - Test orders with realistic stock quantities
  - Test payment flows with PayOS samples
  - Test stock reservation with pre-loaded variants

*/
