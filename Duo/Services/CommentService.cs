using System;
using System.Collections.Generic;

public class CommentService
{
    private readonly CommentRepository _commentRepository;
    private readonly PostRepository _postRepository;
    private readonly UserService _userService;
    private Dictionary<int, int> _commentNumberPerPost = new Dictionary<int, int>();

    public CommentService(CommentRepository commentRepository, PostRepository postRepository, UserService userService)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userService = userService;
    }

    public Comment GetCommentById(int id)
    {
        try
        {
            return _commentRepository.GetCommentById(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public List<Comment> GetCommentsByPostId(int postId)
    {
        try
        {
            return _commentRepository.GetCommentsByPostId(postId);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public List<Comment> GetRepliesByCommentId(int commentId)
    {
        try
        {
            return _commentRepository.GetRepliesByCommentId(commentId);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Comment CreateComment(int postId, string content)
    {
        try
        {
            var user = _userService.GetCurrentUser();
            
            if (user == null)
                throw new Exception("User not found");

            var comment = new Comment(1, content, postId, user.Id, null, DateTime.Now, 0, 1);

            return _commentRepository.CreateComment(comment);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Comment CreateReply(int parrentCommentId, string content)
    {
        try
        {
            var parentComment = _commentRepository.GetCommentById(parrentCommentId);

            if (parentComment == null)
                throw new Exception("Parent comment not found");

            if (parentComment.Level >= 3) // Enforce maximum 3-level nesting
                throw new Exception("Replies cannot exceed 3 levels of nesting");

            var user = _userService.GetCurrentUser();

            if (user == null)
                throw new Exception("User not found");

            var comment = new Comment(1, content, parentComment.PostId, user.Id, parrentCommentId, DateTime.Now, 0, parentComment.Level + 1);
            return _commentRepository.CreateComment(comment);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool UpdateComment(int commentId, string content)
    {
        try
        {
            var comment = _commentRepository.GetCommentById(commentId);
            comment.Content = content;
            return _commentRepository.UpdateComment(comment);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool DeleteComment(int id)
    {
        try
        {
            return _commentRepository.DeleteComment(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool LikeComment(int commentId)
    {
        try
        {
            return _commentRepository.IncrementLikeCount(commentId);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
