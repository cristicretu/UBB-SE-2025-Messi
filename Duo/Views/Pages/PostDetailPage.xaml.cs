using Duo.Models;
using Duo.Services;
using Duo.Views.Components;
using Duo.Repositories;
using Duo.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duo.ViewModels;

// is this the right way to access userService and its methods?
using static Duo.App;

namespace Duo.Views.Pages
{
    public sealed partial class PostDetailPage : Page
    {
        private readonly CommentService _commentService;
        
        public PostDetailPage()
        {
            this.InitializeComponent();
            
            _commentService = new CommentService(_commentRepository, _postRepository, userService);
            
            ViewModel.CommentsPanel = CommentsPanel;
            
            ViewModel.CommentsLoaded += ViewModel_CommentsLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Models.Post post && post.Id > 0)
            {
                // Load data using the ViewModel - this will set the Post property internally
                ViewModel.LoadPostDetails(post.Id);
            }
            else
            {
                // Handle invalid post case
                TextBlock errorText = new TextBlock
                {
                    Text = "Invalid post data received",
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
                    Margin = new Thickness(0, 20, 0, 0)
                };
                CommentsPanel.Children.Add(errorText);
            }
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        
        private void CommentInputControl_CommentSubmitted(object sender, RoutedEventArgs e)
        {
            if (sender is Components.CommentInput commentInput && commentInput.CommentText != null)
            {
                if (ViewModel.AddCommentCommand.CanExecute(commentInput.CommentText))
                {
                    ViewModel.AddCommentCommand.Execute(commentInput.CommentText);
                    
                    // Clear the comment input after submission
                    commentInput.ClearComment();
                }
            }
        }
        
        private void ViewModel_CommentsLoaded(object sender, EventArgs e)
        {
            ConnectCommentReplyEvents();
            
            ConnectCommentLikeEvents();
        }
        
        private void ConnectCommentReplyEvents()
        {
            DisconnectCommentReplyEvents();
            
            var commentControls = GetAllChildrenOfType<Components.Comment>(CommentsPanel);
            
            foreach (var comment in commentControls)
            {
                comment.ReplySubmitted += Comment_ReplySubmitted;
            }
        }
        
        private void DisconnectCommentReplyEvents()
        {
            var commentControls = GetAllChildrenOfType<Components.Comment>(CommentsPanel);
            foreach (var comment in commentControls)
            {
                comment.ReplySubmitted -= Comment_ReplySubmitted;
            }
        }
        
        private void ConnectCommentLikeEvents()
        {
            // Disconnect existing events first to avoid duplicates
            DisconnectCommentLikeEvents();
            
            // Get all comment components from the comments panel
            var commentControls = GetAllChildrenOfType<Components.Comment>(CommentsPanel);
            
            foreach (var comment in commentControls)
            {
                comment.CommentLiked += Comment_CommentLiked;
            }
        }
        
        private void DisconnectCommentLikeEvents()
        {
            var commentControls = GetAllChildrenOfType<Components.Comment>(CommentsPanel);
            foreach (var comment in commentControls)
            {
                comment.CommentLiked -= Comment_CommentLiked;
            }
        }
        
        private void Comment_ReplySubmitted(object sender, CommentReplyEventArgs e)
        {
            // Add debugging to track the event source
            System.Diagnostics.Debug.WriteLine($"Reply submitted event received from {sender.GetType().Name} for parent comment ID: {e.ParentCommentId}");
            
            // Add a reply to the comment
            ViewModel.AddReplyToComment(e.ParentCommentId, e.CommentText);
        }
        
        private void Comment_CommentLiked(object sender, CommentLikedEventArgs e)
        {
            // Handle comment liked event (if needed for analytics, etc.)
            System.Diagnostics.Debug.WriteLine($"Comment liked event received for ID: {e.CommentId}");
        }
        
        // Helper method to get all children of a specific type from a panel
        private List<T> GetAllChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            var list = new List<T>();
            var count = VisualTreeHelper.GetChildrenCount(parent);
            
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is T childOfType)
                {
                    list.Add(childOfType);
                }
                
                // Recursively search child elements
                if (child is FrameworkElement)
                {
                    var childrenOfType = GetAllChildrenOfType<T>(child);
                    if (childrenOfType.Any())
                    {
                        list.AddRange(childrenOfType);
                    }
                }
            }
            
            return list;
        }
    }
}
//* Loading Process

// When navigating to the detail page:
//    - The CommentService provides all comments for a post
//    - Comments are displayed in the UI through Comment components
//    - Child comments can be added recursively through the Comment component

// This structure allows for a clean display of comments in the post detail page.