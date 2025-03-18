using Duo.Models;
using Duo.Services;
using Duo.Views.Components;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Duo.Views.Pages
{
    public sealed partial class PostDetailPage : Page
    {
        private MockPost _post;

        public PostDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MockPost post)
            {
                _post = post;
                DisplayPostDetails();
                LoadComments();
            }
        }

        private async void MoreOptions_EditClicked(object sender, RoutedEventArgs e)
        {
            // Handle the edit logic here

            bool succesfullyEdited = true; // Placeholder for actual logic
            
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
                // Send/confirm
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
            UserTextBlock.Text = $"u/{_post.User}";
            ContentTextBlock.Text = _post.Content;
            
            // Set hashtags
            HashtagsItemsControl.ItemsSource = _post.Hashtags;
            
            // Use PostDate if it's set to a non-default value, otherwise use the Date string
            if (_post.PostDate != default)
            {
                DateTextBlock.Text = FormatDate(_post.PostDate);
            }
            else
            {
                DateTextBlock.Text = FormatDate(_post.Date);
            }

            // Set like button state
            LikeButton.IsLiked = _post.IsLiked;
            LikeButton.LikeCount = _post.LikeCount;
        }

        private void LoadComments()
        {
            // Clear existing comments
            CommentsPanel.Children.Clear();

            // Get mock comments for this post
            var comments = MockCommentService.GetCommentsForPost(_post.Id);
            var commentsByParent = MockCommentService.GroupCommentsByParent(comments);

            // Only show top-level comments (ParentId = -1) in the main list
            if (commentsByParent.ContainsKey(-1))
            {
                foreach (var mockComment in commentsByParent[-1])
                {
                    var commentComponent = new Views.Components.Comment();
                    commentComponent.SetCommentData(mockComment, commentsByParent);
                    CommentsPanel.Children.Add(commentComponent);
                }
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
//    - The MockCommentService provides all comments for a post
//    - Comments are grouped by parent ID using GroupCommentsByParent
//    - Only top-level comments are added directly to the page
//    - Each comment component recursively adds its own child comments

// This recursive structure allows unlimited nesting depth while keeping the code clean and maintainable.

// The result is a nested comment thread that visually shows the conversation hierarchy 
// through indentation and vertical lines, similar to modern discussion platforms like Reddit.