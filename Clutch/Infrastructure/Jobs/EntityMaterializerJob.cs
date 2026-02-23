using Clutch.Features.CommentLikes.Services;
using Clutch.Features.CommentThreads.Services;
using Clutch.Features.Follows.Services;
using Clutch.Features.Likes.Services;
using Clutch.Features.Saves.Services;
using Clutch.Features.Views.Services;
using Hangfire;
using Immediate.Cache;
using Immediate.Handlers.Shared;

namespace Clutch.Infrastructure.Jobs;
[DisableConcurrentExecution(60)]
public class EntityMaterializerJob(
    Owned<IHandler<MaterializeLikes.Request, ValueTuple>> likesHandler,
    Owned<IHandler<MaterializeCommentLikes.Request, ValueTuple>> commentLikesHandler,
    Owned<IHandler<MaterializeComments.Request, ValueTuple>> commentsHandler,
    Owned<IHandler<MaterializeSaves.Request, ValueTuple>> savesHandler,
    Owned<IHandler<MaterializeViews.Request, ValueTuple>> viewsHandler,
    Owned<IHandler<MaterializeFollows.Request, ValueTuple>> followsHandler
    )
    : IRecurringJob
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        await Task.WhenAll(
          RunAndDisposeScoped(likesHandler, new MaterializeLikes.Request(), cancellationToken),
          RunAndDisposeScoped(commentLikesHandler, new MaterializeCommentLikes.Request(), cancellationToken),
          RunAndDisposeScoped(commentsHandler, new MaterializeComments.Request(), cancellationToken),
          RunAndDisposeScoped(savesHandler, new MaterializeSaves.Request(), cancellationToken),
          RunAndDisposeScoped(viewsHandler, new MaterializeViews.Request(), cancellationToken),
          RunAndDisposeScoped(followsHandler, new MaterializeFollows.Request(), cancellationToken)
          );

    }

    private static async Task RunAndDisposeScoped<TRequest>(
        Owned<IHandler<TRequest, ValueTuple>> ownedHandler,
        TRequest request,
        CancellationToken ct)
        where TRequest : class
    {
        await using var scope = ownedHandler.GetScope();
        await scope.Service.HandleAsync(request, ct);
    }
}
