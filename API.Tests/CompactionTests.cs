using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.UserEvents;
using Clutch.Features.CommentLikes.Shared;
using Clutch.Features.Follows.Shared;
using Clutch.Features.Saves.Shared;
using Clutch.Features.Shared.Extensions;
using Clutch.Features.Shared.Interfaces;
using FluentAssertions;

namespace API.Tests;

public class CompactionTests
{
    [Fact]
    public void CompactBy_LikeEvent_GroupsCorrectly()
    {
        var user1 = "123";
        var user2 = "345";
        var clipId = 456;

        var e1 = TestUtilities.CreateUserEventWithId(1, new LikeEventData(
                    AuthorId: user1,
                    ClipId: clipId,
                    Timestamp: DateTimeOffset.UtcNow,
                    Platform: "Web",
                    DevicePlatform: "Windows",
                    DeviceId: "test-device-1",
                    BrowserLanguage: "en-US",
                    UserIsLoggedIn: true,
                    Type: LikeType.Like
                ),
                EventActionType.Create);

        var e2 = TestUtilities.CreateUserEventWithId(2, new LikeEventData(
                    AuthorId: user1,
                    ClipId: clipId,
                    Timestamp: DateTimeOffset.UtcNow,
                    Platform: "Web",
                    DevicePlatform: "Windows",
                    DeviceId: "test-device-2",
                    BrowserLanguage: "en-US",
                    UserIsLoggedIn: true,
                    Type: LikeType.Like
                ),
                EventActionType.Create);


        var e3 = TestUtilities.CreateUserEventWithId(3, new LikeEventData(
                    AuthorId: user1,
                    ClipId: clipId,
                    Timestamp: DateTimeOffset.UtcNow,
                    Platform: "Web",
                    DevicePlatform: "Windows",
                    DeviceId: "test-device-2",
                    BrowserLanguage: "en-US",
                    UserIsLoggedIn: true,
                    Type: LikeType.Unlike
                ),
                EventActionType.Delete);


        var e4 = TestUtilities.CreateUserEventWithId(4, new LikeEventData(
                    AuthorId: user2,
                    ClipId: clipId,
                    Timestamp: DateTimeOffset.UtcNow,
                    Platform: "Web",
                    DevicePlatform: "Windows",
                    DeviceId: "test-device-2",
                    BrowserLanguage: "en-US",
                    UserIsLoggedIn: true,
                    Type: LikeType.Like
                    ),
                EventActionType.Create);


        var e5 = TestUtilities.CreateUserEventWithId(5, new LikeEventData(
                    AuthorId: user2,
                    ClipId: 456,
                    Timestamp: DateTimeOffset.UtcNow,
                    Platform: "Web",
                    DevicePlatform: "Windows",
                    DeviceId: "test-device-2",
                    BrowserLanguage: "en-US",
                    UserIsLoggedIn: true,
                    Type: LikeType.Like),
                EventActionType.Create);

        var result = new List<UserEvent> { e1, e2, e3, e4, e5 }.CompactBy(ev => ev.LikeEvent);

        result.Should().HaveCount(2);

        // Result for "123:456"
        var first = result.Single(r => r.Data.AuthorId == user1);
        first.EventId.Should().Be(3);
        first.Data.Type.Should().Be(LikeType.Unlike);
        first.TotalCount.Should().Be(3);

        // Result for "345:456"
        var second = result.Single(r => r.Data.AuthorId == user2);
        second.EventId.Should().Be(5);
        second.Data.Type.Should().Be(LikeType.Like);
        second.TotalCount.Should().Be(2);
    }

