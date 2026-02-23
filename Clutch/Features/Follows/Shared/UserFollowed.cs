using MediatR;

namespace Clutch.Features.Follows.Shared;

public record UserFollowed(string RecipientUserId) : INotification;