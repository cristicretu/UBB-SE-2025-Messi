using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Input;

namespace Duo.Views.Components
{
    public sealed partial class PostComponent : UserControl, INotifyPropertyChanged
    {
        // Property change notification
        public event PropertyChangedEventHandler? PropertyChanged;

        // Username property
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(PostComponent), new PropertyMetadata("u/username"));

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        // Date property
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(string), typeof(PostComponent), new PropertyMetadata("2023-03-12"));

        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        // Title property
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PostComponent), 
                new PropertyMetadata("No hardware errors detected - what could it be?"));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Content property
        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register("ContentText", typeof(string), typeof(PostComponent), 
                new PropertyMetadata("Any Macheads have an idea what could cause the following on a MPB M1 Max with NO hardware issues detected using Apple's hardware test tool screen rapidly flickers brightness when under RAM/CPU pressure screen turns full black and then back on under sam login screen goes black, flashes, & resets to the previous after choosing account (e.g. I go from one account, to the account picker, click one account and begin to type in the password — and then it suddenly flashes black, jitters, and resets to the account picker again shutting off w/o battery warning (battery health is good) freezes sometimes (way more than I'm used to won't let me switch apps sometimes (while other things are still clickable, the menu bar will change to the app I switched to, but the menu isn't clickable; switching to another app, the menu works again graphics for apps just don't update sometime generally incredibly slow at times (compared to my husband's MPB M1 Max with, admittedly, more RAM)"));

        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        // Likes counter
        private int _likesCount = 0;
        public int LikesCount
        {
            get => _likesCount;
            set
            {
                if (_likesCount != value)
                {
                    _likesCount = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(LikesText));
                }
            }
        }

        // Likes text
        public string LikesText => $"{LikesCount} likes";

        // Is Liked
        private bool _isLiked = false;
        public bool IsLiked
        {
            get => _isLiked;
            set
            {
                if (_isLiked != value)
                {
                    _isLiked = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(HeartColor));
                }
            }
        }

        // Heart color based on like state
        public SolidColorBrush HeartColor => IsLiked ? 
            new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : 
            new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public PostComponent()
        {
            this.InitializeComponent();
            this.DataContext = this;
            
            // Register pointer events for hover
            this.PointerEntered += PostComponent_PointerEntered;
            this.PointerExited += PostComponent_PointerExited;
        }
        
        // Handle pointer entered event
        private void PostComponent_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }
        
        // Handle pointer exited event
        private void PostComponent_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        // Handle like button click
        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            IsLiked = !IsLiked;
            
            // Update like count
            LikesCount += 1;
        }

        // Property changed notification
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
