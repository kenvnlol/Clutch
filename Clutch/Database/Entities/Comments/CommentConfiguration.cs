using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.Comments;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasQueryFilter(comment => !comment.IsDeleted);

        builder.HasKey(c => c.Id).IsClustered();
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        // User → Comment (Author)
        builder.HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Comment → RootComment (e.g., for threading under top-level comment)
        builder.HasOne(c => c.RootComment)
            .WithMany()
            .HasForeignKey(c => c.RootCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comment → ParentComment (reply relationship)
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies) // ← THIS is what prevents EF from guessing
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
