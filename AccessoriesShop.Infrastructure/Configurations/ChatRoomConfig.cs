using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class ChatRoomConfig : IEntityTypeConfiguration<ChatRoom>
    {
        public void Configure(EntityTypeBuilder<ChatRoom> builder)
        {
            builder.ToTable("ChatRooms");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(r => r.Name)
                .HasMaxLength(200);

            builder.Property(r => r.CustomerId)
                .IsRequired();

            builder.Property(r => r.Status)
                .IsRequired()
                .HasDefaultValue(Domain.Enums.ChatRoomStatus.Waiting);

            builder.Property(r => r.IsAIEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(r => r.CreatedAt)
                .HasDefaultValueSql("NOW()");

            // Customer FK
            builder.HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ActiveStaff FK (optional)
            builder.HasOne(r => r.ActiveStaff)
                .WithMany()
                .HasForeignKey(r => r.ActiveStaffId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(r => r.Messages)
                .WithOne(m => m.Room)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
