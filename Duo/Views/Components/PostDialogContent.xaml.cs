using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using Duo.Helpers;

namespace Duo.Views.Components
{
    public sealed partial class PostDialogContent : UserControl, INotifyPropertyChanged
    {
        private string _postTitle = string.Empty;
        private string _postContent = string.Empty;
        private string _currentHashtag = string.Empty;
        private ObservableCollection<string> _hashtags = new ObservableCollection<string>();

        // Validation state properties
        private bool _isTitleValid = true;
        private bool _isContentValid = true;
        private bool _isHashtagValid = true;

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

        #region Validation Methods
        
        private bool ValidateTitle()
        {
            HideError(TitleErrorTextBlock);
            
            var (isValid, errorMessage) = ValidationHelper.ValidatePostTitle(PostTitle);
            
            if (!isValid)
            {
                ShowError(TitleErrorTextBlock, errorMessage);
                return false;
            }
            
            return true;
        }

        private bool ValidateContent()
        {
            HideError(ContentErrorTextBlock);
            
            var (isValid, errorMessage) = ValidationHelper.ValidatePostContent(PostContent);
            
            if (!isValid)
            {
                ShowError(ContentErrorTextBlock, errorMessage);
                return false;
            }
            
            return true;
        }

        private bool ValidateHashtag(string hashtag)
        {
            HideError(HashtagErrorTextBlock);
            
            var (isValid, errorMessage) = ValidationHelper.ValidateHashtagInput(hashtag);
            
            if (!isValid)
            {
                ShowError(HashtagErrorTextBlock, errorMessage);
                return false;
            }
            
            return true;
        }
        
        private void ShowError(TextBlock errorTextBlock, string errorMessage)
        {
            errorTextBlock.Text = errorMessage;
            errorTextBlock.Visibility = Visibility.Visible;
        }
        
        private void HideError(TextBlock errorTextBlock)
        {
            errorTextBlock.Text = string.Empty;
            errorTextBlock.Visibility = Visibility.Collapsed;
        }
        
        public bool IsFormValid()
        {
            bool isTitleValid = ValidateTitle();
            bool isContentValid = ValidateContent();
            bool isHashtagValid = true;
            
            if (!string.IsNullOrEmpty(CurrentHashtag))
            {
                isHashtagValid = ValidateHashtag(CurrentHashtag);
            }
            
            return isTitleValid && isContentValid && isHashtagValid;
        }
        #endregion

        #region Event Handlers
        private void TitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _isTitleValid = ValidateTitle();
        }

        private void ContentTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _isContentValid = ValidateContent();
        }
        
        private void HashtagTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _isHashtagValid = ValidateHashtag(CurrentHashtag);
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
            
            if (string.IsNullOrEmpty(hashtag))
                return;
                
            if (!ValidateHashtag(hashtag))
                return;
            
            // // Add # if not present
            // if (!hashtag.StartsWith("#"))
            // {
            //     hashtag = "#" + hashtag;
            // }

            // Add to collection if not a duplicate
            if (!Hashtags.Contains(hashtag))
            {
                Hashtags.Add(hashtag);
            }

            // Clear the input
            CurrentHashtag = string.Empty;
            HideError(HashtagErrorTextBlock);
        }

        private void RemoveHashtag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string hashtag)
            {
                Hashtags.Remove(hashtag);
            }
        }
        #endregion
    }
}