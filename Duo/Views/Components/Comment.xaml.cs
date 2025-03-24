using Duo.Models;
using Duo.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Duo.ViewModels;
using static Duo.App;

namespace Duo.Views.Components
{
    public sealed partial class Comment : UserControl
    {
        private Models.Comment _commentData;
        private Dictionary<int?, List<Models.Comment>> _commentsByParent;
        private readonly CommentService _commentService;
        private const int MAX_NESTING_LEVEL = 3; // Maximum allowed nesting level
        private bool _childrenVisible = true; // Track children visibility state
        
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
            
            // For each comment, we show indentation lines for all previous levels
            for (int i = 1; i <= comment.Level; i++)
            {
                indentationLevels.Add(i);
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
                
                // Show the toggle button only if this comment has children
                ToggleChildrenButton.Visibility = Visibility.Visible;
                // Set initial icon to minus (collapse) since children are visible by default
                ToggleIcon.Glyph = "\uE108"; // Minus icon
            }
            else
            {
                // Hide the toggle button if there are no children
                ToggleChildrenButton.Visibility = Visibility.Collapsed;
            }
            
            // Hide the reply input if visible
            HideReplyInput();
        }
        
        /// <summary>
        /// Sets the collapsed state of child comments programmatically
        /// </summary>
        /// <param name="collapsed">True to collapse children, false to expand</param>
        public void SetChildrenCollapsed(bool collapsed)
        {
            // Only proceed if we have children
            if (ChildCommentsPanel.Children.Count > 0)
            {
                _childrenVisible = !collapsed;
                ChildCommentsPanel.Visibility = _childrenVisible ? Visibility.Visible : Visibility.Collapsed;
                ToggleIcon.Glyph = _childrenVisible ? "\uE108" : "\uE109"; // Minus or Plus icon
            }
        }
        
        private void ToggleChildrenButton_Click(object sender, RoutedEventArgs e)
        {
            _childrenVisible = !_childrenVisible;
            
            // Update the visibility of child comments
            ChildCommentsPanel.Visibility = _childrenVisible ? Visibility.Visible : Visibility.Collapsed;
            
            // Change the icon based on state
            ToggleIcon.Glyph = _childrenVisible ? "\uE108" : "\uE109"; // Minus or Plus icon
            
            // Track the collapsed state in the PostDetailViewModel
            if (_commentData != null)
            {
                PostDetailViewModel.CollapsedComments[_commentData.Id] = !_childrenVisible;
                System.Diagnostics.Debug.WriteLine($"Comment {_commentData.Id} collapse state set to {!_childrenVisible}");
            }
        }
        
        private void LikeButton_LikeClicked(object sender, LikeButtonClickedEventArgs e)
        {
            if (e.TargetType == LikeTargetType.Comment && e.TargetId == _commentData.Id)
            {
                try
                {
                    if (_commentService.LikeComment(_commentData.Id))
                    {
                        LikeButton.LikeCount++;
                        
                        CommentLiked?.Invoke(this, new CommentLikedEventArgs(_commentData.Id));
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
            CommentLiked?.Invoke(this, e);
        }
        
        private void CommentReplyButton_Click(object sender, RoutedEventArgs e)
        {
            ShowReplyInput();
        }
        
        private void ReplyInput_CommentSubmitted(object sender, RoutedEventArgs e)
        {
            if (sender is Components.CommentInput commentInput && !string.IsNullOrWhiteSpace(commentInput.CommentText))
            {
                // Trigger the reply submitted event
                ReplySubmitted?.Invoke(this, new CommentReplyEventArgs(_commentData.Id, commentInput.CommentText));
                
                // Clear the reply input and hide it
                commentInput.ClearComment();
                HideReplyInput();
            }
        }
        
        private void ChildComment_ReplySubmitted(object sender, CommentReplyEventArgs e)
        {
            // Forward the reply event to the parent
            ReplySubmitted?.Invoke(this, e);
        }
        
        public void AddReplyComment(Models.Comment replyComment)
        {
            var childCommentControl = new Comment();
            childCommentControl.SetCommentData(replyComment, _commentsByParent);
            childCommentControl.ReplySubmitted += ChildComment_ReplySubmitted;
            childCommentControl.CommentLiked += ChildComment_CommentLiked;
            ChildCommentsPanel.Children.Add(childCommentControl);
            
            // Add the reply to the local comments dictionary
            if (!_commentsByParent.ContainsKey(_commentData.Id))
            {
                _commentsByParent[_commentData.Id] = new List<Models.Comment>();
            }
            
            _commentsByParent[_commentData.Id].Add(replyComment);
            
            // Ensure the toggle button is visible now that we have children
            ToggleChildrenButton.Visibility = Visibility.Visible;
            
            // Set the icon to minus (collapse) and make sure children are visible
            ToggleIcon.Glyph = "\uE108"; // Minus icon
            _childrenVisible = true;
            ChildCommentsPanel.Visibility = Visibility.Visible;
        }
        
        private void ShowReplyInput()
        {
            ReplyInputControl.Visibility = Visibility.Visible;
            ReplyInputControl.Focus(FocusState.Programmatic);
        }
        
        private void HideReplyInput()
        {
            ReplyInputControl.Visibility = Visibility.Collapsed;
            ReplyInputControl.ClearComment();
        }
        
        private string FormatDate(DateTime dateTime)
        {
            if (dateTime.Date == DateTime.Today)
            {
                return "Today";
            }
            else if (dateTime.Date == DateTime.Today.AddDays(-1))
            {
                return "Yesterday";
            }
            else if ((DateTime.Today - dateTime.Date).TotalDays < 7)
            {
                return dateTime.ToString("ddd"); // Day of week
            }
            else
            {
                return dateTime.ToString("MMM d"); // Month and day
            }
        }
    }
    
    // Event args for comment reply events
    public class CommentReplyEventArgs : EventArgs
    {
        public int ParentCommentId { get; private set; }
        public string ReplyText { get; private set; }
        
        public CommentReplyEventArgs(int parentCommentId, string replyText)
        {
            ParentCommentId = parentCommentId;
            ReplyText = replyText;
        }
    }
    
    // Event args for comment liked events
    public class CommentLikedEventArgs : EventArgs
    {
        public int CommentId { get; private set; }
        
        public CommentLikedEventArgs(int commentId)
        {
            CommentId = commentId;
        }
    }
} 