using MediatR;

namespace Clutch.Features.CommentThreads.Shared;

public sealed record CommentChanged(string RecipientUserId, bool IsDelete) : INotification;

