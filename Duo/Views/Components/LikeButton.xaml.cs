using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;

namespace Duo.Views.Components
{
    public sealed partial class LikeButton : UserControl
    {
        private SolidColorBrush _heartDefaultBrush;
        private SolidColorBrush _heartHoverBrush;
        private SolidColorBrush _heartFilledBrush;

        public LikeButton()
        {
            this.InitializeComponent();
            
            // Store brushes for easy access
            _heartDefaultBrush = Resources["HeartDefaultBrush"] as SolidColorBrush;
            _heartHoverBrush = Resources["HeartHoverBrush"] as SolidColorBrush;
            _heartFilledBrush = Resources["HeartFilledBrush"] as SolidColorBrush;
            
            // Register event handlers for the icon's parent (button)
            likeButton.PointerEntered += LikeButton_PointerEntered;
            likeButton.PointerExited += LikeButton_PointerExited;
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
            DependencyProperty.Register(nameof(IsLiked), typeof(bool), typeof(LikeButton), new PropertyMetadata(false, OnIsLikedChanged));

        public bool IsLiked
        {
            get { return (bool)GetValue(IsLikedProperty); }
            set { SetValue(IsLikedProperty, value); }
        }

        private static void OnIsLikedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LikeButton;
            if (control != null)
            {
                control.UpdateHeartAppearance();
            }
        }

        private void UpdateHeartAppearance()
        {
            // Only update the heart icon, not the entire button
            if (IsLiked)
            {
                heartIcon.Foreground = _heartFilledBrush;
            }
            else
            {
                heartIcon.Foreground = _heartDefaultBrush;
            }
        }

        private void LikeButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!IsLiked)
            {
                // Only change the heart icon color
                heartIcon.Foreground = _heartHoverBrush;
            }
        }

        private void LikeButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!IsLiked)
            {
                // Reset to default color when not hovering
                heartIcon.Foreground = _heartDefaultBrush;
            }
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            LikeCount++;
        }
    }
}