    [Fact]
    public void CompactBy_CommentEvent_GroupsCorrectly()
    {
        var author1 = "user1";
        var commentId1 = 99;

        var author2 = "user2";
        var commentId2 = 100;

        var e1 = TestUtilities.CreateUserEventWithId(1, new CommentEventData(commentId1, 1, 1, 1, author1, "Hello", "", "", "", "", true, DateTimeOffset.UtcNow, CommentType.Post), EventActionType.Create);
        var e2 = TestUtilities.CreateUserEventWithId(2, new CommentEventData(commentId1, 1, 1, 1, author1, "Hello Again", "", "", "", "", true, DateTimeOffset.UtcNow, CommentType.Post), EventActionType.Create);

        var e3 = TestUtilities.CreateUserEventWithId(3, new CommentEventData(commentId2, 1, 1, 1, author2, "Hi", "", "", "", "", true, DateTimeOffset.UtcNow, CommentType.Post), EventActionType.Create);
        var e4 = TestUtilities.CreateUserEventWithId(4, new CommentEventData(commentId2, 1, 1, 1, author2, "Bye", "", "", "", "", true, DateTimeOffset.UtcNow, CommentType.Delete), EventActionType.Delete);

        var events = new List<UserEvent> { e1, e2, e3, e4 };

        var result = events.CompactBy(ev => ev.CommentEvent);

        result.Should().HaveCount(2);

        var first = result.Single(r => r.Data.AuthorId == author1);
        first.EventId.Should().Be(2);
        first.TotalCount.Should().Be(2);
        first.Data.Type.Should().Be(CommentType.Post);
        first.Data.Text.Should().Be("Hello Again");

        var second = result.Single(r => r.Data.AuthorId == author2);
        second.EventId.Should().Be(4);
        second.TotalCount.Should().Be(2);
        second.Data.Type.Should().Be(CommentType.Delete);
        second.Data.Text.Should().Be("Bye");
    }


    [Fact]
    public void CompactBy_CommentLike_GroupsCorrectly()
    {
        var user1 = "user1";
        var commentId1 = 99;

        var user2 = "user2";
        var commentId2 = 100;

        var e1 = TestUtilities.CreateUserEventWithId(1, new CommentLikeEventData(commentId1, user1, DateTimeOffset.UtcNow, LikeType.Like), EventActionType.Create);
        var e2 = TestUtilities.CreateUserEventWithId(2, new CommentLikeEventData(commentId1, user1, DateTimeOffset.UtcNow, LikeType.Like), EventActionType.Create);

        var e3 = TestUtilities.CreateUserEventWithId(3, new CommentLikeEventData(commentId2, user2, DateTimeOffset.UtcNow, LikeType.Like), EventActionType.Create);
        var e4 = TestUtilities.CreateUserEventWithId(4, new CommentLikeEventData(commentId2, user2, DateTimeOffset.UtcNow, LikeType.Like), EventActionType.Create);
        var e5 = TestUtilities.CreateUserEventWithId(5, new CommentLikeEventData(commentId2, user2, DateTimeOffset.UtcNow, LikeType.Unlike), EventActionType.Delete);

        var events = new List<UserEvent> { e1, e2, e3, e4 };

        var result = new List<UserEvent> { e1, e2, e3, e4, e5 }.CompactBy(ev => ev.CommentLikeEvent);

        result.Should().HaveCount(2);


        var first = result.Single(r => r.Data.InitiatorUserId == user1);
        first.EventId.Should().Be(2);
        first.Data.Type.Should().Be(LikeType.Like);
        first.TotalCount.Should().Be(2);


        var second = result.Single(r => r.Data.InitiatorUserId == user2);
        second.EventId.Should().Be(5);
        second.Data.Type.Should().Be(LikeType.Unlike);
        second.TotalCount.Should().Be(3);
    }

    [Fact]
    public void CompactBy_Follow_GroupsCorrectly()
    {
        var initiator1 = "initiator1";
        var target1 = "target1";

        var initiator2 = "initiator2";
        var target2 = "target2";

        var f1 = TestUtilities.CreateUserEventWithId(1, new FollowEventData(initiator1, target1, DateTimeOffset.UtcNow, FollowType.Follow), EventActionType.Create);
        var f2 = TestUtilities.CreateUserEventWithId(2, new FollowEventData(initiator1, target1, DateTimeOffset.UtcNow, FollowType.Follow), EventActionType.Create);

        // user2 follows target2 twice, then unfollows once
        var f3 = TestUtilities.CreateUserEventWithId(3, new FollowEventData(initiator2, target2, DateTimeOffset.UtcNow, FollowType.Follow), EventActionType.Create);
        var f4 = TestUtilities.CreateUserEventWithId(4, new FollowEventData(initiator2, target2, DateTimeOffset.UtcNow, FollowType.Follow), EventActionType.Create);
        var f5 = TestUtilities.CreateUserEventWithId(5, new FollowEventData(initiator2, target2, DateTimeOffset.UtcNow, FollowType.Unfollow), EventActionType.Delete);


        // now compact, including the unfollo
        var result = new List<UserEvent> { f1, f2, f3, f4, f5 }
            .CompactBy(ev => ev.FollowEvent);

        // should have one entry per (initiator:target) pair
        result.Should().HaveCount(2);

        // the first pair (initiator1->target1) stayed as Follow, last id = 2, count = 2
        var first = result.Single(r =>
            r.Data.InitiatorUserId == initiator1 &&
            r.Data.TargetUserId == target1);
        first.EventId.Should().Be(2);
        first.Data.Type.Should().Be(FollowType.Follow);
        first.TotalCount.Should().Be(2);

        // the second pair (initiator2->target2) ended as Unfollow, last id = 5, count = 3
        var second = result.Single(r =>
            r.Data.InitiatorUserId == initiator2 &&
            r.Data.TargetUserId == target2);
        second.EventId.Should().Be(5);
        second.Data.Type.Should().Be(FollowType.Unfollow);
        second.TotalCount.Should().Be(3);
    }

