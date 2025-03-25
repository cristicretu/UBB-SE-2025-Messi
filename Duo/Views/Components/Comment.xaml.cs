using Duo.Models;
using Duo.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using Duo.ViewModels;
using static Duo.App;

namespace Duo.Views.Components
{
    public sealed partial class Comment : UserControl
    {
        // For backward compatibility
        private Models.Comment _commentData;
        private Dictionary<int?, List<Models.Comment>> _commentsByParent;
        private readonly CommentService _commentService;
        private const int MAX_NESTING_LEVEL = 3; 

        public event EventHandler<CommentReplyEventArgs> ReplySubmitted;
        public event EventHandler<CommentLikedEventArgs> CommentLiked;

        public event EventHandler<CommentDeletedEventArgs> CommentDeleted;

        public CommentViewModel ViewModel => DataContext as CommentViewModel;

        public Comment()
        {
            this.InitializeComponent();

            _commentService = new CommentService(_commentRepository, _postRepository, userService);

            CommentReplyButton.Click += CommentReplyButton_Click;
            LikeButton.LikeClicked += LikeButton_LikeClicked;
            ReplyInputControl.CommentSubmitted += ReplyInput_CommentSubmitted;
            
            this.DataContextChanged += Comment_DataContextChanged;
            
            // Set up the element factory for the ChildCommentsRepeater
            ChildCommentsRepeater.ElementPrepared += ChildCommentsRepeater_ElementPrepared;
        }

        private void ChildCommentsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
        {
            if (args.Element is ContentPresenter presenter)
            {
                var index = args.Index;
                if (ViewModel?.Replies != null && index < ViewModel.Replies.Count)
                {
                    var childViewModel = ViewModel.Replies[index];
                    
                    // Create a Comment control for this child
                    var childComment = new Comment
                    {
                        DataContext = childViewModel,
                        Margin = new Thickness(0, 4, 0, 0)
                    };
                    
                    // Wire up events
                    childComment.ReplySubmitted += ChildComment_ReplySubmitted;
                    childComment.CommentLiked += ChildComment_CommentLiked;
                    childComment.CommentDeleted += ChildComment_CommentDeleted;

                    // Set the content of the presenter
                    presenter.Content = childComment;
                }
            }
        }

        private void Comment_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ViewModel != null)
            {
                // Set up level lines for indentation
                var indentationLevels = new List<int>();
                for (int i = 1; i <= ViewModel.Level; i++)
                {
                    indentationLevels.Add(i);
                }
                LevelLinesRepeater.ItemsSource = indentationLevels;

                // Handle reply button visibility
                CommentReplyButton.Visibility = (ViewModel.Level >= MAX_NESTING_LEVEL) 
                    ? Visibility.Collapsed 
                    : Visibility.Visible;

                // Handle toggle button visibility
                ToggleChildrenButton.Visibility = (ViewModel.Replies != null && ViewModel.Replies.Count > 0)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                try
                {
                    var currentUser = userService.GetCurrentUser();
                    if (currentUser != null && currentUser.UserId == ViewModel.UserId)
                    {
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception)
                {
                    DeleteButton.Visibility = Visibility.Collapsed;
                }

                // Set initial toggle button state
                if (ToggleChildrenButton.Visibility == Visibility.Visible)
                {
                    ToggleIcon.Glyph = ViewModel.IsExpanded ? "\uE108" : "\uE109";
                }
            }
        }

        // For backward compatibility
        public void SetCommentData(Models.Comment comment, Dictionary<int?, List<Models.Comment>> commentsByParent)
        {
            _commentData = comment;
            _commentsByParent = commentsByParent;
            
            var convertedDictionary = commentsByParent.Where(kvp => kvp.Key.HasValue)
                                             .ToDictionary(kvp => kvp.Key.Value, kvp => kvp.Value);
            
            var viewModel = new CommentViewModel(comment, convertedDictionary);
            this.DataContext = viewModel;

          
            
            if (commentsByParent.ContainsKey(comment.Id))
            {
                ChildCommentsPanel.Children.Clear();
                
                foreach (var childComment in commentsByParent[comment.Id])
                {
                    var childCommentControl = new Comment();
                    childCommentControl.SetCommentData(childComment, commentsByParent);
                    childCommentControl.ReplySubmitted += ChildComment_ReplySubmitted;
                    childCommentControl.CommentLiked += ChildComment_CommentLiked;
                    ChildCommentsPanel.Children.Add(childCommentControl);
                }
            }
        }

        // For backward compatibility
        public void SetChildrenCollapsed(bool collapsed)
        {
            if (ViewModel != null)
            {
                ViewModel.IsExpanded = !collapsed;
            }
        }

        private void ToggleChildrenButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.IsExpanded = !ViewModel.IsExpanded;
                ToggleIcon.Glyph = ViewModel.IsExpanded ? "\uE108" : "\uE109";
            }
        }

        private void LikeButton_LikeClicked(object sender, LikeButtonClickedEventArgs e)
        {
            if (ViewModel != null && e.TargetType == LikeTargetType.Comment && e.TargetId == ViewModel.Id)
            {
                // Use the CommentLiked event to bubble up to the PostDetailPage
                CommentLiked?.Invoke(this, new CommentLikedEventArgs(ViewModel.Id));
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

        private void ReplyInput_CommentSubmitted(object sender, EventArgs e)
        {
            if (ViewModel == null) return;
            
            if (sender is CommentInput commentInput && !string.IsNullOrWhiteSpace(commentInput.CommentText))
            {
                ReplySubmitted?.Invoke(this, new CommentReplyEventArgs(ViewModel.Id, commentInput.CommentText));
                commentInput.ClearComment();
                HideReplyInput();
            }
        }

        private void ChildComment_ReplySubmitted(object sender, CommentReplyEventArgs e)
        {
            ReplySubmitted?.Invoke(this, e);
        }

        private void ChildComment_CommentDeleted(object sender, CommentDeletedEventArgs e)
         {
             CommentDeleted?.Invoke(this, e);
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
         {
             ShowDeleteConfirmation();
         }

         private async void ShowDeleteConfirmation()
         {
             var dialog = new ContentDialog
             {
                 Title = "Delete Comment",
                 Content = "Are you sure you want to delete this comment? This action cannot be undone.",
                 PrimaryButtonText = "Delete",
                 CloseButtonText = "Cancel",
                 DefaultButton = ContentDialogButton.Close
             };

             dialog.XamlRoot = this.XamlRoot;

             var result = await dialog.ShowAsync();

             if (result == ContentDialogResult.Primary)
             {
                 try
                 {
                     var currentUser = userService.GetCurrentUser();
                     if (currentUser != null)
                     {
                         // Get comment ID from either ViewModel or _commentData
                         int commentId = ViewModel != null ? ViewModel.Id : (_commentData != null ? _commentData.Id : -1);
                         
                         if (commentId > 0)
                         {
                             CommentDeleted?.Invoke(this, new CommentDeletedEventArgs(commentId));
                         }
                         else
                         {
                             System.Diagnostics.Debug.WriteLine("Error: Could not determine comment ID for deletion");
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     System.Diagnostics.Debug.WriteLine($"Error deleting comment: {ex.Message}");
                 }
             }
         }
    }

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

    public class CommentLikedEventArgs : EventArgs
    {
        public int CommentId { get; private set; }

        public CommentLikedEventArgs(int commentId)
        {
            CommentId = commentId;
        }
    }

    public class CommentDeletedEventArgs : EventArgs
     {
         public int CommentId { get; private set; }

         public CommentDeletedEventArgs(int commentId)
         {
             CommentId = commentId;
         }
     }
} 