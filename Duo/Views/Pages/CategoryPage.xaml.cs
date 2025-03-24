using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using Duo.Models;
using System.Collections.Generic;
using Duo.ViewModels;
using Duo.Views.Components;
using static Duo.App;

namespace Duo.Views.Pages
{
    public sealed partial class CategoryPage : Page
    {
        private CategoryViewModel _viewModel;
        private int _currentCategoryId = 0;
        private string _currentCategoryName = string.Empty;

        public CategoryPage()
        {
            try
            {
                this.InitializeComponent();

                _viewModel = new CategoryViewModel(App._categoryService);

                this.DataContext = _viewModel;

                PopulateCommunityMenuItems();

                try
                {
                    User currentUser = App.userService.GetCurrentUser();
                    UsernameTextBlock.Text = currentUser.Username;
                }
                catch (Exception ex)
                {
                    UsernameTextBlock.Text = "Guest";
                    Debug.WriteLine($"Failed to get username: {ex.Message}");
                }

                try
                {
                    contentFrame.Navigate(typeof(MainPage));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Initial navigation failed: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Page initialization failed: {ex.Message}");
            }
        }

        private void PopulateCommunityMenuItems()
        {
            try
            {
                var categoryNames = _viewModel.GetCategoryNames();

                CommunityItem.MenuItems.Clear();

                foreach (string categoryName in categoryNames)
                {
                    var item = new NavigationViewItem
                    {
                        Content = categoryName,
                        Icon = new SymbolIcon(Symbol.Message),
                        Tag = categoryName
                    };

                    ToolTipService.SetToolTip(item, categoryName);

                    CommunityItem.MenuItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void NavigationView_SelectionChanged(object sender, NavigationViewSelectionChangedEventArgs args)
        {
            try
            {
                if (args.SelectedItem is NavigationViewItem selectedItem)
                {
                    var tag = selectedItem.Tag?.ToString();

                    if (string.IsNullOrEmpty(tag))
                        return;

                    if (selectedItem.MenuItems.Count > 0)
                    {
                        return;
                    }

                    if (IsCategoryTag(tag))
                    {
                        // Save the current category name
                        _currentCategoryName = tag;
                        
                        // Get category ID from name
                        if (!string.IsNullOrEmpty(_currentCategoryName))
                        {
                            try
                            {
                                var category = App._categoryService.GetCategoryByName(_currentCategoryName);
                                if (category != null)
                                {
                                    _currentCategoryId = category.Id;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error getting category ID: {ex.Message}");
                            }
                        }
                        
                        NavigateToPostListPage(tag);
                    }
                    else
                    {
                        // Reset the current category when navigating to a non-category page
                        _currentCategoryName = string.Empty;
                        _currentCategoryId = 0;
                        NavigateToPage(tag);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool IsCategoryTag(string tag)
        {
            var categoryNames = _viewModel.GetCategoryNames();
            foreach (var category in categoryNames)
            {
                if (tag == category)
                    return true;
            }
            return false;
        }

        private void NavigateToPostListPage(string category)
        {
            try
            {
                contentFrame.Navigate(typeof(PostListPage), category);
            }
            catch (Exception ex)
            {

            }
        }

        private void NavigateToPage(string tag)
        {
            try
            {
                Type? pageType = null;

                switch (tag)
                {
                    case "MainPage":
                        pageType = typeof(MainPage);
                        break;
                    default:
                        Debug.WriteLine($"Unknown page tag: {tag}");
                        return;
                }

                contentFrame.Navigate(pageType);
            }
            catch (Exception ex)
            {

            }
        }

        private async void CreatePostBtn_CreatePostRequested(object sender, RoutedEventArgs e) 
        {
            // First approach: Use the dialog with the embedded ViewModel
            var dialogComponent = new DialogComponent();
            var result = await dialogComponent.ShowCreatePostDialog(this.XamlRoot, _currentCategoryId);
            
            if (result.Success)
            {
                // The post is already created in the database through the ViewModel
                // Just show the success message and refresh the page
                
                // Show success message
                ContentDialog successDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Success",
                    Content = "Your post was created successfully!",
                    CloseButtonText = "OK"
                };
                await successDialog.ShowAsync();
                
                // Refresh the current page if we're on a category page
                if (!string.IsNullOrEmpty(_currentCategoryName))
                {
                    NavigateToPostListPage(_currentCategoryName);
                }
            }
            
            // Alternative approach: Use the ViewModel directly without the dialog
            // This is commented out as an example of how to use the ViewModel directly
            
            /*
            // Create a new ViewModel instance
            var postViewModel = new ViewModels.PostCreationViewModel();
            
            // Set the properties
            bool result = await postViewModel.CreatePostAsync(
                "Example Title", 
                "Example Content", 
                _currentCategoryId, 
                new List<string> { "example", "test" });
                
            if (result)
            {
                // Show success message and refresh the page
                ContentDialog successDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Success",
                    Content = "Your post was created successfully!",
                    CloseButtonText = "OK"
                };
                await successDialog.ShowAsync();
                
                // Refresh the current page if we're on a category page
                if (!string.IsNullOrEmpty(_currentCategoryName))
                {
                    NavigateToPostListPage(_currentCategoryName);
                }
            }
            else
            {
                // Show error message
                ContentDialog errorDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Error",
                    Content = $"Failed to create post: {postViewModel.Error}",
                    CloseButtonText = "OK"
                };
                await errorDialog.ShowAsync();
            }
            */
        }
    }
}