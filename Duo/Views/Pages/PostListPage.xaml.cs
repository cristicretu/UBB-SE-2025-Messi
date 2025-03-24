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
        private PostListViewModel _viewModel;
        private Dictionary<string, Button> hashtagButtons = new Dictionary<string, Button>();

        private double _previousPosition;
        private bool _isDragging;

        public PostListPage()
        {
            this.InitializeComponent();

            var postService = App._postService;
            var categoryService = App._categoryService;

            _viewModel = new PostListViewModel(postService, categoryService);

            this.DataContext = _viewModel;

            PostsPager.SelectedIndexChanged += PostsPager_SelectedIndexChanged;
            FilterByTitle.TextChanged += OnFilterChanged;

            SetupHashtagDragScrolling();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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

            _viewModel.LoadPosts();

            UpdateHashtagsList();
        }

        private void UpdateHashtagsList()
        {
            HashtagsContainer.Items.Clear();
            hashtagButtons.Clear();

            if (_viewModel.AllHashtags != null)
            {
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
                _viewModel.ToggleHashtag(hashtag);

                UpdateHashtagButtonStyles();

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

            PostsPager.NumberOfPages = _viewModel.TotalPages;
            PostsPager.SelectedPageIndex = _viewModel.CurrentPage - 1;
        }

        private void FilteredListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Post clickedPost)
            {
                Frame.Navigate(typeof(PostDetailPage), clickedPost);
            }
        }

        private void SetupHashtagDragScrolling()
        {
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

            HashtagsScrollViewer.CapturePointer(e.Pointer);

            e.Handled = true;
        }

        private void HashtagsScrollViewer_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_isDragging)
            {
                var currentPosition = e.GetCurrentPoint(HashtagsScrollViewer).Position.X;
                var delta = _previousPosition - currentPosition;

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
