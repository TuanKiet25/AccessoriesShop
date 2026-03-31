using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class ChatMessageConfig : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(m => m.RoomId)
                .IsRequired();

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(m => m.UserType)
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .HasDefaultValueSql("NOW()");

            // Sender FK (nullable — null means AI sent this message)
            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(m => m.RoomId);
            builder.HasIndex(m => m.CreatedAt);
        }
    }
}
