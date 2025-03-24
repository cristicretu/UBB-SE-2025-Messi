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
        private const int MAX_NESTING_LEVEL = 3; // Maximum allowed nesting level
        
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
            
            ReplyInputControl.CommentSubmitted += ReplyInput_CommentSubmitted;
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
            
            // For each comment, we need to show indentation lines for ALL levels
            // including its own level if it's not a top-level comment
            for (int i = 1; i <= comment.Level; i++)
            {
                if (i < comment.Level || comment.Level == 1)
                {
                    indentationLevels.Add(i);
                }
            }
            
            LevelLinesRepeater.ItemsSource = indentationLevels;

            // Hide reply button for comments at or beyond the max nesting level
            CommentReplyButton.Visibility = (comment.Level >= MAX_NESTING_LEVEL) 
                ? Visibility.Collapsed 
                : Visibility.Visible;

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
        
        private void IndentationLine_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the expanded state of this comment's replies
            ChildCommentsExpander.IsExpanded = !ChildCommentsExpander.IsExpanded;
            
            // If the sender is a button that contains a level value, we can get the level
            if (sender is Button button && button.DataContext is int level)
            {
                System.Diagnostics.Debug.WriteLine($"Toggled expansion for level {level} comment, ID: {_commentData.Id}");
            }
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
            System.Diagnostics.Debug.WriteLine($"Detected reply to child comment {e.ParentCommentId} in parent comment {_commentData.Id}");
            
            ReplySubmitted?.Invoke(this, e);
            
            HideReplyInput();
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