# 🚀 One-Click Database Seeding - Quick Start

## Add This to Program.cs (Right After `var app = builder.Build();`)

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // ⬇️ ADD THIS LINE ⬇️
    await app.Services.SeedDatabaseAsync();
}
```

That's it! Your PostgreSQL database will be seeded with sample data on startup.

---

## What You Get

✅ **4 User Accounts** - 1 admin + 3 regular users  
✅ **4 Brands** - Apple, Samsung, Anker, Belkin  
✅ **5 Categories** - Smartphones, Cases, Protectors, Chargers, Wireless  
✅ **7 Products** - iPhone cases, chargers, power banks, etc.  
✅ **10+ Product Variants** - Colors, capacities, SKUs  
✅ **3 Sample Orders** - With items, payments, various statuses  
✅ **Realistic Stock** - 30-120 units per variant  

---

## Test Credentials

```
Admin:
  Username: admin
  Password: admin123

Regular User:
  Username: user1
  Password: user123
```

---

## Files Created

| File | Purpose |
|------|---------|
| `DatabaseSeeder.cs` | Main seeding logic with nuke + seed |
| `SeederExtensions.cs` | Extension method for Program.cs |
| `SeedingGuide.md` | Detailed documentation |
| `USAGE_EXAMPLES.cs` | Code examples and reference |
| `QUICKSTART.md` | This file |

---

## Advanced Usage

### Auto-seed only if empty:
```csharp
if (app.Environment.IsDevelopment() && !await context.Products.AnyAsync())
{
    await app.Services.SeedDatabaseAsync();
}
```

### Manual seeding from controller:
```csharp
var seeder = new DatabaseSeeder(_context);
await seeder.NukeAndSeedAsync();
```

### Customize seed data:
Edit the `Seed*()` methods in `DatabaseSeeder.cs`

---

## Important ⚠️

- **Data will be DELETED** - This is not reversible!
- **Development only** - Never use in production
- **Stock-aware** - Works with your stock reservation system
- **PostgreSQL compatible** - Tested with your setup

---

## Troubleshooting

**Q: Nothing happens?**
A: Check your `IsDevelopment()` environment setting

**Q: Foreign key errors?**
A: Data is seeded in correct dependency order. If errors persist, check AppDbContext.

**Q: Want different data?**
A: Edit `DatabaseSeeder.cs` seed methods and rebuild

---

**All set? Run your app and check the console for seeding progress! 🎉**
