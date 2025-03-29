using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Domain.Enums;

namespace Infraestructure.Data
{
    public static class SeedDatabase
    {
        public static void Initialize(AppDbContext context)
        {
            var colombiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, colombiaTimeZone)
                                  .AddTicks(-(DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond));

            if (!context.Wallets.Any())
            {
                context.Wallets.AddRange(
                    new Wallet
                    {
                        Id = 1,
                        DocumentId = "123456789",
                        Name = "Alice",
                        Balance = 1000m,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Wallet
                    {
                        Id = 2,
                        DocumentId = "987654321",
                        Name = "Bob",
                        Balance = 500m,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                );

                context.SaveChanges();
            }

            if (!context.MovementsHistory.Any())
            {
                context.MovementsHistory.AddRange(
                    new MovementHistory
                    {
                        WalletId = 1,
                        Amount = 100m,
                        Type = MovementType.Credit,
                        CreatedAt = now
                    },
                    new MovementHistory
                    {
                        WalletId = 1,
                        Amount = 50m,
                        Type = MovementType.Debit,
                        CreatedAt = now
                    },
                    new MovementHistory
                    {
                        WalletId = 2,
                        Amount = 200m,
                        Type = MovementType.Credit,
                        CreatedAt = now
                    }
                );

                context.SaveChanges();
            }

            var hasher = new PasswordHasher<string>();
            var passwordHash = hasher.HashPassword("admin", "secret123");

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = passwordHash,
                    Role = "Admin"
                });
                context.SaveChanges();
            }
        }
    }
}