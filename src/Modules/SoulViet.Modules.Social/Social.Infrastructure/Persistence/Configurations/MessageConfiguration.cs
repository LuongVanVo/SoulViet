using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages", "social");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content).IsRequired(false);
            builder.Property(m => m.MediaUrl).IsRequired(false);
            builder.Property(m => m.CreatedAt).HasDefaultValueSql("now()");

            builder.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => new { m.ConversationId, m.CreatedAt }).IsDescending(false, true);
            builder.HasIndex(m => new { m.ConversationId, m.ClientTempId }).IsUnique();
        }
    }
}
