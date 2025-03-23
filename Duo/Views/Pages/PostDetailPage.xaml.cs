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
using Duo.ViewModels;

// is this the right way to access userService and its methods?
using static Duo.App;

namespace Duo.Views.Pages
{
    public sealed partial class PostDetailPage : Page
    {
        public PostDetailPage()
        {
            this.InitializeComponent();
            
            // Connect the comments panel to the view model
            ViewModel.CommentsPanel = CommentsPanel;
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
            // Get the comment text from the CommentInput control
            if (sender is CommentInput commentInput && commentInput.CommentText != null)
            {
                // Call the ViewModel's AddComment method
                if (ViewModel.AddCommentCommand.CanExecute(commentInput.CommentText))
                {
                    ViewModel.AddCommentCommand.Execute(commentInput.CommentText);
                    
                    // Clear the comment input after submission
                    commentInput.ClearComment();
                }
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