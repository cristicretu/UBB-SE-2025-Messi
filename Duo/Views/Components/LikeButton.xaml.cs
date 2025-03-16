using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

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

        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            LikeCount++;
            IsLiked = true;
        }
    }
}