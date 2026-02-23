using Clutch.Database;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Database.Entities.Users;
using Clutch.Database.Entities.UserWallets;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Clutch.API.Features.Users.Endpoints;

[Handler]
[MapPost("/users/register")]
public static partial class Register
{
    public sealed record Command(
        string FirstName,
        string LastName,
        string DisplayName,
        string EmailAddress,
        string Password,
        string ConfirmPassword);

    private static async ValueTask HandleAsync(
        Command command,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        ApplicationDbContext dbContext,
        CancellationToken token)
    {
        if (await dbContext.Users.AnyAsync(u => u.DisplayName == command.DisplayName))
        {
            throw new InvalidOperationException("Username is already taken.");
        }

        if (!String.Equals(command.Password, command.ConfirmPassword, StringComparison.Ordinal))
        {
            throw new ValidationException("Password does not match confirm password.");
        }

        var user = new User
        {
            UserName = command.EmailAddress,
            DisplayName = command.DisplayName,
            Email = command.EmailAddress,
            CreatedAt = DateTimeOffset.UtcNow,
            FirstName = command.FirstName,
            LastName = command.LastName,
            EmailConfirmed = true,
            AvatarUri = "",
            Comments = [],
            Clips = [],
            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            FollowsReceived = [],
            FollowsMade = [],
            MentionsMade = [],
            MentionsReceived = [],
            CommentsLiked = [],
            CommentLikesReceived = [],
            ProfileBio = "",
            UserInbox = new UserInbox
            {
                Activities = [],
                UnseenClipLikeCount = 0,
                UnseenCommentClipCount = 0,
                UnseenCommentLikeCount = 0,
                UnseenCommentReplyCount = 0,
                UnseenDirectMessageCount = 0,
                UnseenUserFollowCount = 0,
                UnseenUserMentionCount = 0
            },
            DirectThreads = [],
            InitiatedActivities = [],
            Wallet = new UserWallet
            {
                WithdrawalRequests = []
            }
        };

        var createUserResult = await userManager.CreateAsync(user, command.Password);

        if (!createUserResult.Succeeded)
        {
            throw new ArgumentException(createUserResult.Errors.FirstOrDefault()?.Description ?? "Registration failed with no description available.");
        }


        //var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        //var encodedToken = HttpUtility.UrlEncode(confirmationToken);
        //mailService.SendRegistrationConfirmationEmail(user.Id, request.Email, confirmationToken, user.DisplayName);

        await signInManager.SignInAsync(user, true);
    }
}

