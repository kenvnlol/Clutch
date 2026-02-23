namespace Clutch.Database.Entities.Comments;
public static class CommentExtensions
{
    public static bool IsTopLevelComment(this Comment comment)
        => comment is { RootCommentId: null, ParentCommentId: null };

    public static bool IsReply(this Comment comment)
        => comment.ParentCommentId is not null;
}
