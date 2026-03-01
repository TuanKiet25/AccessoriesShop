# Database Seeding Architecture & Flow

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Application                          │
│                      (Program.cs)                           │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       │ IsDevelopment() = true
                       │
                       ▼
        ┌──────────────────────────────┐
        │  SeederExtensions            │
        │  .SeedDatabaseAsync()        │
        └──────────────┬───────────────┘
                       │
                       ▼
        ┌──────────────────────────────┐
        │  DatabaseSeeder              │
        │  .NukeAndSeedAsync()         │
        │  ├─ NukeDatabase()           │
        │  └─ SeedDatabase()           │
        └──────────────┬───────────────┘
                       │
          ┌────────────┴────────────┐
          │                         │
          ▼                         ▼
    ┌──────────────┐        ┌──────────────┐
    │  DELETE ALL  │        │  CREATE NEW  │
    │  (Reverse)   │        │  (Ordered)   │
    └──────────────┘        └──────────────┘
          │                         │
          ▼                         ▼
    PostgreSQL DB            PostgreSQL DB
```

## Seeding Flow - Dependency Order

```
Step 1: NukeDatabase() → Delete in REVERSE order
┌─────────────────────────────────┐
│ 1. Payments                     │
│ 2. OrderItems                   │
│ 3. Orders                       │
│ 4. ProductAttributes            │
│ 5. ProductCompatibilities       │
│ 6. ProductVariants              │
│ 7. Products                     │
│ 8. Attributes                   │
│ 9. Devices                      │
│ 10. Brands                      │
│ 11. Categories                  │
│ 12. Accounts                    │
│ 13. OtpVerifications            │
└─────────────────────────────────┘

Step 2: SeedDatabase() → Create in CORRECT order

     ┌──────────────┐
     │ Categories   │ (no dependencies)
     └──────┬───────┘
            │
     ┌──────▼───────┐
     │  Brands      │ (no dependencies)
     └──────┬───────┘
            │
     ┌──────▼───────┐
     │  Devices     │ (no dependencies)
     └──────┬───────┘
            │
     ┌──────▼───────────────┐
     │  Attributes          │ (no dependencies)
     └──────┬───────────────┘
            │
     ┌──────▼───────┐
     │  Accounts    │ (no dependencies)
     └──────┬───────┘
            │
    ┌───────┴────────────────────┐
    │ Products                   │
    │ (depends on Brand+Category)│
    └───────┬────────────────────┘
            │
    ┌───────▼──────────────────┐
    │ ProductVariants          │
    │ (depends on Product)     │
    └───────┬──────────────────┘
            │
    ┌───────┴──────────────────┐
    │ ProductAttributes        │
    │ (depends on Product+Attr)│
    └───────┬──────────────────┘
            │
    ┌───────▼────────────────────────┐
    │ ProductCompatibilities         │
    │ (depends on Product+Device)    │
    └───────┬────────────────────────┘
            │
    ┌───────▼──────────────────┐
    │ Orders                   │
    │ (depends on Account)     │
    └───────┬──────────────────┘
            │
    ┌───────▼──────────────────────┐
    │ OrderItems                   │
    │ (depends on Order+Variant)   │
    └───────┬──────────────────────┘
            │
    ┌───────▼──────────────────┐
    │ Payments                 │
    │ (depends on Order)       │
    └───────┬──────────────────┘
            │
            ✅ DONE
```

## Entity Relationship Diagram

```
                    Account
                      │
                      ├─ Order
                      │   ├─ OrderItem ──┐
                      │   └─ Payment     │
                      └─ OtpVerification │
                                         │
                                    ProductVariant
                                      │   │
Category ├─ Product ├─ ProductVariant ──┘
    │        │       │   │
    │    Brand       ProductAttribute ──┐
    │                │                  │
    │                └─ Attributes     │
    │
    └─ ProductCompatibility ──┐
                              │
                            Attributes


    Device ──┐
            │
    ProductCompatibility
            │
            └─ Product
