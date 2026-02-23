using Clutch.Database.Entities;
using Clutch.Database.Entities.Blobs;
using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.CommentThreads;
using Clutch.Database.Entities.Contests;
using Clutch.Database.Entities.ContestWinners;
using Clutch.Database.Entities.DirectMessages;
using Clutch.Database.Entities.DirectThreads;
using Clutch.Database.Entities.Follows;
using Clutch.Database.Entities.Games;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Mentions;
using Clutch.Database.Entities.Saves;
using Clutch.Database.Entities.Shares;
using Clutch.Database.Entities.StagingClips;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Database.Entities.Users;
using Clutch.Database.Entities.Views;
using Clutch.Database.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Clutch.Database;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Clip> Clips { get; set; }
    public DbSet<CommentThread> CommentThreads { get; set; }
    public DbSet<Entities.Comments.Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<Blob> Blobs { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<View> Views { get; set; }
    public DbSet<Entities.Likes.Like> Likes { get; set; }
    public DbSet<Share> Shares { get; set; }
    public DbSet<Save> Saves { get; set; }
    public DbSet<InboxActivity> InboxActivities { get; set; }
    public DbSet<DirectThread> DirectThreads { get; set; }
    public DbSet<UserInbox> UserInboxes { get; set; }
    public DbSet<DirectMessage> DirectMessages { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Mention> Mentions { get; set; }
    public DbSet<ContestWinner> ContestWinners { get; set; }
    public DbSet<Contest> Contests { get; set; }
    public DbSet<UserEvent> UserEvents { get; set; }
    public DbSet<EventConsumerOffset> EventConsumerOffsets { get; set; }
    public DbSet<StagingClip> StagingClips { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        // if Clip is deleted, its CommentThread is deleted.
        modelBuilder.Entity<CommentThread>()
            .HasOne(ct => ct.Clip)
            .WithOne(c => c.CommentThread)
            .OnDelete(DeleteBehavior.Cascade);

        // if CommentThread is deleted, all its Comments are deleted.
        modelBuilder.Entity<Entities.Comments.Comment>()
            .HasOne(c => c.CommentThread)
            .WithMany(ct => ct.Comments)
            .HasForeignKey(c => c.CommentThreadId)
            .OnDelete(DeleteBehavior.Cascade);

        // self-referencing relationships should NOT cascade (to prevent infinite loops)
        modelBuilder.Entity<Entities.Comments.Comment>()
            .HasOne(c => c.RootComment)
            .WithMany()
            .HasForeignKey(c => c.RootCommentId)
            .OnDelete(DeleteBehavior.Restrict); // prevent infinite recursion

        modelBuilder.Entity<Entities.Comments.Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany()
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict); // prevent infinite recursion

        modelBuilder.Entity<Entities.Likes.Like>()
            .HasIndex(l => new { l.AuthorId, l.ClipId });

        modelBuilder.Entity<Save>()
            .HasIndex(s => new { s.AuthorId, s.ClipId });

        modelBuilder.Entity<Share>()
            .HasIndex(s => new { s.AuthorId, s.ClipId });

        modelBuilder.Entity<View>()
            .HasIndex(v => new { v.AuthorId, v.ClipId });

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.InitiatorUser)
            .WithMany(u => u.FollowsReceived)
            .HasForeignKey(f => f.InitiatorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.TargetUser)
            .WithMany(u => u.FollowsMade)
            .HasForeignKey(f => f.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CommentLike>()
            .HasOne(f => f.InitiatorUser)
            .WithMany(u => u.CommentLikesReceived)
            .HasForeignKey(f => f.InitiatorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Mention>()
            .HasOne(f => f.InitiatorUser)
            .WithMany(u => u.MentionsMade) // User's followings
            .HasForeignKey(f => f.InitiatorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Mention>()
            .HasOne(f => f.TargetUser)
            .WithMany(u => u.MentionsReceived) // User's followers
            .HasForeignKey(f => f.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Entities.Likes.Like>()
            .HasOne(l => l.Clip)
            .WithMany(c => c.Likes)
            .HasForeignKey(l => l.ClipId)
            .OnDelete(DeleteBehavior.NoAction); // Automatically delete likes when clip is deleted

        modelBuilder.Entity<Entities.Likes.Like>()
            .HasOne(l => l.Author)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.AuthorId)
            .OnDelete(DeleteBehavior.NoAction); // Prevent multiple cascading paths


        modelBuilder.Entity<Save>()
            .HasOne(l => l.Clip)
            .WithMany(c => c.Saves)
            .HasForeignKey(l => l.ClipId)
            .OnDelete(DeleteBehavior.NoAction); // Automatically delete likes when clip is deleted

        modelBuilder.Entity<Save>()
            .HasOne(l => l.Author)
            .WithMany(u => u.Saves)
            .HasForeignKey(l => l.AuthorId)
            .OnDelete(DeleteBehavior.NoAction); // Prevent multiple cascading paths


        modelBuilder.Entity<Share>()
            .HasOne(l => l.Clip)
            .WithMany(c => c.Shares)
            .HasForeignKey(l => l.ClipId)
            .OnDelete(DeleteBehavior.NoAction); // Automatically delete likes when clip is deleted

        modelBuilder.Entity<Share>()
            .HasOne(l => l.Author)
            .WithMany(u => u.Shares)
            .HasForeignKey(l => l.AuthorId)
            .OnDelete(DeleteBehavior.NoAction); // Prevent multiple cascading paths

        modelBuilder.Entity<View>()
            .HasOne(l => l.Clip)
            .WithMany(c => c.Views)
            .HasForeignKey(l => l.ClipId)
            .OnDelete(DeleteBehavior.NoAction); // Automatically delete likes when clip is deleted

        modelBuilder.Entity<View>()
            .HasOne(l => l.Author)
            .WithMany(u => u.Views)
            .HasForeignKey(l => l.AuthorId)
            .OnDelete(DeleteBehavior.NoAction); // Prevent multiple cascading paths

        modelBuilder.Entity<UserInbox>()
            .HasMany(ui => ui.Activities)
            .WithOne(ia => ia.UserInbox)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.ApplySoftDeleteQueryFilter();
    }
}










