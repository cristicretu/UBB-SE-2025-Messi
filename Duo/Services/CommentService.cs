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

        public Comment GetCommentById(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid comment ID", nameof(id));

            try
            {
                var comment = _commentRepository.GetCommentById(id);

                // Add username to the comment
                try
                {
                    User user = _userService.GetUserById(comment.UserId);
                    comment.Username = user.Username;
                }
                catch (Exception)
                {
                    comment.Username = "Unknown User";
                }

                return comment;
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
                System.Diagnostics.Debug.WriteLine($"CommentService: Getting comments for post ID {postId}");
                
                var comments = _commentRepository.GetCommentsByPostId(postId);
                System.Diagnostics.Debug.WriteLine($"CommentService: Retrieved {comments?.Count ?? 0} comments from repository");

                // Add usernames to the comments
                if (comments != null && comments.Count > 0)
                {
                    foreach (var comment in comments)
                    {
                        try
                        {
                            User user = _userService.GetUserById(comment.UserId);
                            comment.Username = user.Username;
                            System.Diagnostics.Debug.WriteLine($"CommentService: Set username '{user.Username}' for comment ID {comment.Id}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"CommentService: Error getting username for comment ID {comment.Id}: {ex.Message}");
                            comment.Username = "Unknown User";
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"CommentService: No comments found for post ID {postId}");
                }

                return comments;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CommentService: Error retrieving comments: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"CommentService: Stack trace: {ex.StackTrace}");
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

        public int CreateComment(string content, int postId, int? parentCommentId = null)
        {
            if (postId <= 0) throw new ArgumentException("Invalid post ID", nameof(postId));
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty", nameof(content));

            try
            {
                // Validate comment count for the post
                ValidateCommentCount(postId);

                // Determine comment level
                int level = 1; // Default level for top-level comments
                if (parentCommentId.HasValue)
                {
                    var parentComment = _commentRepository.GetCommentById(parentCommentId.Value);
                    if (parentComment == null) throw new Exception("Parent comment not found");
                    if (parentComment.Level >= 5) throw new Exception("Comment nesting limit reached");
                    level = parentComment.Level + 1;
                }

                // Retrieve the current user
                User user = _userService.GetCurrentUser();

                // Create the comment
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