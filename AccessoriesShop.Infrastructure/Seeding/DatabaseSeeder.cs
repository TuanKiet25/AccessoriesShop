using AccessoriesShop.Domain.Entities;
using AccessoriesShop.Domain.Enums;
using AccessoriesShop.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Seeding
{
    /// <summary>
    /// Database seeding service for development and testing
    /// Provides nuke-and-seed functionality with proper dependency ordering
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher _passwordHasher;

        public DatabaseSeeder(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher();
        }

        /// <summary>
        /// Nukes the database and seeds it with sample data
        /// WARNING: This will delete ALL data!
        /// </summary>
        public async Task NukeAndSeedAsync()
        {
            try
            {
                Console.WriteLine("🗑️  Starting database nuke...");
                await NukeDatabase();
                
                Console.WriteLine("🌱 Seeding database with sample data...");
                await SeedDatabase();
                
                Console.WriteLine("✅ Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during seeding: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes all data from the database in reverse dependency order
        /// </summary>
        public async Task NukeDatabase()
        {
            // Disable foreign key constraints during delete
            await _context.Database.ExecuteSqlRawAsync("SET CONSTRAINTS ALL DEFERRED;");

            // Delete in reverse dependency order
            _context.Payments.RemoveRange(_context.Payments);
            _context.OrderItems.RemoveRange(_context.OrderItems);
            _context.Orders.RemoveRange(_context.Orders);
            _context.ProductAttributes.RemoveRange(_context.ProductAttributes);
            _context.ProductCompatibilities.RemoveRange(_context.ProductCompatibilities);
            
            // Get all ProductVariants first to check if they exist
            var variants = await _context.ProductVariants.ToListAsync();
            _context.ProductVariants.RemoveRange(variants);
            
            _context.Products.RemoveRange(_context.Products);
            _context.Attributes.RemoveRange(_context.Attributes);
            _context.Devices.RemoveRange(_context.Devices);
            _context.Brands.RemoveRange(_context.Brands);
            _context.Categories.RemoveRange(_context.Categories);
            _context.Accounts.RemoveRange(_context.Accounts);
            _context.OtpVerifications.RemoveRange(_context.OtpVerifications);

            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ All tables cleared");
       
        }

        public async Task CascadeDeleteAllAsync()
        {
            try
            {
                // Disable triggers
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"OrderItems\" DISABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Orders\" DISABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Payments\" DISABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"ProductAttributes\" DISABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"ProductCompatibilities\" DISABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"ProductVariants\" DISABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Products\" DISABLE TRIGGER ALL");

                // Delete all data
                _context.OrderItems.RemoveRange(_context.OrderItems);
                _context.Payments.RemoveRange(_context.Payments);
                _context.Orders.RemoveRange(_context.Orders);
                _context.ProductAttributes.RemoveRange(_context.ProductAttributes);
                _context.ProductCompatibilities.RemoveRange(_context.ProductCompatibilities);
                _context.ProductVariants.RemoveRange(_context.ProductVariants);
                _context.Products.RemoveRange(_context.Products);
                _context.Categories.RemoveRange(_context.Categories);
                _context.Brands.RemoveRange(_context.Brands);
                _context.Devices.RemoveRange(_context.Devices);
                _context.Attributes.RemoveRange(_context.Attributes);
                _context.OtpVerifications.RemoveRange(_context.OtpVerifications);
                _context.Accounts.RemoveRange(_context.Accounts);

                await _context.SaveChangesAsync();

                // Re-enable triggers
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"OrderItems\" ENABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Orders\" ENABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Payments\" ENABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"ProductAttributes\" ENABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"ProductCompatibilities\" ENABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"ProductVariants\" ENABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Products\" ENABLE TRIGGER ALL");

                Console.WriteLine("✅ All tables cascaded!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Cascade failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Seeds the database with sample data in dependency order
        /// </summary>
        public async Task SeedDatabase()
        {
            // 1. Seed Categories (parent first)
            var categories = SeedCategories();
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Categories seeded");

            // 2. Seed Brands
            var brands = SeedBrands();
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Brands seeded");

            // 3. Seed Devices
            var devices = SeedDevices();
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Devices seeded");

            // 4. Seed Attributes
            var attributes = SeedAttributes();
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Attributes seeded");

            // 5. Seed Accounts
            var accounts = SeedAccounts();
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Accounts seeded");

            // 6. Seed Products
            var products = SeedProducts(brands, categories);
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Products seeded");

            // 7. Seed ProductVariants
            var variants = SeedProductVariants(products);
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Product Variants seeded");

            // 8. Seed ProductAttributes
            SeedProductAttributes(products, attributes);
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Product Attributes seeded");

            // 9. Seed ProductCompatibilities
            SeedProductCompatibilities(products, devices);
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Product Compatibilities seeded");

            // 10. Seed Orders
            var orders = SeedOrders(accounts);
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Orders seeded");

            // 11. Seed OrderItems
            SeedOrderItems(orders, variants);
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Order Items seeded");

            // 12. Seed Payments
            SeedPayments(orders);
            await _context.SaveChangesAsync();
            Console.WriteLine("   ✓ Payments seeded");
        }

        private List<Category> SeedCategories()
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Smartphones",
                    Slug = "smartphones",
                    ParentId = null
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Phone Cases",
                    Slug = "phone-cases",
                    ParentId = null
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Screen Protectors",
                    Slug = "screen-protectors",
                    ParentId = null
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Chargers & Cables",
                    Slug = "chargers-cables",
                    ParentId = null
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Wireless Chargers",
                    Slug = "wireless-chargers",
                    ParentId = null
                }
            };

            _context.Categories.AddRange(categories);
            return categories;
        }

        private List<Brand> SeedBrands()
        {
            var brands = new List<Brand>
            {
                new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "Apple",
                    Description = "Premium Apple accessories",
                    LogoUrl = "https://via.placeholder.com/150?text=Apple"
                },
                new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "Samsung",
                    Description = "Quality Samsung accessories",
                    LogoUrl = "https://via.placeholder.com/150?text=Samsung"
                },
                new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "Anker",
                    Description = "Innovative tech accessories",
                    LogoUrl = "https://via.placeholder.com/150?text=Anker"
                },
                new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = "Belkin",
                    Description = "Professional mobile accessories",
                    LogoUrl = "https://via.placeholder.com/150?text=Belkin"
                }
            };

            _context.Brands.AddRange(brands);
            return brands;
        }

        private List<Device> SeedDevices()
        {
            var devices = new List<Device>
            {
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "iPhone 15",
                    Description = "Latest iPhone model"
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "iPhone 14",
                    Description = "iPhone 14 series"
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "iPhone 13",
                    Description = "iPhone 13 series"
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "Samsung Galaxy S24",
                    Description = "Latest Samsung Galaxy"
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "Samsung Galaxy S23",
                    Description = "Samsung Galaxy S23"
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "iPad Pro",
                    Description = "iPad Pro tablet"
                }
            };

            _context.Devices.AddRange(devices);
            return devices;
        }

        private List<Attributes> SeedAttributes()
        {
            var attributes = new List<Attributes>
            {
                new Attributes
                {
                    Id = Guid.NewGuid(),
                    Name = "Color",
                    DataType = "string"
                },
                new Attributes
                {
                    Id = Guid.NewGuid(),
                    Name = "Material",
                    DataType = "string"
                },
                new Attributes
                {
                    Id = Guid.NewGuid(),
                    Name = "Weight",
                    DataType = "decimal"
                },
                new Attributes
                {
                    Id = Guid.NewGuid(),
                    Name = "Capacity",
                    DataType = "string"
                },
                new Attributes
                {
                    Id = Guid.NewGuid(),
                    Name = "Warranty",
                    DataType = "string"
                }
            };

            _context.Attributes.AddRange(attributes);
            return attributes;
        }

        private List<Account> SeedAccounts()
        {
            var accounts = new List<Account>
            {
                new Account
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@example.com",
                    PhoneNumber = "+1234567890",
                    PasswordHash = _passwordHasher.Hash("admin123"),
                    Role = Role.Admin,
                    IsActive = true
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    Username = "user1",
                    Email = "user1@example.com",
                    PhoneNumber = "+0987654321",
                    PasswordHash = _passwordHasher.Hash("user123"),
                    Role = Role.User,
                    IsActive = true
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    Username = "user2",
                    Email = "user2@example.com",
                    PhoneNumber = "+1122334455",
                    PasswordHash = _passwordHasher.Hash("user123"),
                    Role = Role.User,
                    IsActive = true
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    Username = "user3",
                    Email = "user3@example.com",
                    PhoneNumber = "+5566778899",
                    PasswordHash = _passwordHasher.Hash("user123"),
                    Role = Role.User,
                    IsActive = true
                }
            };

            _context.Accounts.AddRange(accounts);
            return accounts;
        }

        private List<Product> SeedProducts(List<Brand> brands, List<Category> categories)
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Premium iPhone 15 Case",
                    Description = "Durable protective case with shock absorption",
                    Price = 29.99m,
                    IsActive = true,
                    BrandId = brands[0].Id,
                    CategoryId = categories[1].Id
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "iPhone 15 Screen Protector",
                    Description = "Tempered glass screen protector with anti-fingerprint",
                    Price = 14.99m,
                    IsActive = true,
                    BrandId = brands[0].Id,
                    CategoryId = categories[2].Id
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Fast USB-C Charger",
                    Description = "20W USB-C fast charger with safety certification",
                    Price = 24.99m,
                    IsActive = true,
                    BrandId = brands[2].Id,
                    CategoryId = categories[3].Id
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Wireless Charging Pad",
                    Description = "10W wireless charging pad with LED indicator",
                    Price = 34.99m,
                    IsActive = true,
                    BrandId = brands[2].Id,
                    CategoryId = categories[4].Id
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Samsung Galaxy Case",
                    Description = "Slim protective case for Samsung Galaxy phones",
                    Price = 27.99m,
                    IsActive = true,
                    BrandId = brands[1].Id,
                    CategoryId = categories[1].Id
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "USB-C Cable 3m",
                    Description = "High-speed USB-C data and charging cable",
                    Price = 12.99m,
                    IsActive = true,
                    BrandId = brands[3].Id,
                    CategoryId = categories[3].Id
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Power Bank 20000mAh",
                    Description = "Fast charging power bank with dual ports",
                    Price = 44.99m,
                    IsActive = true,
                    BrandId = brands[2].Id,
                    CategoryId = categories[3].Id
                }
            };

            _context.Products.AddRange(products);
            return products;
        }

        private List<ProductVariant> SeedProductVariants(List<Product> products)
        {
            var variants = new List<ProductVariant>();

            // Variants for iPhone Case
            variants.AddRange(new[]
            {
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Sku = "IPHONE-CASE-BLK",
                    Name = "iPhone 15 Case - Black",
                    StockQuantity = 50,
                    Color = "Black",
                    Price = 29.99m,
                    ImageUrl = "https://via.placeholder.com/300x300?text=iPhone+Case+Black"
                },
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Sku = "IPHONE-CASE-BLU",
                    Name = "iPhone 15 Case - Blue",
                    StockQuantity = 35,
                    Color = "Blue",
                    Price = 29.99m,
                    ImageUrl = "https://via.placeholder.com/300x300?text=iPhone+Case+Blue"
                }
            });

            // Variants for Screen Protector
            variants.AddRange(new[]
            {
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Sku = "SCREEN-PROT-2PK",
                    Name = "Screen Protector - 2 Pack",
                    StockQuantity = 100,
                    Price = 14.99m,
                    ImageUrl = "https://via.placeholder.com/300x300?text=Screen+Protector"
                }
            });

            // Variants for USB-C Charger
            variants.AddRange(new[]
            {
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Sku = "CHARGER-20W",
                    Name = "USB-C Charger 20W",
                    StockQuantity = 75,
                    Color = "White",
                    Price = 24.99m,
                    ImageUrl = "https://via.placeholder.com/300x300?text=USB-C+Charger"
                }
            });

            // Variants for Wireless Charger
            variants.AddRange(new[]
            {
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = products[3].Id,
                    Sku = "WIRELESS-10W",
                    Name = "Wireless Charging Pad 10W",
                    StockQuantity = 40,
                    Color = "White",
                    Price = 34.99m,
                    ImageUrl = "https://via.placeholder.com/300x300?text=Wireless+Charger"
                }
            });

            // Variants for Samsung Case
            variants.AddRange(new[]
            {
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = products[4].Id,
                    Sku = "SAMSUNG-CASE-BLK",
                    Name = "Samsung Galaxy Case - Black",
                    StockQuantity = 45,
                    Color = "Black",
                    Price = 27.99m,
                    ImageUrl = "https://via.placeholder.com/300x300?text=Samsung+Case"
                }
            });

            // Variants for USB-C Cable
            variants.AddRange(new[]
            {
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = products[5].Id,
                    Sku = "USBC-CABLE-3M",
                    Name = "USB-C Cable 3m",
                    StockQuantity = 120,
                    Price = 12.99m,
                    ImageUrl = "https://via.placeholder.com/300x300?text=USB-C+Cable"
                }
            });

            // Variants for Power Bank
            variants.AddRange(new[]
            {
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = products[6].Id,
                    Sku = "POWERBANK-20K-BLK",
                    Name = "Power Bank 20000mAh - Black",
                    StockQuantity = 30,
                    Color = "Black",
                    Price = 44.99m,
                    ImageUrl = "https://via.placeholder.com/300x300?text=Power+Bank"
                }
            });

            _context.ProductVariants.AddRange(variants);
            return variants;
        }

        private void SeedProductAttributes(List<Product> products, List<Attributes> attributes)
        {
            var colorAttr = attributes.First(a => a.Name == "Color");
            var materialAttr = attributes.First(a => a.Name == "Material");
            var capacityAttr = attributes.First(a => a.Name == "Capacity");
            var warrantyAttr = attributes.First(a => a.Name == "Warranty");

            var productAttributes = new List<ProductAttribute>
            {
                // iPhone Case attributes
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[0].Id, AttributeId = colorAttr.Id, Value = "Black, Blue, Red" },
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[0].Id, AttributeId = materialAttr.Id, Value = "TPU + Polycarbonate" },
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[0].Id, AttributeId = warrantyAttr.Id, Value = "1 Year" },

                // Screen Protector attributes
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[1].Id, AttributeId = materialAttr.Id, Value = "Tempered Glass" },
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[1].Id, AttributeId = warrantyAttr.Id, Value = "6 Months" },

                // Charger attributes
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[2].Id, AttributeId = capacityAttr.Id, Value = "20W" },
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[2].Id, AttributeId = warrantyAttr.Id, Value = "1 Year" },

                // Wireless Charger attributes
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[3].Id, AttributeId = capacityAttr.Id, Value = "10W" },
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[3].Id, AttributeId = warrantyAttr.Id, Value = "1 Year" },

                // Samsung Case attributes
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[4].Id, AttributeId = materialAttr.Id, Value = "Rubber + Plastic" },
                new ProductAttribute { Id = Guid.NewGuid(), ProductId = products[4].Id, AttributeId = warrantyAttr.Id, Value = "1 Year" }
            };

            _context.ProductAttributes.AddRange(productAttributes);
        }

        private void SeedProductCompatibilities(List<Product> products, List<Device> devices)
        {
            var productCompatibilities = new List<ProductCompatibility>
            {
                // iPhone Case compatibility
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[0].Id, DeviceId = devices[0].Id, Note = "Perfect fit for iPhone 15" },
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[0].Id, DeviceId = devices[1].Id, Note = "Also works with iPhone 14" },

                // Screen Protector compatibility
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[1].Id, DeviceId = devices[0].Id, Note = "Designed for iPhone 15" },

                // Charger compatibility (universal)
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[2].Id, DeviceId = devices[0].Id, Note = "Universal USB-C" },
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[2].Id, DeviceId = devices[3].Id, Note = "Compatible with Samsung" },

                // Wireless Charger (universal)
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[3].Id, DeviceId = devices[0].Id, Note = "Qi Compatible" },
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[3].Id, DeviceId = devices[3].Id, Note = "Qi Compatible" },

                // Samsung Case
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[4].Id, DeviceId = devices[3].Id, Note = "Perfect fit for S24" },
                new ProductCompatibility { Id = Guid.NewGuid(), ProductId = products[4].Id, DeviceId = devices[4].Id, Note = "Compatible with S23" }
            };

            _context.ProductCompatibilities.AddRange(productCompatibilities);
        }

        private List<Order> SeedOrders(List<Account> accounts)
        {
            var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    AccountId = accounts[1].Id,
                    OrderDate = DateTime.UtcNow.AddDays(-5),
                    TotalAmount = 74.97m,
                    Status = "Pending"
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    AccountId = accounts[2].Id,
                    OrderDate = DateTime.UtcNow.AddDays(-3),
                    TotalAmount = 44.99m,
                    Status = "Confirmed"
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    AccountId = accounts[3].Id,
                    OrderDate = DateTime.UtcNow.AddDays(-1),
                    TotalAmount = 57.98m,
                    Status = "Pending"
                }
            };

            _context.Orders.AddRange(orders);
            return orders;
        }

        private void SeedOrderItems(List<Order> orders, List<ProductVariant> variants)
        {
            var orderItems = new List<OrderItem>
            {
                // Order 1 items
                new OrderItem { Id = Guid.NewGuid(), OrderId = orders[0].Id, VariantId = variants[0].Id, Quantity = 2, Price = 29.99m },
                new OrderItem { Id = Guid.NewGuid(), OrderId = orders[0].Id, VariantId = variants[2].Id, Quantity = 1, Price = 14.99m },

                // Order 2 items
                new OrderItem { Id = Guid.NewGuid(), OrderId = orders[1].Id, VariantId = variants[3].Id, Quantity = 1, Price = 44.99m },

                // Order 3 items
                new OrderItem { Id = Guid.NewGuid(), OrderId = orders[2].Id, VariantId = variants[6].Id, Quantity = 1, Price = 44.99m },
                new OrderItem { Id = Guid.NewGuid(), OrderId = orders[2].Id, VariantId = variants[5].Id, Quantity = 1, Price = 12.99m }
            };

            _context.OrderItems.AddRange(orderItems);
        }

        private void SeedPayments(List<Order> orders)
        {
            var payments = new List<Payment>
            {
                new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = orders[0].Id,
                    Amount = 74.97m,
                    Currency = "USD",
                    PaymentMethod = "CreditCard",
                    TransactionCode = "TXN001",
                    Status = "Pending",
                    TransactionRef = DateTime.UtcNow.Ticks.ToString()
                },
                new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = orders[1].Id,
                    Amount = 44.99m,
                    Currency = "USD",
                    PaymentMethod = "PayOS",
                    TransactionCode = "TXN002",
                    Status = "Success",
                    PaidAt = DateTime.UtcNow.AddDays(-3),
                    TransactionRef = DateTime.UtcNow.Ticks.ToString()
                },
                new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = orders[2].Id,
                    Amount = 57.98m,
                    Currency = "USD",
                    PaymentMethod = "PayOS",
                    TransactionCode = "TXN003",
                    Status = "Pending",
                    TransactionRef = DateTime.UtcNow.Ticks.ToString()
                }
            };

            _context.Payments.AddRange(payments);
        }
    }
}
