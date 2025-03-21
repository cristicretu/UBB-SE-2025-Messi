using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// is this the right way to access userService and its methods?
using static Duo.App;
using Duo.Views.Pages;
namespace Duo.Views.Components
{
    public sealed partial class Post : UserControl
    {
        public static readonly DependencyProperty UsernameProperty = 
            DependencyProperty.Register(nameof(Username), typeof(string), typeof(Post), new PropertyMetadata(""));
        
        public static readonly DependencyProperty DateProperty = 
            DependencyProperty.Register(nameof(Date), typeof(string), typeof(Post), new PropertyMetadata(""));
        
        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Post), new PropertyMetadata(""));
        
        public static new readonly DependencyProperty ContentProperty = 
            DependencyProperty.Register(nameof(Content), typeof(string), typeof(Post), new PropertyMetadata(""));
        
        public static readonly DependencyProperty LikeCountProperty = 
            DependencyProperty.Register(nameof(LikeCount), typeof(int), typeof(Post), new PropertyMetadata(0));
        
        public static readonly DependencyProperty HashtagsProperty = 
            DependencyProperty.Register(nameof(Hashtags), typeof(IEnumerable<string>), typeof(Post), new PropertyMetadata(null));

        public static readonly DependencyProperty PostIdProperty = 
            DependencyProperty.Register(nameof(PostId), typeof(int), typeof(Post), new PropertyMetadata(0));

        public Post()
        {
            InitializeComponent();
        }

        // Handle pointer entered event for hover effects
        private void PostBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = Application.Current.Resources["SystemControlBackgroundAltHighBrush"] as Microsoft.UI.Xaml.Media.Brush;
                border.BorderBrush = Application.Current.Resources["SystemControlBackgroundListLowBrush"] as Microsoft.UI.Xaml.Media.Brush;
            }
        }

        // Handle pointer exited event for hover effects
        private void PostBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Transparent);
                border.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Transparent);
            }
        }

        // Handle tapped event for navigation
        private void PostBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Check if the tap originated from a LikeButton or its children
            if (IsLikeButtonTap(e.OriginalSource as DependencyObject))
            {
                // Skip navigation if the tap was on the like button
                return;
            }
            
            // Get the parent frame for navigation
            var frame = FindParentFrame();
            if (frame != null)
            {
                // Create a Post with the current post's data
                var post = new Models.Post
                {
                    Title = this.Title,
                    Description = this.Content,
                    Username = this.Username,
                    Date = this.Date,
                    LikeCount = this.LikeCount
                };

                // Copy hashtags
                if (this.Hashtags != null)
                {
                    foreach (var hashtag in this.Hashtags)
                    {
                        post.Hashtags.Add(hashtag);
                    }
                }

                // Navigate to the post detail page
                frame.Navigate(typeof(PostDetailPage), post);
            }
        }
        
        // Helper method to determine if a tap originated from the LikeButton
        private bool IsLikeButtonTap(DependencyObject element)
        {
            // If null, it can't be the LikeButton
            if (element == null)
                return false;
                
            // Check if the element is a LikeButton
            if (element is LikeButton)
                return true;
                
            // Check parent hierarchy
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null && !(parent is Post))
            {
                if (parent is LikeButton)
                    return true;
                    
                parent = VisualTreeHelper.GetParent(parent);
            }
            
            return false;
        }

        // Helper method to find the parent Frame
        private Frame FindParentFrame()
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is Frame))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as Frame;
        }

        // Event handlers for the MoreDropdown component
        private async void MoreOptions_EditClicked(object sender, RoutedEventArgs e)
        {
            // Verify that the current user is the owner of the post            
            if(this.Username != $"u/{userService.GetCurrentUser().Username}")
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
            // Verify that the current user is the owner of the post
            if(this.Username != $"u/{userService.GetCurrentUser().Username}")
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

        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public string Date
        {
            get => (string)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public new string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public int LikeCount
        {
            get => (int)GetValue(LikeCountProperty);
            set => SetValue(LikeCountProperty, value);
        }

        public IEnumerable<string> Hashtags
        {
            get => (IEnumerable<string>)GetValue(HashtagsProperty);
            set => SetValue(HashtagsProperty, value);
        }

        public int PostId
        {
            get => (int)GetValue(PostIdProperty);
            set => SetValue(PostIdProperty, value);
        }
    }
} 