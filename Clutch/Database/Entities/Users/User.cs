using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.DirectThreads;
using Clutch.Database.Entities.Follows;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Likes;
using Clutch.Database.Entities.Mentions;
using Clutch.Database.Entities.Saves;
using Clutch.Database.Entities.Shares;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Database.Entities.UserWallets;
using Clutch.Database.Entities.Views;
using Microsoft.AspNetCore.Identity;

namespace Clutch.Database.Entities.Users;

public sealed class User : IdentityUser
{
    public required string DisplayName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastLoginDate { get; set; }
    public bool IsLoggedIn { get; set; }
    public required string AvatarUri { get; set; }
    public required string ProfileBio { get; set; }
    public int LikesReceivedCount => Clips.Sum(clip => clip.LikeCount);
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public required List<Clip> Clips { get; init; }
    public required List<Comment> Comments { get; init; }
    public required List<Like> Likes { get; init; }
    public required List<View> Views { get; init; }
    public required List<Share> Shares { get; init; }
    public required List<Save> Saves { get; init; }
    public required List<DirectThread> DirectThreads { get; init; }
    public required List<Follow> FollowsMade { get; init; }
    public required List<Follow> FollowsReceived { get; init; }
    public required List<Mention> MentionsMade { get; init; }
    public required List<Mention> MentionsReceived { get; init; }
    public required List<CommentLike> CommentsLiked { get; init; }
    public required List<CommentLike> CommentLikesReceived { get; init; }
    public required UserInbox UserInbox { get; init; }
    public required UserWallet Wallet { get; init; }
    public required List<InboxActivity> InitiatedActivities { get; init; }
}





