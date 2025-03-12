using System.Collections.Generic;

public interface ICommentRepository
{
    int CreateComment(Comment comment);
    Comment GetCommentById(int commentId);
    IEnumerable<Comment> GetAllComments();
    void UpdateComment(int commentId, string newContent);
    void DeleteComment(int commentId);
    IEnumerable<Comment> GetReplies(int parentCommentId);
}