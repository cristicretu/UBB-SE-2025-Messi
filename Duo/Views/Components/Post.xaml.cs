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
using CommunityToolkit.WinUI.UI.Controls;

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

        public static readonly DependencyProperty IsAlwaysHighlightedProperty = 
            DependencyProperty.Register(nameof(IsAlwaysHighlighted), typeof(bool), typeof(Post), new PropertyMetadata(false, OnIsAlwaysHighlightedChanged));

        private static void OnIsAlwaysHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Post post)
            {
                post.UpdateHighlightState();
            }
        }

        private bool _isPointerOver;
        private LikeButton? _likeButton;

        public Post()
        {
            InitializeComponent();

            UpdateMoreOptionsVisibility();
            UpdateHighlightState();

            Loaded += Post_Loaded;
        }

        private void Post_Loaded(object sender, RoutedEventArgs e)
        {

            _likeButton = FindDescendant<LikeButton>(this);
            if (_likeButton != null)
            {
                _likeButton.LikeClicked += LikeButton_LikeClicked;
            }
        }

        private T FindDescendant<T>(DependencyObject parent) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T result)
                {
                    return result;
                }

                T descendant = FindDescendant<T>(child);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        private void LikeButton_LikeClicked(object sender, LikeButtonClickedEventArgs e)
        {
            if (e.TargetType == LikeTargetType.Post && e.TargetId == PostId)
            {
                try
                {
                    if (App._postService.LikePost(PostId))
                    {
                        if (_likeButton != null)
                        {
                            _likeButton.IncrementLikeCount();
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void UpdateMoreOptionsVisibility()
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser != null)
            {
                MoreOptions.Visibility = (this.Username == currentUser.Username) 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
            else
            {
                MoreOptions.Visibility = Visibility.Collapsed;
            }
        }

        private void PostBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isPointerOver = true;

            if (!IsAlwaysHighlighted) 
            {
                if (sender is Border border)
                {
                    border.Background = Application.Current.Resources["SystemControlBackgroundAltHighBrush"] as Microsoft.UI.Xaml.Media.Brush;
                    border.BorderBrush = Application.Current.Resources["SystemControlBackgroundListLowBrush"] as Microsoft.UI.Xaml.Media.Brush;
                }
            }
        }

        private void PostBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isPointerOver = false;

            if (!IsAlwaysHighlighted) 
            {
                if (sender is Border border)
                {
                    border.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Transparent);
                    border.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Transparent);
                }
            }
        }

        private void PostBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsAlwaysHighlighted)
            {
                return;
            }

            if (IsLikeButtonTap(e.OriginalSource as DependencyObject))
            {

                return;
            }

            var frame = FindParentFrame();
            if (frame != null)
            {

                var post = new Models.Post
                {
                    Title = this.Title ?? string.Empty,
                    Description = this.Content ?? string.Empty,
                    Username = this.Username,
                    Date = this.Date,
                    LikeCount = this.LikeCount
                };

                if (this.Hashtags != null)
                {
                    foreach (var hashtag in this.Hashtags)
                    {
                        post.Hashtags.Add(hashtag);
                    }
                }

                frame.Navigate(typeof(PostDetailPage), post);
            }
        }

        private bool IsLikeButtonTap(DependencyObject element)
        {

            if (element == null)
                return false;

            if (element is LikeButton)
                return true;

            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null && !(parent is Post))
            {
                if (parent is LikeButton)
                    return true;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return false;
        }

        private Frame FindParentFrame()
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is Frame))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as Frame;
        }

        private async void MoreOptions_EditClicked(object sender, RoutedEventArgs e)
        {

            if(this.Username != $"{userService.GetCurrentUser().Username}")
            {

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

            bool succesfullyEdited = true; 

            if (succesfullyEdited)
            {

                ContentDialog successDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Edited",
                    Content = "The item has been successfully edited.",
                    CloseButtonText = "OK"
                };
                await successDialog.ShowAsync();
            } else {

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
            if(this.Username != $"{userService.GetCurrentUser().Username}")
            {

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

            var deleteDialog = new DialogComponent();

            bool isConfirmed = await deleteDialog.ShowConfirmationDialog(
                "Confirm Deletion",
                "Are you sure you want to delete this item?",
                this.XamlRoot
            );

            if (isConfirmed)
            {

                try {
                _postService.DeletePost(this.PostId);
                }
                catch (Exception ex)
                {

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

        private void MarkdownText_MarkdownRendered(object sender, CommunityToolkit.WinUI.UI.Controls.MarkdownRenderedEventArgs e)
        {

        }

        private void MarkdownText_LinkClicked(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
        {

            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri? uri))
            {

                Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        private void UpdateHighlightState()
        {
            if (PostBorder != null)
            {
                if (IsAlwaysHighlighted)
                {
                    PostBorder.Background = Application.Current.Resources["SystemControlBackgroundAltHighBrush"] as Microsoft.UI.Xaml.Media.Brush;
                    PostBorder.BorderBrush = Application.Current.Resources["SystemControlBackgroundListLowBrush"] as Microsoft.UI.Xaml.Media.Brush;
                }
                else if (!_isPointerOver) 
                {
                    PostBorder.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Transparent);
                    PostBorder.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Transparent);
                }
            }
        }

        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set 
            { 
                SetValue(UsernameProperty, value);

                UpdateMoreOptionsVisibility();
            }
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
            get { return (int)GetValue(PostIdProperty); }
            set { SetValue(PostIdProperty, value); }
        }

        public bool IsAlwaysHighlighted
        {
            get { return (bool)GetValue(IsAlwaysHighlightedProperty); }
            set { SetValue(IsAlwaysHighlightedProperty, value); }
        }
    }
} 