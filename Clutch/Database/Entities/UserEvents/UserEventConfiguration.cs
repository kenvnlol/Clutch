using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.UserEvents;

public class UserEventConfiguration : IEntityTypeConfiguration<UserEvent>
{
    public void Configure(EntityTypeBuilder<UserEvent> builder)
    {
        builder.OwnsOne(e => e.LikeEvent, nav =>
        {
            nav.ToTable("UserEvent_LikeEvent");
        });

        builder.OwnsOne(e => e.CommentEvent, nav =>
        {
            nav.ToTable("UserEvent_CommentEvent");
        });

        builder.OwnsOne(e => e.CommentLikeEvent, nav =>
        {
            nav.ToTable("UserEvent_CommentLikeEvent");
        });

        builder.OwnsOne(e => e.ViewEvent, nav =>
        {
            nav.ToTable("UserEvent_ViewEvent");
        });

        builder.OwnsOne(e => e.ShareEvent, nav =>
        {
            nav.ToTable("UserEvent_ShareEvent");
        });

        builder.OwnsOne(e => e.FollowEvent, nav =>
        {
            nav.ToTable("UserEvent_FollowEvent");
        });

        builder.OwnsOne(e => e.SaveEvent, nav =>
        {
            nav.ToTable("UserEvent_SaveEvent");
        });
    }
}
