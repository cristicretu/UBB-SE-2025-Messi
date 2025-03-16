using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.UI.Xaml.Input;

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
        
        public static readonly DependencyProperty ContentProperty = 
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

        public string Content
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