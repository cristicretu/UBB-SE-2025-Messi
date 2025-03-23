using Duo.Models;
using Duo.Services;
using Duo.Views.Components;
using Duo.Repositories;
using Duo.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// is this the right way to access userService and its methods?
using static Duo.App;

namespace Duo.Views.Pages
{
    public sealed partial class PostDetailPage : Page
    {
        private Models.Post _post;
        private string _username; // Store username for the post

        public PostDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Models.Post post)
            {
                _post = post;
                
                // Get username for this post
                try 
                {
                    var user = userService.GetUserById(_post.UserID);
                    _username = user.Username;
                }
                catch
                {
                    _username = "Unknown User";
                }
                
                DisplayPostDetails();
                LoadComments();
            }
        }

        private async void MoreOptions_EditClicked(object sender, RoutedEventArgs e)
        {
            // Get the current user
            var currentUser = userService.GetCurrentUser();
            
            // Verify that the current user is the owner of the post            
            if(_post.UserID != currentUser.UserId)
            {
                // Display an error dialog if the user is not the owner
                ContentDialog errorDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Error",
                    Content = "You do not have permission to edit this item.",
                    CloseButtonText = "OK"
                };
                await errorDialog.ShowAsync();
                return;
            }

            // Handle the edit logic here

            bool succesfullyEdited = true; // Placeholder for actual success event
            
            if (succesfullyEdited)
            {
                // Send/confirm
                ContentDialog successDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Edited",
                    Content = "The item has been successfully edited.",
                    CloseButtonText = "OK"
                };
                await successDialog.ShowAsync();
            } else {
                // Handle the error logic here
                ContentDialog errorDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Error",
                    Content = "An error occurred while editing the item. Please try again.",
                    CloseButtonText = "OK"
                };
                await errorDialog.ShowAsync();
            }
        }

        private async void MoreOptions_DeleteClicked(object sender, RoutedEventArgs e)
        {
            // Get the current user
            var currentUser = userService.GetCurrentUser();
            
            // Verify that the current user is the owner of the post
            if(_post.UserID != currentUser.UserId)
            {
                // Display an error dialog if the user is not the owner
                ContentDialog errorDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Error",
                    Content = "You do not have permission to delete this item.",
                    CloseButtonText = "OK"
                };
                await errorDialog.ShowAsync();
                return;
            }

            // Instantiate a DeleteDialog
            var deleteDialog = new DialogComponent();

            // Create the deletion confirmation (add whatever text you wish to display)
            bool isConfirmed = await deleteDialog.ShowConfirmationDialog(
                "Confirm Deletion",
                "Are you sure you want to delete this item?",
                this.XamlRoot
            );

            // if User confirms...
            if (isConfirmed)
            {
                // Handle the deletion logic here
                try {
                _postService.DeletePost(_post.Id);
                }
                catch (Exception ex)
                {
                    // Handle the error logic here
                    ContentDialog errorDialog = new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        Title = "Error",
                        Content = "An error occurred while deleting the item. Please try again.\n" + ex.Message,
                        CloseButtonText = "OK"
                    };
                    await errorDialog.ShowAsync();
                    return;
                }

                ContentDialog successDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Deleted",
                    Content = "The item has been successfully deleted.",
                    CloseButtonText = "OK"
                };
                await successDialog.ShowAsync();
            }
        }

        private void DisplayPostDetails()
        {
            // Set post details
            PostTitleTextBlock.Text = _post.Title;
            UserTextBlock.Text = $"u/{_username}";
            ContentTextBlock.Text = _post.Description;
            
            // Set hashtags - requires retrieving associated hashtags
            var hashtags = _postService.GetHashtagsByPostId(_post.Id);
            HashtagsItemsControl.ItemsSource = hashtags;
            
            // Format the date
            DateTextBlock.Text = FormatDate(_post.CreatedAt);

            // Set like button state - default to not liked
            LikeButton.IsLiked = false; // This would need to be retrieved from a likes service
            LikeButton.LikeCount = _post.LikeCount;
        }

        private void LoadComments()
        {
            // Clear existing comments
            CommentsPanel.Children.Clear();

            try
            {
                // Get comments for this post using service
                // Use global service instances from App class
                var commentService = new CommentService(_commentRepository, _postRepository, userService);
                
                var comments = commentService.GetCommentsByPostId(_post.Id);
                
                // Process comments and add them to the UI
                if (comments != null && comments.Any())
                {
                    // Find top-level comments (those without parent comments)
                    var topLevelComments = comments.Where(c => c.ParentCommentId == null).ToList();
                    
                    foreach (var comment in topLevelComments)
                    {
                        var commentComponent = new Views.Components.Comment();
                        // Create a dictionary for replies
                        var repliesByParentId = comments
                            .Where(c => c.ParentCommentId != null)
                            .GroupBy(c => c.ParentCommentId)
                            .ToDictionary(g => g.Key, g => g.ToList());
                            
                        commentComponent.SetCommentData(comment, repliesByParentId);
                        CommentsPanel.Children.Add(commentComponent);
                    }
                }
            }
            catch (Exception ex)
            {
                // Add error handling UI if needed
                TextBlock errorText = new TextBlock
                {
                    Text = "Could not load comments. " + ex.Message,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red)
                };
                CommentsPanel.Children.Add(errorText);
            }
        }

        private string FormatDate(DateTime date)
        {
            // For simplicity, just display the short date
            return date.ToString("MMM dd");
        }

        private string FormatDate(string dateStr)
        {
            // If we receive a string date (from older code), just return it
            return dateStr;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}
//* Loading Process

// When navigating to the detail page:
//    - The CommentService provides all comments for a post
//    - Comments are displayed in the UI through Comment components
//    - Child comments can be added recursively through the Comment component

// This structure allows for a clean display of comments in the post detail page.