using MediatR;

namespace Clutch.Features.Likes.Shared;

public record ClipLiked(string RecipientUserId) : INotification;