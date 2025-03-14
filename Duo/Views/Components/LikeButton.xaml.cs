using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

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

        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            LikeCount++;
        }
    }
}
