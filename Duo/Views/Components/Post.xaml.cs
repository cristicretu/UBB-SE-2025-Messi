using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Duo.Views.Pages;
using Microsoft.UI.Xaml.Media;

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
                // Create a MockPost with the current post's data
                var post = new MockPost
                {
                    Title = this.Title,
                    Content = this.Content,
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
    }
} 