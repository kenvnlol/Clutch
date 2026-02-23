using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clutch.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastLoginDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    AvatarUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileBio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FollowerCount = table.Column<int>(type: "int", nullable: false),
                    FollowingCount = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SubmissionCount = table.Column<int>(type: "int", nullable: false),
                    TotalLikes = table.Column<int>(type: "int", nullable: false),
                    TotalViews = table.Column<int>(type: "int", nullable: false),
                    Sponsors = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventConsumerOffsets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumerGroup = table.Column<int>(type: "int", nullable: false),
                    LastProcessedEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventConsumerOffsets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectThreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantAId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParticipantBId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastMessageAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasNewUnreadMessage = table.Column<bool>(type: "bit", nullable: false),
                    ParticipantALastReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ParticipantBLastReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectThreads_AspNetUsers_ParticipantAId",
                        column: x => x.ParticipantAId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DirectThreads_AspNetUsers_ParticipantBId",
                        column: x => x.ParticipantBId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DirectThreads_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Follows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitiatorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Follows_AspNetUsers_InitiatorUserId",
                        column: x => x.InitiatorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Follows_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mentions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitiatorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mentions_AspNetUsers_InitiatorUserId",
                        column: x => x.InitiatorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mentions_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInboxes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnseenCommentLikeCount = table.Column<int>(type: "int", nullable: false),
                    UnseenCommentReplyCount = table.Column<int>(type: "int", nullable: false),
                    UnseenCommentClipCount = table.Column<int>(type: "int", nullable: false),
                    UnseenUserMentionCount = table.Column<int>(type: "int", nullable: false),
                    UnseenUserFollowCount = table.Column<int>(type: "int", nullable: false),
                    UnseenClipLikeCount = table.Column<int>(type: "int", nullable: false),
                    UnseenDirectMessageCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInboxes", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserInboxes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWallet",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalEarned = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWallet", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserWallet_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VideoBlobUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarBlobUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationInSeconds = table.Column<int>(type: "int", nullable: false),
                    CommentCount = table.Column<int>(type: "int", nullable: false),
                    ShareCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    SaveCount = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastCounterUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clips_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clips_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEvent_CommentEvent",
                columns: table => new
                {
                    UserEventId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    CommentThreadId = table.Column<int>(type: "int", nullable: false),
                    RootCommentId = table.Column<long>(type: "bigint", nullable: true),
                    ParentCommentId = table.Column<long>(type: "bigint", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvent_CommentEvent", x => x.UserEventId);
                    table.ForeignKey(
                        name: "FK_UserEvent_CommentEvent_UserEvents_UserEventId",
                        column: x => x.UserEventId,
                        principalTable: "UserEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEvent_CommentLikeEvent",
                columns: table => new
                {
                    UserEventId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    InitiatorUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvent_CommentLikeEvent", x => x.UserEventId);
                    table.ForeignKey(
                        name: "FK_UserEvent_CommentLikeEvent_UserEvents_UserEventId",
                        column: x => x.UserEventId,
                        principalTable: "UserEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEvent_FollowEvent",
                columns: table => new
                {
                    UserEventId = table.Column<int>(type: "int", nullable: false),
                    InitiatorUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvent_FollowEvent", x => x.UserEventId);
                    table.ForeignKey(
                        name: "FK_UserEvent_FollowEvent_UserEvents_UserEventId",
                        column: x => x.UserEventId,
                        principalTable: "UserEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEvent_LikeEvent",
                columns: table => new
                {
                    UserEventId = table.Column<int>(type: "int", nullable: false),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvent_LikeEvent", x => x.UserEventId);
                    table.ForeignKey(
                        name: "FK_UserEvent_LikeEvent_UserEvents_UserEventId",
                        column: x => x.UserEventId,
                        principalTable: "UserEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEvent_SaveEvent",
                columns: table => new
                {
                    UserEventId = table.Column<int>(type: "int", nullable: false),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvent_SaveEvent", x => x.UserEventId);
                    table.ForeignKey(
                        name: "FK_UserEvent_SaveEvent_UserEvents_UserEventId",
                        column: x => x.UserEventId,
                        principalTable: "UserEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEvent_ShareEvent",
                columns: table => new
                {
                    UserEventId = table.Column<int>(type: "int", nullable: false),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destination = table.Column<int>(type: "int", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvent_ShareEvent", x => x.UserEventId);
                    table.ForeignKey(
                        name: "FK_UserEvent_ShareEvent_UserEvents_UserEventId",
                        column: x => x.UserEventId,
                        principalTable: "UserEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEvent_ViewEvent",
                columns: table => new
                {
                    UserEventId = table.Column<int>(type: "int", nullable: false),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewDurationInSeconds = table.Column<double>(type: "float", nullable: false),
                    PercentViewed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Muted = table.Column<bool>(type: "bit", nullable: false),
                    ReplayCount = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvent_ViewEvent", x => x.UserEventId);
                    table.ForeignKey(
                        name: "FK_UserEvent_ViewEvent_UserEvents_UserEventId",
                        column: x => x.UserEventId,
                        principalTable: "UserEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DirectThreadId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessages_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessages_DirectThreads_DirectThreadId",
                        column: x => x.DirectThreadId,
                        principalTable: "DirectThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserWalletId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawalRequest_UserWallet_UserWalletId",
                        column: x => x.UserWalletId,
                        principalTable: "UserWallet",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentThreads",
                columns: table => new
                {
                    ClipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentThreads", x => x.ClipId);
                    table.ForeignKey(
                        name: "FK_CommentThreads_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContestWinners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    ContestId = table.Column<int>(type: "int", nullable: false),
                    Placement = table.Column<int>(type: "int", nullable: false),
                    PrizeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestWinners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContestWinners_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestWinners_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Likes_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Saves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Saves_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Saves_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Shares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Destination = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shares_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shares_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Views",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ViewDurationInSeconds = table.Column<double>(type: "float", nullable: false),
                    PercentViewed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Muted = table.Column<bool>(type: "bit", nullable: false),
                    ReplayCount = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Views", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Views_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Views_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentThreadId = table.Column<int>(type: "int", nullable: false),
                    RootCommentId = table.Column<long>(type: "bigint", nullable: true),
                    ParentCommentId = table.Column<long>(type: "bigint", nullable: true),
                    ReplyCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevicePlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_CommentThreads_CommentThreadId",
                        column: x => x.CommentThreadId,
                        principalTable: "CommentThreads",
                        principalColumn: "ClipId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_RootCommentId",
                        column: x => x.RootCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitiatorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentLikes_AspNetUsers_InitiatorUserId",
                        column: x => x.InitiatorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentLikes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InboxActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DestinationUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitiatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserInboxId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowId = table.Column<int>(type: "int", nullable: true),
                    CommentId = table.Column<long>(type: "bigint", nullable: true),
                    MentionId = table.Column<int>(type: "int", nullable: true),
                    LikeId = table.Column<int>(type: "int", nullable: true),
                    CommentLikeId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboxActivities_AspNetUsers_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InboxActivities_CommentLikes_CommentLikeId",
                        column: x => x.CommentLikeId,
                        principalTable: "CommentLikes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InboxActivities_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InboxActivities_Follows_FollowId",
                        column: x => x.FollowId,
                        principalTable: "Follows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InboxActivities_Likes_LikeId",
                        column: x => x.LikeId,
                        principalTable: "Likes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InboxActivities_Mentions_MentionId",
                        column: x => x.MentionId,
                        principalTable: "Mentions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InboxActivities_UserInboxes_UserInboxId",
                        column: x => x.UserInboxId,
                        principalTable: "UserInboxes",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clips_AuthorId",
                table: "Clips",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clips_GameId",
                table: "Clips",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_CommentId",
                table: "CommentLikes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_InitiatorUserId_CommentId",
                table: "CommentLikes",
                columns: new[] { "InitiatorUserId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_UserId",
                table: "CommentLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentId",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentThreadId",
                table: "Comments",
                column: "CommentThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RootCommentId",
                table: "Comments",
                column: "RootCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestWinners_ClipId",
                table: "ContestWinners",
                column: "ClipId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestWinners_ContestId",
                table: "ContestWinners",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_AuthorId",
                table: "DirectMessages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_DirectThreadId",
                table: "DirectMessages",
                column: "DirectThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectThreads_ParticipantAId_ParticipantBId",
                table: "DirectThreads",
                columns: new[] { "ParticipantAId", "ParticipantBId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectThreads_ParticipantBId",
                table: "DirectThreads",
                column: "ParticipantBId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectThreads_UserId",
                table: "DirectThreads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_InitiatorUserId_TargetUserId",
                table: "Follows",
                columns: new[] { "InitiatorUserId", "TargetUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Follows_TargetUserId",
                table: "Follows",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InboxActivities_CommentId",
                table: "InboxActivities",
                column: "CommentId",
                unique: true,
                filter: "[CommentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InboxActivities_CommentLikeId",
                table: "InboxActivities",
                column: "CommentLikeId",
                unique: true,
                filter: "[CommentLikeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InboxActivities_FollowId",
                table: "InboxActivities",
                column: "FollowId",
                unique: true,
                filter: "[FollowId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InboxActivities_InitiatorId",
                table: "InboxActivities",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_InboxActivities_LikeId",
                table: "InboxActivities",
                column: "LikeId",
                unique: true,
                filter: "[LikeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InboxActivities_MentionId",
                table: "InboxActivities",
                column: "MentionId",
                unique: true,
                filter: "[MentionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InboxActivities_UserInboxId",
                table: "InboxActivities",
                column: "UserInboxId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorId_ClipId",
                table: "Likes",
                columns: new[] { "AuthorId", "ClipId" });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ClipId",
                table: "Likes",
                column: "ClipId");

            migrationBuilder.CreateIndex(
                name: "IX_Mentions_InitiatorUserId",
                table: "Mentions",
                column: "InitiatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Mentions_TargetUserId",
                table: "Mentions",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Saves_AuthorId_ClipId",
                table: "Saves",
                columns: new[] { "AuthorId", "ClipId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Saves_ClipId",
                table: "Saves",
                column: "ClipId");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_AuthorId_ClipId",
                table: "Shares",
                columns: new[] { "AuthorId", "ClipId" });

            migrationBuilder.CreateIndex(
                name: "IX_Shares_ClipId",
                table: "Shares",
                column: "ClipId");

            migrationBuilder.CreateIndex(
                name: "IX_Views_AuthorId_ClipId",
                table: "Views",
                columns: new[] { "AuthorId", "ClipId" });

            migrationBuilder.CreateIndex(
                name: "IX_Views_ClipId",
                table: "Views",
                column: "ClipId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequest_UserWalletId",
                table: "WithdrawalRequest",
                column: "UserWalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Blobs");

            migrationBuilder.DropTable(
                name: "ContestWinners");

            migrationBuilder.DropTable(
                name: "DirectMessages");

            migrationBuilder.DropTable(
                name: "EventConsumerOffsets");

            migrationBuilder.DropTable(
                name: "InboxActivities");

            migrationBuilder.DropTable(
                name: "Saves");

            migrationBuilder.DropTable(
                name: "Shares");

            migrationBuilder.DropTable(
                name: "UserEvent_CommentEvent");

            migrationBuilder.DropTable(
                name: "UserEvent_CommentLikeEvent");

            migrationBuilder.DropTable(
                name: "UserEvent_FollowEvent");

            migrationBuilder.DropTable(
                name: "UserEvent_LikeEvent");

            migrationBuilder.DropTable(
                name: "UserEvent_SaveEvent");

            migrationBuilder.DropTable(
                name: "UserEvent_ShareEvent");

            migrationBuilder.DropTable(
                name: "UserEvent_ViewEvent");

            migrationBuilder.DropTable(
                name: "Views");

            migrationBuilder.DropTable(
                name: "WithdrawalRequest");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Contests");

            migrationBuilder.DropTable(
                name: "DirectThreads");

            migrationBuilder.DropTable(
                name: "CommentLikes");

            migrationBuilder.DropTable(
                name: "Follows");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Mentions");

            migrationBuilder.DropTable(
                name: "UserInboxes");

            migrationBuilder.DropTable(
                name: "UserEvents");

            migrationBuilder.DropTable(
                name: "UserWallet");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CommentThreads");

            migrationBuilder.DropTable(
                name: "Clips");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
