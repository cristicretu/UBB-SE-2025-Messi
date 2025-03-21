using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using static Duo.App;

namespace Duo.Views.Components
{
    public sealed partial class LikeButton : UserControl
    {
        public LikeButton()
        {
            this.InitializeComponent();
        }

        // Like Count Property
        public static readonly DependencyProperty LikeCountProperty =
            DependencyProperty.Register(nameof(LikeCount), typeof(int), typeof(LikeButton), new PropertyMetadata(0));

        public int LikeCount
        {
            get { return (int)GetValue(LikeCountProperty); }
            set { SetValue(LikeCountProperty, value); }
        }

        // Is Liked Property
        public static readonly DependencyProperty IsLikedProperty =
            DependencyProperty.Register(nameof(IsLiked), typeof(bool), typeof(LikeButton), new PropertyMetadata(false));

        public bool IsLiked
        {
            get { return (bool)GetValue(IsLikedProperty); }
            set { SetValue(IsLikedProperty, value); }
        }
        
        public static readonly DependencyProperty PostIdProperty =
            DependencyProperty.Register(nameof(PostId), typeof(int), typeof(LikeButton), new PropertyMetadata(0));

        public int PostId
        {
            get { return (int)GetValue(PostIdProperty); }
            set { SetValue(PostIdProperty, value); }
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Play heart beat animation
                var heartAnimation = this.Resources["HeartAnimation"] as Storyboard;
                if (heartAnimation != null)
                {
                    heartAnimation.Begin();
                }
                
                if (PostId > 0)
                {
                    try 
                    {
                        var postService = new Duo.Services.PostService(_postRepository, _hashtagRepository, userService);
                        if (postService.LikePost(PostId))
                        {
                            LikeCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error liking post: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Log error or handle exception gracefully
                System.Diagnostics.Debug.WriteLine($"Error in LikeButton_Click: {ex.Message}");
            }
        }

        private void LikeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Mark the tapped event as handled to stop it from bubbling up
            e.Handled = true;
        }
    }
}