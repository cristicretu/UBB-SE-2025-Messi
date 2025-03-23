using Duo.Models;
using Duo.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using static Duo.App;

namespace Duo.Views.Components
{
    public sealed partial class Comment : UserControl
    {
        private Models.Comment _commentData;
        private Dictionary<int?, List<Models.Comment>> _commentsByParent;
        private readonly CommentService _commentService;
        
        // Event for when reply is submitted
        public event EventHandler<CommentReplyEventArgs> ReplySubmitted;
        
        // Event for when a comment is liked
        public event EventHandler<CommentLikedEventArgs> CommentLiked;

        public Comment()
        {
            this.InitializeComponent();
            
            // Initialize services
            _commentService = new CommentService(_commentRepository, _postRepository, userService);
            
            CommentReplyButton.Click += CommentReplyButton_Click;
            
            LikeButton.LikeClicked += LikeButton_LikeClicked;
        }

        public void SetCommentData(Models.Comment comment, Dictionary<int?, List<Models.Comment>> commentsByParent)
        {
            _commentData = comment;
            _commentsByParent = commentsByParent;

            // Set the comment data
            UserTextBlock.Text = $"u/{comment.Username}";
            DescriptionTextBlock.Text = comment.Content;
            DateTextBlock.Text = FormatDate(comment.CreatedAt);
            
            // Set like count and comment ID for the like button
            LikeButton.LikeCount = comment.LikeCount;
            LikeButton.CommentId = comment.Id;

            // Generate the visual indicators for comment level
            var indentationLevels = new List<int>();
            // We start from 1 because we're adjusting the level to be 0-based for visual indentation
            // The comment model's Level property is 1-based (1 = top level, 2 = first reply, etc.)
            for (int i = 1; i < comment.Level; i++)
            {
                indentationLevels.Add(i);
            }
            LevelLinesRepeater.ItemsSource = indentationLevels;

            // Add child comments if any
            if (commentsByParent.ContainsKey(comment.Id))
            {
                foreach (var childComment in commentsByParent[comment.Id])
                {
                    var childCommentControl = new Comment();
                    childCommentControl.SetCommentData(childComment, commentsByParent);
                    childCommentControl.ReplySubmitted += ChildComment_ReplySubmitted;
                    childCommentControl.CommentLiked += ChildComment_CommentLiked;
                    ChildCommentsPanel.Children.Add(childCommentControl);
                }
            }
            
            // Hide the reply input if visible
            HideReplyInput();
        }
        
        private void LikeButton_LikeClicked(object sender, LikeButtonClickedEventArgs e)
        {
            if (e.TargetType == LikeTargetType.Comment && e.TargetId == _commentData.Id)
            {
                try
                {
                    // Like the comment using the comment service
                    if (_commentService.LikeComment(_commentData.Id))
                    {
                        // Increment the like count in the UI
                        LikeButton.IncrementLikeCount();
                        
                        // Update the model
                        _commentData.LikeCount++;
                        
                        // Raise the CommentLiked event
                        CommentLiked?.Invoke(this, new CommentLikedEventArgs(_commentData.Id));
                        
                        System.Diagnostics.Debug.WriteLine($"Comment liked: ID {_commentData.Id}, new count: {_commentData.LikeCount}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error liking comment: {ex.Message}");
                }
            }
        }
        
        private void ChildComment_CommentLiked(object sender, CommentLikedEventArgs e)
        {
            // Forward the event up
            CommentLiked?.Invoke(this, e);
        }
        
        private void CommentReplyButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle reply input visibility
            if (ReplyInputControl.Visibility == Visibility.Visible)
            {
                HideReplyInput();
            }
            else
            {
                ShowReplyInput();
            }
        }
        
        private void ShowReplyInput()
        {
            ReplyInputControl.Visibility = Visibility.Visible;
        }
        
        private void HideReplyInput()
        {
            ReplyInputControl.Visibility = Visibility.Collapsed;
            ReplyInputControl.ClearComment();
        }
        
        private void ReplyInput_CommentSubmitted(object sender, RoutedEventArgs e)
        {
            // Get the comment text from the reply input
            string replyText = ReplyInputControl.CommentText;
            
            if (string.IsNullOrWhiteSpace(replyText))
                return;
                
            // Raise the reply submitted event with the parent comment ID
            ReplySubmitted?.Invoke(this, new CommentReplyEventArgs(_commentData.Id, replyText));
            
            // Hide the reply input after submission
            HideReplyInput();
        }
        
        private void ChildComment_ReplySubmitted(object sender, CommentReplyEventArgs e)
        {
            // Don't forward the event again, just handle it here
            // This prevents duplicate event handling when replying to nested comments
            
            // Log the event but don't forward it
            System.Diagnostics.Debug.WriteLine($"Detected reply to child comment {e.ParentCommentId} in parent comment {_commentData.Id}");
            
            // Stop event propagation by not calling ReplySubmitted?.Invoke
            // Only allow the original comment component to raise the event up to the page level
            if (sender is Comment childComment && childComment != this)
            {
                // Hide any open reply input on this level too
                HideReplyInput();
                
                // Do not forward this event upward - this prevents duplicates
                return;
            }
            
            // Only the original comment component should forward the event
            if (sender == this)
            {
                // Forward the event up to be handled at the top level
                ReplySubmitted?.Invoke(this, e);
            }
        }

        private string FormatDate(DateTime date)
        {
            // For simplicity, just display the short date
            return date.ToString("MMM dd");
        }
    }
    
    // Event args class for comment replies
    public class CommentReplyEventArgs : EventArgs
    {
        public int ParentCommentId { get; private set; }
        public string CommentText { get; private set; }
        
        public CommentReplyEventArgs(int parentCommentId, string commentText)
        {
            ParentCommentId = parentCommentId;
            CommentText = commentText;
        }
    }
    
    // Event args class for comment likes
    public class CommentLikedEventArgs : EventArgs
    {
        public int CommentId { get; private set; }
        
        public CommentLikedEventArgs(int commentId)
        {
            CommentId = commentId;
        }
    }
} 