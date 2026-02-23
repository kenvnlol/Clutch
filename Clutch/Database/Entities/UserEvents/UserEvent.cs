namespace Clutch.Database.Entities.UserEvents;

public class UserEvent
{
    public int Id { get; init; }
    public required string EntityName { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public EventActionType ActionType { get; init; }
    public required string Platform { get; init; }
    public required string BrowserLanguage { get; init; }
    public required string DevicePlatform { get; init; }
    public required string DeviceId { get; init; }
    public bool UserIsLoggedIn { get; init; }
    public LikeEventData? LikeEvent { get; init; }
    public CommentEventData? CommentEvent { get; init; }
    public CommentLikeEventData? CommentLikeEvent { get; init; }
    public ViewEventData? ViewEvent { get; init; }
    public ShareEventData? ShareEvent { get; init; }
    public FollowEventData? FollowEvent { get; init; }
    public SaveEventData? SaveEvent { get; init; }
}



