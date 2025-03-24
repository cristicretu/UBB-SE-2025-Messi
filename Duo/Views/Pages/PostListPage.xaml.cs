using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using Duo.Models;
using Duo.ViewModels;
using Duo.Services;
using Duo.Repositories;
using Duo.Data;
using static Duo.App;

namespace Duo.Views.Pages
{
    public sealed partial class PostListPage : Page
    {
        // Make the ViewModel public to allow it to be accessed from other components
        public PostListViewModel ViewModel => _viewModel;

        private PostListViewModel _viewModel;
        private Dictionary<string, Button> hashtagButtons = new Dictionary<string, Button>();

        private double _previousPosition;
        private bool _isDragging;

        public PostListPage()
        {
            this.InitializeComponent();

            // Use the services from the App class to avoid initialization issues
            // This is more reliable than creating services locally
            var postService = App._postService;
            var categoryService = App._categoryService;

            // Create and initialize the view model
            _viewModel = new PostListViewModel(postService, categoryService);

            // Set DataContext for bindings
            this.DataContext = _viewModel;

            // Set up event handlers
            PostsPager.SelectedIndexChanged += PostsPager_SelectedIndexChanged;
            FilterByTitle.TextChanged += OnFilterChanged;

            // Initialize drag scrolling
            SetupHashtagDragScrolling();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if we received a category parameter
            if (e.Parameter is string categoryName && !string.IsNullOrEmpty(categoryName))
            {
                _viewModel.CategoryName = categoryName;

                PageTitle.Text = categoryName;

                if (_viewModel.CategoryID == 0 && _viewModel.CategoryName != null)
                {
                    var category = _categoryService.GetCategoryByName(_viewModel.CategoryName);
                    if (category != null)
                    {
                        _viewModel.CategoryID = category.Id;
                    }
                }
            }

            // Load posts
            _viewModel.LoadPosts();

            // Load hashtags now uses AllHashtags from the ViewModel
            UpdateHashtagsList();
        }

        private void UpdateHashtagsList()
        {
            // Clear existing items
            HashtagsContainer.Items.Clear();
            hashtagButtons.Clear();

            // Use the AllHashtags from ViewModel which is now properly populated
            if (_viewModel.AllHashtags != null)
            {
                // Create a button for each hashtag
                foreach (var hashtag in _viewModel.AllHashtags)
                {
                    Button button = new Button
                    {
                        Content = hashtag == "All" ? "All" : $"#{hashtag}",
                        Tag = hashtag,
                        Style = _viewModel.SelectedHashtags.Contains(hashtag) ?
                            Resources["SelectedHashtagButtonStyle"] as Style :
                            Resources["HashtagButtonStyle"] as Style
                    };

                    button.Click += Hashtag_Click;
                    HashtagsContainer.Items.Add(button);
                    hashtagButtons[hashtag] = button;
                }
            }
        }

        private void PostsPager_SelectedIndexChanged(PipsPager sender, PipsPagerSelectedIndexChangedEventArgs args)
        {
            // Update page in ViewModel and reload posts
            _viewModel.CurrentPage = sender.SelectedPageIndex + 1;
            _viewModel.LoadPosts();
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs args)
        {
            _viewModel.FilterText = FilterByTitle.Text;
        }

        private void Hashtag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string hashtag)
            {
                // Toggle hashtag in the ViewModel
                _viewModel.ToggleHashtag(hashtag);

                // Update UI to reflect changes
                UpdateHashtagButtonStyles();

                // Update PipsPager to show correct number of pages
                PostsPager.NumberOfPages = _viewModel.TotalPages;
                PostsPager.SelectedPageIndex = _viewModel.CurrentPage - 1;
            }
        }

        private void UpdateHashtagButtonStyles()
        {
            foreach (var entry in hashtagButtons)
            {
                string hashtag = entry.Key;
                Button button = entry.Value;

                button.Style = _viewModel.SelectedHashtags.Contains(hashtag)
                    ? Resources["SelectedHashtagButtonStyle"] as Style
                    : Resources["HashtagButtonStyle"] as Style;
            }
        }

        private void ClearHashtags_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFilters();
            UpdateHashtagButtonStyles();

            // Update PipsPager to show correct number of pages
            PostsPager.NumberOfPages = _viewModel.TotalPages;
            PostsPager.SelectedPageIndex = _viewModel.CurrentPage - 1;
        }

        private void FilteredListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Post clickedPost)
            {
                // Navigate to post detail page with the clicked post as parameter
                Frame.Navigate(typeof(PostDetailPage), clickedPost);
            }
        }

        private void SetupHashtagDragScrolling()
        {
            // Set up drag scrolling for the hashtags list
            HashtagsScrollViewer.PointerPressed += HashtagsScrollViewer_PointerPressed;
            HashtagsScrollViewer.PointerMoved += HashtagsScrollViewer_PointerMoved;
            HashtagsScrollViewer.PointerReleased += HashtagsScrollViewer_PointerReleased;
            HashtagsScrollViewer.PointerExited += HashtagsScrollViewer_PointerReleased;
            HashtagsScrollViewer.PointerCaptureLost += HashtagsScrollViewer_PointerReleased;
        }

        private void HashtagsScrollViewer_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _isDragging = true;
            _previousPosition = e.GetCurrentPoint(HashtagsScrollViewer).Position.X;

            // Capture the pointer to receive events outside the control
            HashtagsScrollViewer.CapturePointer(e.Pointer);

            // Mark the event as handled to prevent standard scrolling behavior
            e.Handled = true;
        }

        private void HashtagsScrollViewer_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_isDragging)
            {
                var currentPosition = e.GetCurrentPoint(HashtagsScrollViewer).Position.X;
                var delta = _previousPosition - currentPosition;

                // Update scroll position
                HashtagsScrollViewer.ChangeView(HashtagsScrollViewer.HorizontalOffset + delta, null, null);

                _previousPosition = currentPosition;
                e.Handled = true;
            }
        }

        private void HashtagsScrollViewer_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                HashtagsScrollViewer.ReleasePointerCapture(e.Pointer);
                e.Handled = true;
            }
        }
    }
}
