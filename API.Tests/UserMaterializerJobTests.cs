using Clutch.Database;
using Clutch.Database.Entities;
using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.CommentThreads;
using Clutch.Database.Entities.Games;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Database.Entities.Users;
using Clutch.Database.Entities.UserWallets;
using Clutch.Features.CommentLikes.Services;
using Clutch.Features.CommentLikes.Shared;
using Clutch.Features.CommentThreads.Services;
using Clutch.Features.Follows.Services;
using Clutch.Features.Follows.Shared;
using Clutch.Features.Likes.Services;
using Clutch.Features.Saves.Services;
using Clutch.Features.Saves.Shared;
using Clutch.Features.Views.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Xunit.Abstractions;
namespace API.Tests;

public class UserEventMaterializerJobTests
{
    private readonly ITestOutputHelper _output;

    public UserEventMaterializerJobTests(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact]
    public async Task Execute_Using_Manual_ServiceCollectionScopeFactory()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ClutchTestDb;Trusted_Connection=True;")
        .Options;

        using var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        await Seed(dbContext);
        await Insert(dbContext);
        //run again to ensure idempotence.
        await Insert(dbContext);
        await InsertReplyAndCommentLike(dbContext);
        await UndoInserts(dbContext);
        //// Second undo verifies that undo is idempotent and safe even if the state is already empty
        await UndoInserts(dbContext);
        await Insert(dbContext);

    }

    private void InvokeHandlers(ApplicationDbContext dbContext)
    {
        static void Run<THandler, TRequest>(TRequest request, ApplicationDbContext db)
            where THandler : class
            where TRequest : class
        {
            var method = typeof(THandler)
                .GetMethod("HandleAsync", BindingFlags.NonPublic | BindingFlags.Static)!;

            var task = (ValueTask)method.Invoke(null, new object[] { request, db, CancellationToken.None })!;
            task.GetAwaiter().GetResult();
        }

        Run<MaterializeLikes, MaterializeLikes.Request>(new(), dbContext);
        Run<MaterializeCommentLikes, MaterializeCommentLikes.Request>(new(), dbContext);
        Run<MaterializeComments, MaterializeComments.Request>(new(), dbContext);
        Run<MaterializeSaves, MaterializeSaves.Request>(new(), dbContext);
        Run<MaterializeViews, MaterializeViews.Request>(new(), dbContext);
        Run<MaterializeFollows, MaterializeFollows.Request>(new(), dbContext);
    }


    private async Task Insert(
        ApplicationDbContext dbContext)
    {
        var userClips = await dbContext.Clips
            .Where(c => c.AuthorId == "user1" || c.AuthorId == "user2" || c.AuthorId == "user3")
            .ToDictionaryAsync(c => c.AuthorId, c => c.Id);

        var user1Clip = userClips["user1"];
        var user2Clip = userClips["user2"];
        var user3Clip = userClips["user3"];

        // user 1 likes clip 2
        var user1LikesClip2Event = TestUtilities.CreateLikeEvent("user1", user2Clip, LikeType.Like);
        dbContext.UserEvents.Add(user1LikesClip2Event);
        await dbContext.SaveChangesAsync();

        var user1CommentsClip2Event = TestUtilities.CreateCommentEvent(1, null, null, user2Clip, "user1", "Great clip bro!!", CommentType.Post); // Top level comment. Has no parent or root.
        dbContext.UserEvents.Add(user1CommentsClip2Event);
        await dbContext.SaveChangesAsync();

        var user1SavesClip2Event = TestUtilities.CreateSaveEvent("user1", user2Clip, SaveType.Save);
        dbContext.UserEvents.Add(user1SavesClip2Event);
        await dbContext.SaveChangesAsync();

        var user1ViewsClip2 = TestUtilities.CreateViewEvent(user2Clip, "user1");
        dbContext.UserEvents.Add(user1ViewsClip2);
        await dbContext.SaveChangesAsync();

        var anonViewsClip2 = TestUtilities.CreateViewEvent(user2Clip, deviceId: "greatest-pc-in-the-world");
        dbContext.UserEvents.Add(anonViewsClip2);
        await dbContext.SaveChangesAsync();

        var user1FollowsUser2 = TestUtilities.CreateFollowEvent("user1", "user2", FollowType.Follow);
        dbContext.UserEvents.Add(user1FollowsUser2);
        await dbContext.SaveChangesAsync();

        var user2FollowsUser1 = TestUtilities.CreateFollowEvent("user2", "user1", FollowType.Follow);
        dbContext.UserEvents.Add(user2FollowsUser1);
        await dbContext.SaveChangesAsync();

        InvokeHandlers(dbContext);


        var likes = await dbContext.Likes.ToListAsync();
        likes.Should().HaveCount(1);
        likes[0].AuthorId.Should().Be("user1");
        likes[0].ClipId.Should().Be(user2Clip);

        var comments = await dbContext.Comments.ToListAsync();
        comments.Should().HaveCount(1);
        comments.Any(c => c.AuthorId == "user1" && c.Text == "Great clip bro!!").Should().BeTrue();

        var follows = await dbContext.Follows.ToListAsync();
        follows.Should().HaveCount(2);
        follows.Should().ContainSingle(f => f.InitiatorUserId == "user1" && f.TargetUserId == "user2");
        follows.Should().ContainSingle(f => f.InitiatorUserId == "user2" && f.TargetUserId == "user1");

        var views = await dbContext.Views.ToListAsync();
        views.Should().HaveCount(2);
        views.Should().Contain(v => v.AuthorId == "user1");
        views.Should().Contain(v => v.DeviceId == "greatest-pc-in-the-world");

        var user1Inbox = await dbContext.UserInboxes.SingleAsync(x => x.UserId == "user1");
        var user2Inbox = await dbContext.UserInboxes.SingleAsync(x => x.UserId == "user2");

        user2Inbox.UnseenClipLikeCount.Should().Be(1);
        user2Inbox.UnseenCommentClipCount.Should().Be(1);
        user2Inbox.UnseenUserFollowCount.Should().Be(1);

        user1Inbox.UnseenUserFollowCount.Should().Be(1);


        var user2Activities = await dbContext.InboxActivities
        .Where(x => x.UserInboxId == "user2")
        .ToListAsync();

        user2Activities.Should().Contain(x => x.Type == InboxNotificationType.ClipLike);
        user2Activities.Should().Contain(x => x.Type == InboxNotificationType.CommentClip);
        user2Activities.Should().Contain(x => x.Type == InboxNotificationType.UserFollow);

        // same for user1
        var user1Activities = await dbContext.InboxActivities
            .Where(x => x.UserInboxId == "user1")
            .ToListAsync();

        user1Activities.Should().Contain(x => x.Type == InboxNotificationType.UserFollow);

        var eventOffsets = await dbContext.EventConsumerOffsets.ToListAsync();

        // Make sure each event's last processed ID matches the last event ID for each type
        eventOffsets.Should().ContainSingle(e => e.ConsumerGroup == EventConsumerGroup.LikeEntity && e.LastProcessedEventId == user1LikesClip2Event.Id);
        eventOffsets.Should().ContainSingle(e => e.ConsumerGroup == EventConsumerGroup.CommentEntity && e.LastProcessedEventId == user1CommentsClip2Event.Id);
        eventOffsets.Should().ContainSingle(e => e.ConsumerGroup == EventConsumerGroup.SaveEntity && e.LastProcessedEventId == user1SavesClip2Event.Id);
        eventOffsets.Should().ContainSingle(e => e.ConsumerGroup == EventConsumerGroup.ViewEntity && e.LastProcessedEventId == anonViewsClip2.Id);
        eventOffsets.Should().ContainSingle(e => e.ConsumerGroup == EventConsumerGroup.FollowEntity && e.LastProcessedEventId == user2FollowsUser1.Id);
    }

    private async Task InsertReplyAndCommentLike(
        ApplicationDbContext dbContext)
    {

        var user2LikesUser1Comment = TestUtilities.CreateCommentLikeEvent(1, "user2", LikeType.Like);
        dbContext.UserEvents.Add(user2LikesUser1Comment);
        await dbContext.SaveChangesAsync();

        var user2RespondsToUser1Comment = TestUtilities.CreateCommentEvent(2, 1, 1, 2, "user2", "Thanks bro", CommentType.Post); // Reply, has parent and root same. 
        dbContext.UserEvents.Add(user2RespondsToUser1Comment);
        await dbContext.SaveChangesAsync();

        InvokeHandlers(dbContext);

        var comments = await dbContext.Comments.ToListAsync();
        comments.Should().HaveCount(2);

        comments.Should().ContainSingle(c =>
            c.AuthorId == "user1" &&
            c.Text == "Great clip bro!!" &&
            c.ParentCommentId == null);

        comments.Should().ContainSingle(c =>
            c.AuthorId == "user2" &&
            c.Text == "Thanks bro" &&
            c.ParentCommentId == 1); // or use the actual ID if tracked


        var commentLikes = await dbContext.CommentLikes.ToListAsync();
        commentLikes.Should().HaveCount(1);

        commentLikes[0].InitiatorUserId.Should().Be("user2");
        commentLikes[0].CommentId.Should().Be(1);

        var eventOffsets = await dbContext.EventConsumerOffsets.ToListAsync();

        eventOffsets.Should().ContainSingle(e =>
            e.ConsumerGroup == EventConsumerGroup.CommentEntity &&
            e.LastProcessedEventId == user2RespondsToUser1Comment.Id);

        eventOffsets.Should().ContainSingle(e =>
            e.ConsumerGroup == EventConsumerGroup.CommentLikeEntity &&
            e.LastProcessedEventId == user2LikesUser1Comment.Id);
    }

    private async Task UndoInserts(
        ApplicationDbContext dbContext)
    {
        var userClips = await dbContext.Clips
         .Where(c => c.AuthorId == "user1" || c.AuthorId == "user2" || c.AuthorId == "user3")
         .ToDictionaryAsync(c => c.AuthorId, c => c.Id);

        var user1Clip = userClips["user1"];
        var user2Clip = userClips["user2"];
        var user3Clip = userClips["user3"];

        // user 1 likes clip 2
        var user1LikesClip2Event = TestUtilities.CreateLikeEvent("user1", user2Clip, LikeType.Unlike);
        dbContext.UserEvents.Add(user1LikesClip2Event);
        await dbContext.SaveChangesAsync();

        var user1CommentsClip2Event = TestUtilities.CreateCommentEvent(1, null, null, user2Clip, "user1", "Great clip bro!!", CommentType.Undo); // Top level comment. Has no parent or root.
        dbContext.UserEvents.Add(user1CommentsClip2Event);
        await dbContext.SaveChangesAsync();

        var user1SavesClip2Event = TestUtilities.CreateSaveEvent("user1", user2Clip, SaveType.Unsave);
        dbContext.UserEvents.Add(user1SavesClip2Event);
        await dbContext.SaveChangesAsync();


        var user1FollowsUser2 = TestUtilities.CreateFollowEvent("user1", "user2", FollowType.Unfollow);
        dbContext.UserEvents.Add(user1FollowsUser2);
        await dbContext.SaveChangesAsync();

        var user2FollowsUser1 = TestUtilities.CreateFollowEvent("user2", "user1", FollowType.Unfollow);
        dbContext.UserEvents.Add(user2FollowsUser1);
        await dbContext.SaveChangesAsync();


        var user2LikesUser1Comment = TestUtilities.CreateCommentLikeEvent(1, "user2", LikeType.Unlike);
        dbContext.UserEvents.Add(user2LikesUser1Comment);
        await dbContext.SaveChangesAsync();

        var user2RespondsToUser1Comment = TestUtilities.CreateCommentEvent(2, 1, 1, 2, "user2", "Thanks bro", CommentType.Undo);
        dbContext.UserEvents.Add(user2RespondsToUser1Comment);
        await dbContext.SaveChangesAsync();

        InvokeHandlers(dbContext);


        (await dbContext.Likes.AnyAsync()).Should().BeFalse();
        (await dbContext.Saves.AnyAsync()).Should().BeFalse();
        (await dbContext.Follows.AnyAsync()).Should().BeFalse();
        (await dbContext.Comments.AnyAsync()).Should().BeFalse();
        (await dbContext.CommentLikes.AnyAsync()).Should().BeFalse();

        var userInboxes = await dbContext.UserInboxes
            .AsSplitQuery()
            .Include(x => x.Activities)
            .Where(y => y.UserId == "user1" || y.UserId == "user2")
            .ToListAsync();


        var user1Inbox = userInboxes.First(i => i.UserId == "user1");
        user1Inbox.UnseenClipLikeCount.Should().Be(0);
        user1Inbox.UnseenCommentClipCount.Should().Be(0);
        user1Inbox.UnseenUserFollowCount.Should().Be(0);
        user1Inbox.UnseenCommentLikeCount.Should().Be(0);
        user1Inbox.UnseenCommentReplyCount.Should().Be(0);

        var user2Inbox = userInboxes.First(i => i.UserId == "user2");
        user2Inbox.UnseenClipLikeCount.Should().Be(0);
        user2Inbox.UnseenCommentClipCount.Should().Be(0);
        user2Inbox.UnseenUserFollowCount.Should().Be(0);
        user2Inbox.UnseenCommentLikeCount.Should().Be(0);
        user2Inbox.UnseenCommentReplyCount.Should().Be(0);

        user1Inbox.Activities.Should().BeEmpty();
        user2Inbox.Activities.Should().BeEmpty();

        // ✅ **Assertions: Offsets are Updated**
        var eventOffsets = await dbContext.EventConsumerOffsets.ToListAsync();

        eventOffsets.Should().ContainSingle(e =>
            e.ConsumerGroup == EventConsumerGroup.LikeEntity &&
            e.LastProcessedEventId == user1LikesClip2Event.Id);

        eventOffsets.Should().ContainSingle(e =>
            e.ConsumerGroup == EventConsumerGroup.CommentEntity &&
            e.LastProcessedEventId == user2RespondsToUser1Comment.Id);

        eventOffsets.Should().ContainSingle(e =>
            e.ConsumerGroup == EventConsumerGroup.SaveEntity &&
            e.LastProcessedEventId == user1SavesClip2Event.Id);

        eventOffsets.Should().ContainSingle(e =>
            e.ConsumerGroup == EventConsumerGroup.FollowEntity &&
            e.LastProcessedEventId == user2FollowsUser1.Id);

        eventOffsets.Should().ContainSingle(e =>
            e.ConsumerGroup == EventConsumerGroup.CommentLikeEntity &&
            e.LastProcessedEventId == user2LikesUser1Comment.Id);
    }

    private async Task Seed(ApplicationDbContext dbContext)
    {
        var user1 = new User
        {
            Id = "user1",
            UserName = "user1",
            Email = "gamer123@example.com",
            DisplayName = "Gamer123",
            FirstName = "Gamer",
            LastName = "OneTwoThree",
            CreatedAt = DateTimeOffset.UtcNow,
            LastLoginDate = DateTimeOffset.UtcNow,
            IsLoggedIn = true,
            AvatarUri = "https://cdn.example.com/avatars/user-123.jpg",
            ProfileBio = "Just a gamer.",
            FollowerCount = 0,
            FollowingCount = 0,
            Wallet = new UserWallet
            {
                WithdrawalRequests = []
            },
            Clips = [],
            Comments = [],
            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            DirectThreads = [],
            FollowsMade = [],
            FollowsReceived = [],
            MentionsMade = [],
            MentionsReceived = [],
            CommentsLiked = [],
            CommentLikesReceived = [],
            InitiatedActivities = [],
            UserInbox = new UserInbox
            {
                Activities = [],
                UnseenCommentLikeCount = 0,
                UnseenCommentReplyCount = 0,
                UnseenCommentClipCount = 0,
                UnseenUserMentionCount = 0,
                UnseenUserFollowCount = 0,
                UnseenClipLikeCount = 0,
                UnseenDirectMessageCount = 0
            }
        };

        var user2 = new User
        {
            Id = "user2",
            UserName = "user2",
            Email = "gamer123@example.com",
            DisplayName = "Gamer123",
            FirstName = "Gamer",
            LastName = "OneTwoThree",
            CreatedAt = DateTimeOffset.UtcNow,
            LastLoginDate = DateTimeOffset.UtcNow,
            IsLoggedIn = true,
            AvatarUri = "https://cdn.example.com/avatars/user-123.jpg",
            ProfileBio = "Just a gamer.",
            FollowerCount = 0,
            FollowingCount = 0,
            Wallet = new UserWallet
            {
                WithdrawalRequests = []
            },
            Clips = new List<Clip>(),
            Comments = [],
            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            DirectThreads = [],
            FollowsMade = [],
            FollowsReceived = [],
            MentionsMade = [],
            MentionsReceived = [],
            CommentsLiked = [],
            CommentLikesReceived = [],
            InitiatedActivities = [],
            UserInbox = new UserInbox
            {
                Activities = [],
                UnseenCommentLikeCount = 0,
                UnseenCommentReplyCount = 0,
                UnseenCommentClipCount = 0,
                UnseenUserMentionCount = 0,
                UnseenUserFollowCount = 0,
                UnseenClipLikeCount = 0,
                UnseenDirectMessageCount = 0
            }
        };


        var user3 = new User
        {
            Id = "user3",
            UserName = "user3",
            Email = "gamer123@example.com",
            DisplayName = "Gamer123",
            FirstName = "Gamer",
            LastName = "OneTwoThree",
            CreatedAt = DateTimeOffset.UtcNow,
            LastLoginDate = DateTimeOffset.UtcNow,
            IsLoggedIn = true,
            AvatarUri = "https://cdn.example.com/avatars/user-123.jpg",
            ProfileBio = "Just a gamer.",
            FollowerCount = 0,
            FollowingCount = 0,
            Wallet = new UserWallet
            {
                WithdrawalRequests = []
            },
            Clips = new List<Clip>(),
            Comments = [],
            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            DirectThreads = [],
            FollowsMade = [],
            FollowsReceived = [],
            MentionsMade = [],
            MentionsReceived = [],
            CommentsLiked = [],
            CommentLikesReceived = [],
            InitiatedActivities = [],
            UserInbox = new UserInbox
            {
                Activities = [],
                UnseenCommentLikeCount = 0,
                UnseenCommentReplyCount = 0,
                UnseenCommentClipCount = 0,
                UnseenUserMentionCount = 0,
                UnseenUserFollowCount = 0,
                UnseenClipLikeCount = 0,
                UnseenDirectMessageCount = 0
            }
        };

        var users = new[] { user1, user2, user3 };
        dbContext.Users.AddRange(users);

        user1.Clips.Add(new Clip
        {
            GameId = 1, // Placeholder ID
            Game = new Game
            {
                Title = "LOL",
                Clips = []
            },
            AuthorId = "user1",
            Author = user1,
            Media = new ClipMedia
            {
                Avatar = new BlobReference
                {
                    BlobName = "",
                    ContainerName = "",
                },
                OriginalUpload = new BlobReference
                {
                    BlobName = "",
                    ContainerName = "",
                }
            },
            Description = "Check out this insane clutch!",
            DurationInSeconds = 15,
            Timestamp = DateTimeOffset.UtcNow,
            LastCounterUpdate = DateTimeOffset.UtcNow,
            CommentThread = new CommentThread { Comments = [] },
            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            ContestWins = []
        });

        user2.Clips.Add(new Clip
        {
            GameId = 1, // Placeholder ID
            Game = new Game
            {
                Title = "LOL",
                Clips = []
            },
            AuthorId = "user2",
            Author = user2,
            Media = new ClipMedia
            {
                Avatar = new BlobReference
                {
                    BlobName = "",
                    ContainerName = "",
                },
                OriginalUpload = new BlobReference
                {
                    BlobName = "",
                    ContainerName = "",
                }
            },
            Description = "Check out this insane clutch!",
            DurationInSeconds = 15,
            Timestamp = DateTimeOffset.UtcNow,
            LastCounterUpdate = DateTimeOffset.UtcNow,
            CommentThread = new CommentThread { Comments = [] },
            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            ContestWins = []
        });

        user3.Clips.Add(new Clip
        {
            GameId = 1, // Placeholder ID
            Game = new Game
            {
                Title = "LOL",
                Clips = []
            },
            AuthorId = "user3",
            Author = user3,
            Media = new ClipMedia
            {
                Avatar = new BlobReference
                {
                    BlobName = "",
                    ContainerName = "",
                },
                OriginalUpload = new BlobReference
                {
                    BlobName = "",
                    ContainerName = "",
                }
            },
            Description = "Check out this insane clutch!",
            DurationInSeconds = 15,
            Timestamp = DateTimeOffset.UtcNow,
            LastCounterUpdate = DateTimeOffset.UtcNow,
            CommentThread = new CommentThread { Comments = [] },
            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            ContestWins = []
        });

        var existingGroups = dbContext.EventConsumerOffsets
                 .Select(e => e.ConsumerGroup)
                 .ToHashSet();

        var missingOffsets = Enum.GetValues<EventConsumerGroup>()
            .Where(group => !existingGroups.Contains(group))
            .Select(group => new EventConsumerOffset
            {
                ConsumerGroup = group,
                LastProcessedEventId = 0
            })
            .ToList();

        if (missingOffsets.Count > 0)
        {
            dbContext.EventConsumerOffsets.AddRange(missingOffsets);
            dbContext.SaveChanges();
        }

        await dbContext.SaveChangesAsync();
    }
}

/*
 
- Duplicate events (e.g., same like twice → compact to one)	❌ -- 
- Deletion events (like unlikes, unfollows, unsaves)	❌ -- 
- Comment replies and comment likes (you postponed those)	❌ --
- Edge cases (missing users, invalid clips, or unauthorized)	❌ 
- Idempotency of repeated job runs	❌ --
- Inbox activity deletion when things are unliked/unfollowed	❌  --



i will now run method 1 agian, basically, and assert that it still looks like in the first method 1.

Then, the next method will do stuff like followback etc

And then the next bathc will essentially ensure that "undos" work for all.

At this point, the db is empty, and we can safely try no oops (so inserting undos again)

 */