    [Fact]
    public void CompactBy_Save_GroupsCorrectly()
    {
        var author1 = "author1";
        var clipId1 = 42;

        var author2 = "author2";
        var clipId2 = 43;

        // author1 saves clip1 twice
        var s1 = TestUtilities.CreateUserEventWithId(1, new SaveEventData(clipId1, author1, "deviceA", "web", "en-US", "Windows", true, DateTimeOffset.UtcNow, SaveType.Save), EventActionType.Create);
        var s2 = TestUtilities.CreateUserEventWithId(2, new SaveEventData(clipId1, author1, "deviceA", "web", "en-US", "Windows", true, DateTimeOffset.UtcNow, SaveType.Save), EventActionType.Create);

        // author2 saves clip2 twice, then unsaves
        var s3 = TestUtilities.CreateUserEventWithId(3, new SaveEventData(clipId2, author2, "deviceB", "mobile", "fr-FR", "iOS", true, DateTimeOffset.UtcNow, SaveType.Save), EventActionType.Create);
        var s4 = TestUtilities.CreateUserEventWithId(4, new SaveEventData(clipId2, author2, "deviceB", "mobile", "fr-FR", "iOS", true, DateTimeOffset.UtcNow, SaveType.Save), EventActionType.Create);
        var s5 = TestUtilities.CreateUserEventWithId(5, new SaveEventData(clipId2, author2, "deviceB", "mobile", "fr-FR", "iOS", true, DateTimeOffset.UtcNow, SaveType.Unsave), EventActionType.Delete);

        // compact all save/unsave events
        var result = new List<UserEvent> { s1, s2, s3, s4, s5 }
            .CompactBy(ev => ev.SaveEvent);

        // one entry per (author:clip) key
        result.Should().HaveCount(2);

        // first key: author1:clip1 — still saved, last event id = 2, total actions = 2
        var first = result.Single(r =>
            r.Data.AuthorId == author1 &&
            r.Data.ClipId == clipId1);
        first.EventId.Should().Be(2);
        first.Data.Type.Should().Be(SaveType.Save);
        first.TotalCount.Should().Be(2);

        // second key: author2:clip2 — ended unsaved, last event id = 5, total actions = 3
        var second = result.Single(r =>
            r.Data.AuthorId == author2 &&
            r.Data.ClipId == clipId2);
        second.EventId.Should().Be(5);
        second.Data.Type.Should().Be(SaveType.Unsave);
        second.TotalCount.Should().Be(3);
    }

