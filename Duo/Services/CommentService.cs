using System;
using System.Collections.Generic;
using Duo.Models;
using Duo.Services;
using Duo.Repositories;

namespace Duo.Services
{
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

        public List<Comment> GetCommentsByPostId(int postId)
        {
            if (postId <= 0) throw new ArgumentException("Invalid post ID", nameof(postId));

            try
            {
                var comments = _commentRepository.GetCommentsByPostId(postId);   

                if (comments != null && comments.Count > 0)
                {
                    foreach (var comment in comments)
                    {
                        try
                        {
                            User user = _userService.GetUserById(comment.UserId);
                            comment.Username = user.Username;
                            
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                else
                {
                    return new List<Comment>();
                }

                return comments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving comments for post ID {postId}: {ex.Message}", ex);
            }
        }

        public int CreateComment(string content, int postId, int? parentCommentId = null)
        {
            if (postId <= 0) throw new ArgumentException("Invalid post ID", nameof(postId));
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty", nameof(content));

            try
            {
                ValidateCommentCount(postId);

                int level = 1; 
                if (parentCommentId.HasValue)
                {
                    var parentComment = _commentRepository.GetCommentById(parentCommentId.Value);
                    if (parentComment == null) throw new Exception("Parent comment not found");
                    if (parentComment.Level >= 5) throw new Exception("Comment nesting limit reached");
                    level = parentComment.Level + 1;
                }

                User user = _userService.GetCurrentUser();

                var comment = new Comment
                {
                    Content = content,
                    PostId = postId,
                    UserId = user.UserId,
                    ParentCommentId = parentCommentId,
                    CreatedAt = DateTime.Now,
                    Level = level
                };

                return _commentRepository.CreateComment(comment);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating comment: {ex.Message}", ex);
            }
        }

        public bool DeleteComment(int commentId, int userId)
        {
            if (commentId <= 0) throw new ArgumentException("Invalid comment ID", nameof(commentId));
            if (userId <= 0) throw new ArgumentException("Invalid user ID", nameof(userId));

            try
            {
                User user = _userService.GetCurrentUser();
                if (user.UserId != userId) throw new Exception("User does not have permission to delete this comment");

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

        private void ValidateCommentCount(int postId)
        {
            var post = _postRepository.GetPostById(postId);
            if (post == null) throw new Exception("Post not found");

            var commentCount = _commentRepository.GetCommentsCountForPost(postId);
            if (commentCount >= 1000) throw new Exception("Comment limit reached");
        }

        private void ValidateCommentNestingLevel(int parentCommentId)
        {
            var parentComment = _commentRepository.GetCommentById(parentCommentId);
            if (parentComment == null) throw new Exception("Parent comment not found");
            if (parentComment.Level >= 5) throw new Exception("Comment nesting limit reached");
        }
    }
}