```

## Data Volume

```
┌─────────────────────────────────────────┐
│           Sample Data Created           │
├─────────────────────────────────────────┤
│ Categories           │  5               │
│ Brands               │  4               │
│ Devices              │  6               │
│ Attributes           │  5               │
│ Accounts             │  4 (1 admin)     │
│ Products             │  7               │
│ ProductVariants      │  10+             │
│ ProductAttributes    │  11              │
│ ProductCompatibilitie│  9               │
│ Orders               │  3               │
│ OrderItems           │  5               │
│ Payments             │  3               │
├─────────────────────────────────────────┤
│ TOTAL RECORDS        │  ~72             │
└─────────────────────────────────────────┘
```

## Stock Reservation Integration

```
User Action                    Database State

[Seed Database]
    ↓
ProductVariant created
stock = 50 units
    │
    ├─→ [DB] stock_quantity = 50
    │       (available inventory)
    │
    ├─→ [Order Created]
    │       ↓
    │   [Stock Reserved]
    │   stock_quantity -= quantity
    │       ↓
    │   [DB] stock_quantity = 45
    │
    ├─→ [Payment Success]
    │       ↓
    │   [Stock Confirmed]
    │   (no change, stays at 45)
    │
    └─→ [Payment Failed]
            ↓
        [Stock Reverted]
        stock_quantity += quantity
            ↓
        [DB] stock_quantity = 50
            (back to original)
```

## Console Output Example

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

## File Organization

```
AccessoriesShop.Infrastructure/
│
└─ Seeding/
   ├─ DatabaseSeeder.cs
   │  └─ Main logic: NukeDatabase() + SeedDatabase()
   │
   ├─ SeederExtensions.cs
   │  └─ Extension method: SeedDatabaseAsync()
   │
   ├─ QUICKSTART.md
   │  └─ One-liner setup guide
   │
   ├─ SeedingGuide.md
   │  └─ Comprehensive documentation
   │
   ├─ ARCHITECTURE.md
   │  └─ This file
   │
   └─ USAGE_EXAMPLES.cs
      └─ Code examples and patterns
```

## Integration Points

```
┌─────────────────────────────────────────┐
│         stockReservationService         │
│  (Auto-called by OrderService.Create)   │
└────────────────────┬────────────────────┘
                     │
                     │ Integrates with
                     │
    ┌────────────────▼────────────────┐
    │      DatabaseSeeder             │
    │                                 │
    │  Creates ProductVariants with:  │
    │  - Initial stock_quantity = N   │
    │  - Ready for order reserve      │
    │                                 │
    │  Creates Orders with:           │
    │  - OrderItems (linked variants) │
    │  - Payments (PayOS samples)     │
    └────────────────┬────────────────┘
                     │
    ┌────────────────▼────────────────┐
    │      PostgreSQL Database        │
    │                                 │
    │  Stock-aware test data ready    │
    │  for integration testing        │
    └─────────────────────────────────┘
```

## Execution Timeline

```
Application Start
    │
    ├─ IsDevelopment()?
    │   No  → Skip seeding, run normally
    │   Yes → Continue
    │
    ├─ app.Services.SeedDatabaseAsync()
    │   │
    │   ├─ Create scope
    │   │   │
    │   ├─ Get AppDbContext
    │   │   │
    │   ├─ Create DatabaseSeeder
    │   │   │
    │   ├─ NukeAndSeedAsync()
    │   │   │
    │   │   ├─ [~200ms] DELETE all (reverse order)
    │   │   │
    │   │   └─ [~300ms] CREATE all (correct order)
    │   │
    │   └─ Logging & completion
    │       │
    │       └─ Total: ~500ms
    │
    ├─ Database ready with sample data
    │
    └─ Application continues to app.Run()
        (All endpoints now have test data)
```

---

**Ready to use? Check QUICKSTART.md!**