    [Fact]
    public void CompactBy_View_GroupsCorrectly()
    {
        var author1 = "author1";
        var clipId1 = 101;

        var author2 = "author2";
        var clipId2 = 102;

        // author1 views clip1 twice with different metrics
        var v1 = TestUtilities.CreateUserEventWithId(1, new ViewEventData(
            clipId1,
            author1,
            10.0,      // ViewDurationInSeconds
            0.50m,     // PercentViewed
            false,     // Muted
            1,         // ReplayCount
            "deviceA",
            "web",
            "en-US",
            "Windows",
            true,      // UserIsLoggedIn
            DateTimeOffset.UtcNow
        ),
        EventActionType.Create);
        var v2 = TestUtilities.CreateUserEventWithId(2, new ViewEventData(
            clipId1,
            author1,
            20.0,
            0.75m,
            true,
            2,
            "deviceA",
            "web",
            "en-US",
            "Windows",
            true,
            DateTimeOffset.UtcNow
        ), EventActionType.Create);

        // author2 views clip2 twice, then a third time
        var v3 = TestUtilities.CreateUserEventWithId(3, new ViewEventData(
            clipId2,
            author2,
            5.0,
            0.25m,
            false,
            1,
            "deviceB",
            "mobile",
            "fr-FR",
            "iOS",
            false,
            DateTimeOffset.UtcNow
        ), EventActionType.Create);
        var v4 = TestUtilities.CreateUserEventWithId(4, new ViewEventData(
            clipId2,
            author2,
            15.0,
            0.60m,
            false,
            2,
            "deviceB",
            "mobile",
            "fr-FR",
            "iOS",
            false,
            DateTimeOffset.UtcNow
        ), EventActionType.Create);
        var v5 = TestUtilities.CreateUserEventWithId(5, new ViewEventData(
            clipId2,
            author2,
            30.0,
            1.00m,
            true,
            3,
            "deviceB",
            "mobile",
            "fr-FR",
            "iOS",
            false,
            DateTimeOffset.UtcNow
        ), EventActionType.Create);

        // compact all view events by (AuthorId:ClipId)
        var result = new List<UserEvent> { v1, v2, v3, v4, v5 }
            .CompactBy(ev => ev.ViewEvent);

        // one entry per author+clip key
        result.Should().HaveCount(2);

        // author1–clip1: last event is v2
        var first = result.Single(r =>
            r.Data.AuthorId == author1 &&
            r.Data.ClipId == clipId1);
        first.EventId.Should().Be(2);
        first.Data.ViewDurationInSeconds.Should().Be(20.0);
        first.Data.Muted.Should().BeTrue();
        first.Data.ReplayCount.Should().Be(2);
        first.TotalCount.Should().Be(2);

        // author2–clip2: last event is v5
        var second = result.Single(r =>
            r.Data.AuthorId == author2 &&
            r.Data.ClipId == clipId2);
        second.EventId.Should().Be(5);
        second.Data.ViewDurationInSeconds.Should().Be(30.0);
        second.Data.Muted.Should().BeTrue();
        second.Data.ReplayCount.Should().Be(3);
        second.TotalCount.Should().Be(3);
    }



    [Fact]
    public void CompactBy_View_GroupedByDeviceWhenAuthorIsNull()
    {
        var clipId = 200;
        var device = "anon-device-1";

        // two anonymous (null AuthorId) views on the same clip from the same device
        var v1 = TestUtilities.CreateUserEventWithId(1, new ViewEventData(
            clipId, null, 5.0, 0.25m, false, 1,
            device, "web", "en-US", "Windows", false, DateTimeOffset.UtcNow
        ), EventActionType.Create);
        var v2 = TestUtilities.CreateUserEventWithId(2, new ViewEventData(
            clipId, null, 10.0, 0.50m, true, 2,
            device, "web", "en-US", "Windows", false, DateTimeOffset.UtcNow
        ), EventActionType.Create);
        // a view from a different device should not join the same group
        var v3 = TestUtilities.CreateUserEventWithId(3, new ViewEventData(
            clipId, null, 8.0, 0.40m, false, 1,
            "anon-device-2", "web", "en-US", "Windows", false, DateTimeOffset.UtcNow
        ), EventActionType.Create);

        var result = new List<UserEvent> { v1, v2, v3 }
            .CompactBy(ev => ev.ViewEvent);

        // should get one group for device1:clip and one for device2:clip
        result.Should().HaveCount(2);

        var first = result.Single(r => r.Data.DeviceId == device);
        first.EventId.Should().Be(2);
        first.Data.ViewDurationInSeconds.Should().Be(10.0);
        first.TotalCount.Should().Be(2);

        var second = result.Single(r => r.Data.DeviceId == "anon-device-2");
        second.EventId.Should().Be(3);
        second.Data.ViewDurationInSeconds.Should().Be(8.0);
        second.TotalCount.Should().Be(1);
    }


    [Fact]
    public void LikeEventData_GetCompactionKey_ReturnsExpected()
    {
        var data = new LikeEventData(
            AuthorId: "user1",
            ClipId: 123,
            Timestamp: DateTimeOffset.UtcNow,
            Platform: "Web",
            DevicePlatform: "Windows",
            DeviceId: "device123",
            BrowserLanguage: "en-US",
            UserIsLoggedIn: true,
            Type: LikeType.Like
        );

        data.GetCompactionKey().Should().Be("user1:123");
    }

    [Fact]
    public void ViewEventData_GetCompactionKey_WithAuthor_ReturnsExpected()
    {
        var data = new ViewEventData(
            ClipId: 200,
            AuthorId: "user42",
            ViewDurationInSeconds: 5,
            PercentViewed: 0.5m,
            Muted: false,
            ReplayCount: 1,
            DeviceId: "deviceA",
            Platform: "web",
            BrowserLanguage: "en-US",
            DevicePlatform: "Windows",
            UserIsLoggedIn: true,
            Timestamp: DateTimeOffset.UtcNow
        );

        data.GetCompactionKey().Should().Be("user42:200");
    }

