using System;
using System.Collections.Generic;

public class CommentService
{
    private readonly CommentRepository _commentRepository;
    private readonly PostRepository _postRepository;
    private readonly UserService _userService;

    public CommentService(CommentRepository commentRepository, PostRepository postRepository, UserService userService)
    {
        _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public Comment GetCommentById(int id)
    {
        if (id <= 0) throw new ArgumentException("Invalid comment ID", nameof(id));

        try
        {
            return _commentRepository.GetCommentById(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving comment with ID {id}: {ex.Message}", ex);
        }
    }

    public List<Comment> GetCommentsByPostId(int postId)
    {
        if (postId <= 0) throw new ArgumentException("Invalid post ID", nameof(postId));

        try
        {
            return _commentRepository.GetCommentsByPostId(postId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving comments for post ID {postId}: {ex.Message}", ex);
        }
    }

    public List<Comment> GetRepliesByCommentId(int commentId)
    {
        if (commentId <= 0) throw new ArgumentException("Invalid comment ID", nameof(commentId));

        try
        {
            return _commentRepository.GetRepliesByCommentId(commentId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving replies for comment ID {commentId}: {ex.Message}", ex);
        }
    }

    public Comment CreateComment(int postId, string content)
    {
        if (postId <= 0) throw new ArgumentException("Invalid post ID", nameof(postId));
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty", nameof(content));

        try
        {
            ValidateCommentCount(postId);

            User user = RetrieveUser();

            var comment = new Comment(1, content, postId, user.Id, null, DateTime.Now, 0, 1);

            return _commentRepository.CreateComment(comment);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating comment for post ID {postId}: {ex.Message}", ex);
        }
    }

    public Comment CreateReply(int parentCommentId, string content)
    {
        if (parentCommentId <= 0) throw new ArgumentException("Invalid parent comment ID", nameof(parentCommentId));
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty", nameof(content));

        try
        {
            var parentComment = _commentRepository.GetCommentById(parentCommentId);

            ValidateCommentCount(parentComment.PostId);
            ValidateCommentNestingLevel(parentComment.Id);

            User user = RetrieveUser();

            var comment = new Comment(1, content, parentComment.PostId, user.Id, parentCommentId, DateTime.Now, 0, parentComment.Level + 1);

            return _commentRepository.CreateComment(comment);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating reply for parent comment ID {parentCommentId}: {ex.Message}", ex);
        }
    }

    public bool UpdateComment(int commentId, string content)
    {
        if (commentId <= 0) throw new ArgumentException("Invalid comment ID", nameof(commentId));
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty", nameof(content));

        try
        {
            var comment = _commentRepository.GetCommentById(commentId);
            if (comment == null) throw new Exception("Comment not found");

            comment.Content = content;
            return _commentRepository.UpdateComment(comment);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating comment with ID {commentId}: {ex.Message}", ex);
        }
    }

    public bool DeleteComment(int commentId)
    {
        if (commentId <= 0) throw new ArgumentException("Invalid comment ID", nameof(commentId));

        try
        {
            return _commentRepository.DeleteComment(commentId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting comment with ID {commentId}: {ex.Message}", ex);
        }
    }

    public bool LikeComment(int commentId)
    {
        if (commentId <= 0) throw new ArgumentException("Invalid comment ID", nameof(commentId));

        try
        {
            return _commentRepository.IncrementLikeCount(commentId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error liking comment with ID {commentId}: {ex.Message}", ex);
        }
    }

    private User RetrieveUser()
    {
        var user = _userService.GetCurrentUser();
        if (user == null) throw new Exception("User not found");

        return user;
    }

    private void ValidateCommentCount(int postId)
    {
        var post = _postRepository.GetPostById(postId);
        if (post == null) throw new Exception("Post not found");

        var commentCount = _commentRepository.GetCommentsCountForPost(postId);
        if (commentCount >= 1000) throw new Exception("Comment limit reached");
    }

    private void ValidateCommentNestingLevel(int parentCommentID)
    {
        var parentComment = _commentRepository.GetCommentById(parentCommentID);
        if (parentComment == null) throw new Exception("Parent comment not found");
        if (parentComment.Level >= 5) throw new Exception("Comment nesting limit reached");
    }
}