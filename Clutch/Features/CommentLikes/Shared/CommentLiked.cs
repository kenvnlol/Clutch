using MediatR;

namespace Clutch.Features.CommentLikes.Shared;

public sealed record CommentLiked(string RecipientUserId) : INotification;