    [Fact]
    public void ViewEventData_GetCompactionKey_WithoutAuthor_ReturnsDeviceKey()
    {
        var data = new ViewEventData(
            ClipId: 200,
            AuthorId: null,
            ViewDurationInSeconds: 5,
            PercentViewed: 0.5m,
            Muted: false,
            ReplayCount: 1,
            DeviceId: "anon-device-xyz",
            Platform: "web",
            BrowserLanguage: "en-US",
            DevicePlatform: "Windows",
            UserIsLoggedIn: false,
            Timestamp: DateTimeOffset.UtcNow
        );

        data.GetCompactionKey().Should().Be("anon-device-xyz:200");
    }

    [Fact]
    public void CommentEventData_GetCompactionKey_ReturnsExpected()
    {
        var data = new CommentEventData(
            CommentId: 99,
            CommentThreadId: 1,
            ParentCommentId: null,
            RootCommentId: null,
            AuthorId: "author42",
            Text: "test",
            Platform: "web",
            DeviceId: "dev",
            DevicePlatform: "win",
            BrowserLanguage: "en",
            UserIsLoggedIn: true,
            Timestamp: DateTimeOffset.UtcNow,
            Type: CommentType.Post
        );

        data.GetCompactionKey().Should().Be("author42:99");
    }

    [Fact]
    public void CommentLikeEventData_GetCompactionKey_ReturnsExpected()
    {
        var data = new CommentLikeEventData(
            CommentId: 123,
            InitiatorUserId: "liker1",
            Timestamp: DateTimeOffset.UtcNow,
            Type: LikeType.Like
        );

        data.GetCompactionKey().Should().Be("liker1:123");
    }

    [Fact]
    public void SaveEventData_GetCompactionKey_ReturnsExpected()
    {
        var data = new SaveEventData(
            ClipId: 88,
            AuthorId: "saver1",
            DeviceId: "d1",
            Platform: "web",
            BrowserLanguage: "en",
            DevicePlatform: "linux",
            UserIsLoggedIn: true,
            Timestamp: DateTimeOffset.UtcNow,
            Type: SaveType.Save
        );

        data.GetCompactionKey().Should().Be("saver1:88");
    }

    [Fact]
    public void FollowEventData_GetCompactionKey_ReturnsExpected()
    {
        var data = new FollowEventData(
            InitiatorUserId: "userA",
            TargetUserId: "userB",
            Timestamp: DateTimeOffset.UtcNow,
            Type: FollowType.Follow
        );

        data.GetCompactionKey().Should().Be("userA:userB");
    }

    [Fact]
    public void UnknownType_ThrowsInvalidOperationException()
    {
        var unknown = new DummyCompactionKeyProvider();

        Action act = () => unknown.GetCompactionKey();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*DummyCompactionKeyProvider*");
    }

    private class DummyCompactionKeyProvider : ICompactionKeyProvider { }

}


public static class TestUtilities
{
    // Base method for creating a UserEvent
    public static UserEvent CreateUserEvent<T>(
        T eventData,
        EventActionType actionType)
        where T : class, ICompactionKeyProvider
    {
        return new UserEvent
        {
            ActionType = actionType,
            EntityName = typeof(T).Name,
            Platform = "web",
            BrowserLanguage = "en-US",
            DevicePlatform = "Windows",
            DeviceId = $"device-1",
            Timestamp = DateTimeOffset.UtcNow,
            UserIsLoggedIn = eventData is { } ? true : false,
            LikeEvent = eventData as LikeEventData,
            CommentEvent = eventData as CommentEventData,
            CommentLikeEvent = eventData as CommentLikeEventData,
            ViewEvent = eventData as ViewEventData,
            SaveEvent = eventData as SaveEventData,
            FollowEvent = eventData as FollowEventData
        };
    }

    public static UserEvent CreateUserEventWithId<T>(
    int Id,
    T eventData,
    EventActionType actionType)
    where T : class, ICompactionKeyProvider
    {
        return new UserEvent
        {
            Id = Id,
            ActionType = actionType,
            EntityName = typeof(T).Name,
            Platform = "web",
            BrowserLanguage = "en-US",
            DevicePlatform = "Windows",
            DeviceId = $"device-1",
            Timestamp = DateTimeOffset.UtcNow,
            UserIsLoggedIn = eventData is { } ? true : false,
            LikeEvent = eventData as LikeEventData,
            CommentEvent = eventData as CommentEventData,
            CommentLikeEvent = eventData as CommentLikeEventData,
            ViewEvent = eventData as ViewEventData,
            SaveEvent = eventData as SaveEventData,
            FollowEvent = eventData as FollowEventData
        };
    }

