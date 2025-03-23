using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace Duo.Views.Components
{
    public sealed partial class LikeButton : UserControl
    {
        public event EventHandler<LikeButtonClickedEventArgs> LikeClicked;
        
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
        
        // Post ID Property
        public static readonly DependencyProperty PostIdProperty =
            DependencyProperty.Register(nameof(PostId), typeof(int), typeof(LikeButton), new PropertyMetadata(0));

        public int PostId
        {
            get { return (int)GetValue(PostIdProperty); }
            set { SetValue(PostIdProperty, value); }
        }
        
        // Comment ID Property
        public static readonly DependencyProperty CommentIdProperty =
            DependencyProperty.Register(nameof(CommentId), typeof(int), typeof(LikeButton), new PropertyMetadata(0));

        public int CommentId
        {
            get { return (int)GetValue(CommentIdProperty); }
            set { SetValue(CommentIdProperty, value); }
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
                
                LikeButtonClickedEventArgs args;
                
                if (PostId > 0)
                {
                    // This is a post like
                    args = new LikeButtonClickedEventArgs(LikeTargetType.Post, PostId);
                    System.Diagnostics.Debug.WriteLine($"Like button clicked for post ID: {PostId}");
                }
                else if (CommentId > 0)
                {
                    // This is a comment like
                    args = new LikeButtonClickedEventArgs(LikeTargetType.Comment, CommentId);
                    System.Diagnostics.Debug.WriteLine($"Like button clicked for comment ID: {CommentId}");
                }
                else
                {
                    // Neither post nor comment ID set
                    System.Diagnostics.Debug.WriteLine("Like button clicked but no ID set");
                    return;
                }
                
                // Raise the like clicked event
                LikeClicked?.Invoke(this, args);
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
        
        // Method that can be called to increment the like count (from parent components)
        public void IncrementLikeCount()
        {
            LikeCount++;
        }
    }
    
    // Event args for the LikeClicked event
    public class LikeButtonClickedEventArgs : EventArgs
    {
        public LikeTargetType TargetType { get; }
        public int TargetId { get; }
        
        public LikeButtonClickedEventArgs(LikeTargetType targetType, int targetId)
        {
            TargetType = targetType;
            TargetId = targetId;
        }
    }
    
    // Enum to indicate whether the like is for a post or comment
    public enum LikeTargetType
    {
        Post,
        Comment
    }
}