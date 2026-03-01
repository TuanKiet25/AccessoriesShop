# Database Seeding Guide

## Overview

The `DatabaseSeeder` provides a **one-click seeding solution** for your AccessoriesShop PostgreSQL database with comprehensive sample data.

### Features

✅ **Nuke Option** - Completely clears all data safely (in reverse dependency order)  
✅ **Proper Ordering** - Seeds data respecting all foreign key constraints  
✅ **Sample Data** - Includes realistic e-commerce data (brands, products, variants, orders, payments)  
✅ **PostgreSQL Compatible** - Works seamlessly with PostgreSQL  
✅ **Single Command** - One-click execution in Program.cs  

---

## Quick Start

### Option 1: Seed on Application Startup (Development Only)

Edit your `Program.cs`:

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// ... existing service configuration ...
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// 🎯 ADD THIS LINE to seed database on startup
if (app.Environment.IsDevelopment())
{
    await app.Services.SeedDatabaseAsync();
}

// ... rest of your configuration ...
app.Run();
```

### Option 2: Call Seeding Manually

Create a simple console app or API endpoint:

```csharp
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var seeder = new DatabaseSeeder(context);
await seeder.NukeAndSeedAsync();
```

---

## Data Structure

The seeder creates data in this **dependency order**:

```
1. Categories (base reference data)
   ↓
2. Brands (base reference data)
   ↓
3. Devices (base reference data)
   ↓
4. Attributes (base reference data)
   ↓
5. Accounts (users)
   ↓
6. Products (depends on Brand + Category)
   ↓
7. ProductVariants (depends on Product)
   ↓
8. ProductAttributes (depends on Product + Attributes)
   ↓
9. ProductCompatibilities (depends on Product + Device)
   ↓
10. Orders (depends on Account)
    ↓
11. OrderItems (depends on Order + ProductVariant)
    ↓
12. Payments (depends on Order)
```

---

## Sample Data Included

### Accounts (3 Users)
- **admin** / `admin123` → Role: Admin
- **user1** / `user123` → Role: User
- **user2** / `user123` → Role: User
- **user3** / `user123` → Role: User

### Brands (4)
- Apple
- Samsung
- Anker
- Belkin

### Categories (5)
- Smartphones
- Phone Cases
- Screen Protectors
- Chargers & Cables
- Wireless Chargers

### Devices (6)
- iPhone 15, 14, 13
- Samsung Galaxy S24, S23
- iPad Pro

### Products (7)
- Premium iPhone 15 Case
- iPhone 15 Screen Protector
- Fast USB-C Charger
- Wireless Charging Pad
- Samsung Galaxy Case
- USB-C Cable 3m
- Power Bank 20000mAh

### Product Variants (10+)
- Each product has realistic variants (colors, capacities, etc.)
- Stock quantities ranging from 30-120 units

### Orders (3)
- Sample orders with different statuses (Pending, Confirmed)
- Orders linked to different users

### Payments (3)
- PayOS integration samples
- Different payment statuses (Pending, Success)

---

## Important Notes

### ⚠️ WARNING
**The `nuke` option will DELETE ALL DATA!** Use only in development/testing environments.

### 🔒 Recommended Usage
1. Use on **development** environment only
2. Call on **first run** or when resetting data
3. Do NOT call in **production**

### 🗑️ What Gets Deleted
All data in these tables (in reverse order):
- Payments
- OrderItems
- Orders
- ProductAttributes
- ProductCompatibilities
- ProductVariants
- Products
- Attributes
- Devices
- Brands
- Categories
- Accounts
- OtpVerifications

### ✅ What Gets Created
Fresh sample data with:
- 4 user accounts (1 admin, 3 regular users)
- 4 brands with 5 categories
- 7 products with 10+ variants
- 3 sample orders
- 3 payment records

---

## Methods

### `NukeAndSeedAsync()`
Main method that:
1. Deletes all data
2. Seeds fresh data
3. Logs progress to console

### `ReserveStockAsync()` Integration
The seeder works with the stock reservation system:
- Creates ProductVariants with initial stock quantities
- These quantities serve as available inventory
- When orders are created, stock is reserved automatically

---

## Example Output

```
🗑️  Starting database nuke...
   ✓ All tables cleared
🌱 Seeding database with sample data...
   ✓ Categories seeded
   ✓ Brands seeded
   ✓ Devices seeded
   ✓ Attributes seeded
   ✓ Accounts seeded
   ✓ Products seeded
   ✓ Product Variants seeded
   ✓ Product Attributes seeded
   ✓ Product Compatibilities seeded
   ✓ Orders seeded
   ✓ Order Items seeded
   ✓ Payments seeded
✅ Database seeding completed successfully!
```

---

## Customization

To customize seed data, edit `DatabaseSeeder.cs`:

```csharp
private List<Brand> SeedBrands()
{
    var brands = new List<Brand>
    {
        new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Your Brand",
            Description = "Your description",
            LogoUrl = "https://your-url.com/logo.png"
        }
        // Add more brands...
    };

    _context.Brands.AddRange(brands);
    return brands;
}
```

---

## Troubleshooting

### Issue: Foreign key constraint error
**Solution:** Ensure data is seeded in the correct order. The seeder handles this automatically.

### Issue: Duplicate key error
**Solution:** Run nuke first to clear existing data.

### Issue: Password hashing errors
**Solution:** The seeder uses BCrypt (same as your application). Ensure `PasswordHasher` is available.

### Issue: PostgreSQL connection error
**Solution:** Verify your `DefaultConnection` in `appsettings.json` is correct.

---

## Security Notes

🔐 **Production WARNING:**
- **NEVER** run seeder in production
- Passwords are hashed with BCrypt
- Sample passwords are for demo only
- Change all passwords before going live

---

## Files

- `DatabaseSeeder.cs` - Main seeding logic
- `SeederExtensions.cs` - Extension method for easy integration
- `SeedingGuide.md` - This file (documentation)

---

## Support

If you encounter issues:
1. Check the console output for detailed error messages
2. Verify your database connection string
3. Ensure all migrations are applied
4. Review the dependency order above
