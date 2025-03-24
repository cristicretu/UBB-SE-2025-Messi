using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using Duo.Helpers;
using Duo.Models;
using static Duo.App;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Duo.ViewModels;

namespace Duo.Views.Components
{
    // Community item class with selection state
    public class CommunityItem : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private bool _isSelected;

        public int Id 
        { 
            get => _id; 
            set 
            { 
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            } 
        }
        
        public string Name 
        { 
            get => _name; 
            set 
            { 
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            } 
        }
        
        public bool IsSelected 
        { 
            get => _isSelected; 
            set 
            { 
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Bool to background color converter
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isSelected && isSelected)
            {
                return new SolidColorBrush(Colors.DodgerBlue);
            }
            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    // Integer to visibility converter
    public class IntToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int count)
            {
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed partial class PostDialogContent : UserControl
    {
        // Validation TextBlocks
        private bool _isTitleValid = true;
        private bool _isContentValid = true;
        private bool _isHashtagValid = true;

        // ViewModel
        private PostCreationViewModel _viewModel;
        
        public PostCreationViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
                
                _viewModel = value;
                DataContext = _viewModel;
                
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged += ViewModel_PropertyChanged;
                }
                
                UpdateUIVisibility();
            }
        }

        public PostDialogContent()
        {
            this.InitializeComponent();
            
            // Initialize ViewModel
            ViewModel = new PostCreationViewModel();
            
            // Subscribe to the post creation success event
            ViewModel.PostCreationSuccessful += ViewModel_PostCreationSuccessful;
            
            // Ensure UI is updated
            UpdateUIVisibility();
        }

        private void ViewModel_PostCreationSuccessful(object sender, EventArgs e)
        {
            // This method can be used to handle successful post creation
            // For example, close the dialog or show a success message
        }

        public void SetSelectedCommunity(int communityId)
        {
            ViewModel.SelectedCategoryId = communityId;
        }

        private void CommunityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int communityId)
            {
                ViewModel.SelectCommunity(communityId);
            }
        }

        #region Validation Methods
        
        private bool ValidateTitle()
        {
            HideError(TitleErrorTextBlock);
            
            var (isValid, errorMessage) = ValidationHelper.ValidatePostTitle(ViewModel.Title);
            
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
            
            var (isValid, errorMessage) = ValidationHelper.ValidatePostContent(ViewModel.Content);
            
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
            bool isCommunitySelected = ViewModel.SelectedCategoryId > 0;
            
            if (!string.IsNullOrEmpty(HashtagTextBox.Text))
            {
                isHashtagValid = ValidateHashtag(HashtagTextBox.Text);
            }
            
            return isTitleValid && isContentValid && isHashtagValid && isCommunitySelected;
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
            _isHashtagValid = ValidateHashtag(HashtagTextBox.Text);
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
            string hashtag = HashtagTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(hashtag))
                return;
                
            if (!ValidateHashtag(hashtag))
                return;

            // Add to collection if not a duplicate
            ViewModel.AddHashtag(hashtag);
            
            // Debug output
            System.Diagnostics.Debug.WriteLine($"Added hashtag: {hashtag}, Count now: {ViewModel.Hashtags.Count}");

            // Clear the input
            HashtagTextBox.Text = string.Empty;
            HideError(HashtagErrorTextBlock);
            
            // Force UI update
            UpdateUIVisibility();
        }

        private void RemoveHashtag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string hashtag)
            {
                ViewModel.RemoveHashtag(hashtag);
                System.Diagnostics.Debug.WriteLine($"Removed hashtag: {hashtag}, Count now: {ViewModel.Hashtags.Count}");
                
                // Force UI update
                UpdateUIVisibility();
            }
        }
        #endregion

        private void UpdateUIVisibility()
        {
            // Debug output
            System.Diagnostics.Debug.WriteLine($"UpdateUIVisibility called. Hashtags count: {ViewModel?.Hashtags?.Count ?? 0}");
            
            // Update hashtags header visibility
            HashtagsHeader.Visibility = (ViewModel != null && ViewModel.Hashtags != null && ViewModel.Hashtags.Count > 0) 
                ? Visibility.Visible : Visibility.Collapsed;
            
            // Update error TextBlock visibility
            ErrorTextBlock.Visibility = (ViewModel != null && !string.IsNullOrWhiteSpace(ViewModel.Error)) 
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Property changed: {e.PropertyName}");
            
            if (e.PropertyName == nameof(ViewModel.Hashtags) || 
                e.PropertyName == nameof(ViewModel.Error) ||
                e.PropertyName == "Item[]") // This can be fired for collection changes
            {
                UpdateUIVisibility();
            }
        }
    }
}