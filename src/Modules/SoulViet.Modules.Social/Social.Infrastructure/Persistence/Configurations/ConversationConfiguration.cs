using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ToTable("Conversations", "social", t => 
            {
                t.HasCheckConstraint("CK_Conversation_UserOrder", "\"UserAId\" < \"UserBId\"");
            });
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => new { c.UserAId, c.UserBId })
                .IsUnique();
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("now()");  
            builder.HasOne(c => c.LastReadMessageA)
                .WithMany()
                .HasForeignKey(c => c.LastReadMessageIdA)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.LastReadMessageB)
                .WithMany()
                .HasForeignKey(c => c.LastReadMessageIdB)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
