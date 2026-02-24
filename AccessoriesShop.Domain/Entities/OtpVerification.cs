using System;

namespace AccessoriesShop.Domain.Entities
{
    public class OtpVerification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AccountId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Account? Account { get; set; }
    }
}
