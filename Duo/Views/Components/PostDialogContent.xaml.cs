using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Duo.Views.Components
{
    public sealed partial class PostDialogContent : UserControl, INotifyPropertyChanged
    {
        private string _postTitle = string.Empty;
        private string _postContent = string.Empty;
        private string _currentHashtag = string.Empty;
        private ObservableCollection<string> _hashtags = new ObservableCollection<string>();

        public string PostTitle
        {
            get => _postTitle;
            set
            {
                if (_postTitle != value)
                {
                    _postTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PostContent
        {
            get => _postContent;
            set
            {
                if (_postContent != value)
                {
                    _postContent = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentHashtag
        {
            get => _currentHashtag;
            set
            {
                if (_currentHashtag != value)
                {
                    _currentHashtag = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Hashtags => _hashtags;

        public bool HasHashtags => Hashtags.Count > 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public PostDialogContent()
        {
            this.InitializeComponent();
            Hashtags.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasHashtags));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddHashtagButton_Click(object sender, RoutedEventArgs e)
        {
            AddHashtag();
        }

        private void HashtagTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AddHashtag();
                e.Handled = true;
            }
        }

        private void AddHashtag()
        {
            string hashtag = CurrentHashtag.Trim();
            if (!string.IsNullOrEmpty(hashtag))
            {
                // Add # if not present
                if (!hashtag.StartsWith("#"))
                {
                    hashtag = "#" + hashtag;
                }

                // Add to collection if not a duplicate
                if (!Hashtags.Contains(hashtag))
                {
                    Hashtags.Add(hashtag);
                }

                // Clear the input
                CurrentHashtag = string.Empty;
            }
        }

        private void RemoveHashtag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string hashtag)
            {
                Hashtags.Remove(hashtag);
            }
        }
    }
}