    // Helper for Like events (clip likes)
    public static UserEvent CreateLikeEvent(
        string authorId,
        int clipId,
        LikeType likeType = LikeType.Like)
    {
        var action = likeType == LikeType.Like
            ? EventActionType.Create
            : EventActionType.Delete;

        var data = new LikeEventData(
            ClipId: clipId,
            DeviceId: $"device-1",
            AuthorId: authorId,
            Platform: "web",
            BrowserLanguage: "en-US",
            DevicePlatform: "Windows",
            UserIsLoggedIn: true,
            Timestamp: DateTimeOffset.UtcNow,
            Type: likeType);

        return CreateUserEvent(data, action);
    }

    // Helper for Save events (clip saves)
    public static UserEvent CreateSaveEvent(
        string authorId,
        int clipId,
        SaveType saveType = SaveType.Save)
    {
        var action = saveType == SaveType.Save
            ? EventActionType.Create
            : EventActionType.Delete;

        var data = new SaveEventData(
            ClipId: clipId,
            AuthorId: authorId,
            DeviceId: $"device-1",
            Platform: "web",
            BrowserLanguage: "en-US",
            DevicePlatform: "Windows",
            UserIsLoggedIn: true,
            Timestamp: DateTimeOffset.UtcNow,
            Type: saveType);

        return CreateUserEvent(data, action);
    }

    // Helper for Comment events (posting and deleting comments)
    public static UserEvent CreateCommentEvent(
        long commentId, // PK
        long? parentCommentId,
        long? rootCommentId,
        int threadId,
        string authorId,
        string text,
        CommentType commentType = CommentType.Post)
    {
        var action = commentType == CommentType.Post
            ? EventActionType.Create
            : EventActionType.Delete;

        var data = new CommentEventData(
            CommentId: commentId,
            CommentThreadId: threadId,
            ParentCommentId: parentCommentId,
            RootCommentId: rootCommentId,
            AuthorId: authorId,
            Text: text,
            Platform: "web",
            DeviceId: $"device-{commentId}",
            DevicePlatform: "Windows",
            BrowserLanguage: "en-US",
            UserIsLoggedIn: true,
            Timestamp: DateTimeOffset.UtcNow,
            Type: commentType);

        return CreateUserEvent(data, action);
    }

    // Helper for CommentLike events
    public static UserEvent CreateCommentLikeEvent(
        long commentId,
        string initiatorUserId,
        LikeType likeType = LikeType.Like)
    {
        var action = likeType == LikeType.Like
            ? EventActionType.Create
            : EventActionType.Delete;

        var data = new CommentLikeEventData(
            CommentId: commentId,
            InitiatorUserId: initiatorUserId,
            Timestamp: DateTimeOffset.UtcNow,
            Type: likeType);

        return CreateUserEvent(data, action);
    }

    // Helper for Follow events (follow/unfollow users)
    public static UserEvent CreateFollowEvent(
        string initiatorUserId,
        string targetUserId,
        FollowType followType = FollowType.Follow)
    {
        var action = followType == FollowType.Follow
            ? EventActionType.Create
            : EventActionType.Delete;

        var data = new FollowEventData(
            InitiatorUserId: initiatorUserId,
            TargetUserId: targetUserId,
            Timestamp: DateTimeOffset.UtcNow,
            Type: followType);

        return CreateUserEvent(data, action);
    }

    // Helper for View events (clip views)
    public static UserEvent CreateViewEvent(
        int clipId,
        string? authorId = null,
        string deviceId = "device-1",
        double duration = 0,
        decimal percentViewed = 1.0m,
        bool muted = false,
        int replayCount = 1)
    {
        var data = new ViewEventData(
            ClipId: clipId,
            AuthorId: authorId,
            ViewDurationInSeconds: duration,
            PercentViewed: percentViewed,
            Muted: muted,
            ReplayCount: replayCount,
            DeviceId: deviceId,
            Platform: "web",
            BrowserLanguage: "en-US",
            DevicePlatform: "Windows",
            UserIsLoggedIn: authorId is not null,
            Timestamp: DateTimeOffset.UtcNow);

        return CreateUserEvent(data, EventActionType.Create);
    }
}
