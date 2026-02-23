using MediatR;

namespace Clutch.Features.DirectThreads.Shared;

public sealed record MessagePosted(string RecipientUserId) : INotification;