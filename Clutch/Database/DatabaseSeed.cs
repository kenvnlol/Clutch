using Clutch.Database.Entities;
using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.CommentThreads;
using Clutch.Database.Entities.Games;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Database.Entities.Users;
using Clutch.Database.Entities.UserWallets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Database;

public class DatabaseSeed
{
    public static async Task SeedDatabase(
        ApplicationDbContext dbContext,
        UserManager<User> userManager)
    {
        dbContext.Database.EnsureCreated();

        await SeedTopGames(dbContext);
        await SeedDemoUserAsync(dbContext, userManager);
        await SeedDemoClipAsync(dbContext);
        SeedEventOffsets(dbContext);
    }
    public static async Task SeedTopGames(
        ApplicationDbContext dbContext)
    {
        if (dbContext.Games.Any())
        {
            return;
        }
        var games = new List<Game>
        {
            new Game { Title = "Minecraft", Clips = new List<Clip>() },
            new Game { Title = "Grand Theft Auto V", Clips = new List<Clip>() },
            new Game { Title = "Counter-Strike 2", Clips = new List<Clip>() },
            new Game { Title = "World of Warcraft", Clips = new List<Clip>() },
            new Game { Title = "The Legend of Zelda: Breath of the Wild", Clips = new List<Clip>() },
            new Game { Title = "Red Dead Redemption 2", Clips = new List<Clip>() },
            new Game { Title = "The Witcher 3: Wild Hunt", Clips = new List<Clip>() },
            new Game { Title = "Fortnite", Clips = new List<Clip>() },
            new Game { Title = "Overwatch", Clips = new List<Clip>() },
            new Game { Title = "Call of Duty: Modern Warfare", Clips = new List<Clip>() },
            new Game { Title = "PlayerUnknown's Battlegrounds", Clips = new List<Clip>() },
            new Game { Title = "Apex Legends", Clips = new List<Clip>() },
            new Game { Title = "League of Legends", Clips = new List<Clip>() },
            new Game { Title = "Dota 2", Clips = new List<Clip>() },
            new Game { Title = "Animal Crossing: New Horizons", Clips = new List<Clip>() },
            new Game { Title = "Among Us", Clips = new List<Clip>() },
            new Game { Title = "Cyberpunk 2077", Clips = new List<Clip>() },
            new Game { Title = "Halo Infinite", Clips = new List<Clip>() },
            new Game { Title = "Elden Ring", Clips = new List<Clip>() },
            new Game { Title = "God of War", Clips = new List<Clip>() },
            new Game { Title = "Hades", Clips = new List<Clip>() },
            new Game { Title = "Final Fantasy VII Remake", Clips = new List<Clip>() },
            new Game { Title = "Resident Evil Village", Clips = new List<Clip>() },
            new Game { Title = "Ghost of Tsushima", Clips = new List<Clip>() },
            new Game { Title = "Monster Hunter: World", Clips = new List<Clip>() },
            new Game { Title = "Genshin Impact", Clips = new List<Clip>() },
            new Game { Title = "Destiny 2", Clips = new List<Clip>() },
            new Game { Title = "Borderlands 3", Clips = new List<Clip>() },
            new Game { Title = "Assassin's Creed Valhalla", Clips = new List<Clip>() },
            new Game { Title = "FIFA 22", Clips = new List<Clip>() },
            new Game { Title = "NBA 2K22", Clips = new List<Clip>() },
            new Game { Title = "Madden NFL 22", Clips = new List<Clip>() },
            new Game { Title = "The Sims 4", Clips = new List<Clip>() },
            new Game { Title = "Fall Guys: Ultimate Knockout", Clips = new List<Clip>() },
            new Game { Title = "Rocket League", Clips = new List<Clip>() },
            new Game { Title = "Terraria", Clips = new List<Clip>() },
            new Game { Title = "Stardew Valley", Clips = new List<Clip>() },
            new Game { Title = "Valorant", Clips = new List<Clip>() },
            new Game { Title = "Diablo III", Clips = new List<Clip>() },
            new Game { Title = "The Elder Scrolls V: Skyrim", Clips = new List<Clip>() },
            new Game { Title = "Super Mario Odyssey", Clips = new List<Clip>() },
            new Game { Title = "Mario Kart 8 Deluxe", Clips = new List<Clip>() },
            new Game { Title = "Splatoon 2", Clips = new List<Clip>() },
            new Game { Title = "Metroid Dread", Clips = new List<Clip>() },
            new Game { Title = "Persona 5", Clips = new List<Clip>() },
            new Game { Title = "Bloodborne", Clips = new List<Clip>() },
            new Game { Title = "Sekiro: Shadows Die Twice", Clips = new List<Clip>() },
            new Game { Title = "Dark Souls III", Clips = new List<Clip>() },
            new Game { Title = "Hollow Knight", Clips = new List<Clip>() }
        };

        dbContext.Games.AddRange(games);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedDemoUserAsync(
    ApplicationDbContext dbContext,
    UserManager<User> userManager)
    {
        const string email = "clutchaccount@gmail.com";
        const string password = "ClutchPassword123***";

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return;

        var user = new User
        {
            UserName = email,
            Email = email,
            DisplayName = "clutch_account",
            FirstName = "Clutch",
            LastName = "Account",
            EmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow,
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

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception("Failed to seed demo user: " +
                string.Join(",", result.Errors.Select(e => e.Description)));
    }

    private static async Task SeedDemoClipAsync(
    ApplicationDbContext dbContext)
    {
        if (await dbContext.Clips.AnyAsync())
            return;

        var game = await dbContext.Games
            .FirstAsync(g => g.Title == "Counter-Strike 2");

        var user = await dbContext.Users.FirstAsync();

        var clip = new Clip
        {
            Description = "Insane Donk highlight.",
            AuthorId = user.Id,
            GameId = game.Id,
            Timestamp = DateTimeOffset.UtcNow,

            CommentThread = new CommentThread
            {
                Comments = []
            },

            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            ContestWins = [],

            Media = new ClipMedia
            {
                OriginalUpload = new BlobReference
                {
                    ContainerName = "clips",
                    BlobName = "demo-clutch.mp4"
                },
                Avatar = new BlobReference
                {
                    ContainerName = "thumbnails",
                    BlobName = "demo-clutch.jpg"
                }
            },
            ExternalVideoUrl = "https://www.youtube.com/watch?v=U5R9FgsFtNs"
        };

        dbContext.Clips.Add(clip);
        await dbContext.SaveChangesAsync();
    }



    public static void SeedEventOffsets(ApplicationDbContext dbContext)
    {
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
    }

}
