using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using Duo.Models;
using System.Collections.Generic;
using Duo.ViewModels;
using Duo.Views.Components;

namespace Duo.Views.Pages
{
    public sealed partial class CategoryPage : Page
    {
        private CategoryViewModel _viewModel;

        public CategoryPage()
        {
            try
            {
                this.InitializeComponent();

                // Initialize ViewModel
                _viewModel = new CategoryViewModel(App._categoryService);
                
                // Set DataContext for data binding
                this.DataContext = _viewModel;
                
                // Populate community menu items
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
                
                // Clear existing items
                CommunityItem.MenuItems.Clear();
                
                // Add menu items for each category
                foreach (string categoryName in categoryNames)
                {
                    var item = new NavigationViewItem
                    {
                        Content = categoryName,
                        Icon = new SymbolIcon(Symbol.Message),
                        Tag = categoryName
                    };
                    
                    // Set tooltip after initialization
                    ToolTipService.SetToolTip(item, categoryName);
                    
                    CommunityItem.MenuItems.Add(item);
                }
                
                Debug.WriteLine($"Successfully populated {categoryNames.Count} category menu items");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to populate category menu items: {ex.Message}");
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
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
                        Debug.WriteLine($"Parent item '{selectedItem.Content}' selected. No navigation performed.");
                        return;
                    }

                    Debug.WriteLine($"Navigating to page with tag: {tag}");
                    
                    if (IsCategoryTag(tag))
                    {
                        NavigateToPostListPage(tag);
                    }
                    else
                    {
                        NavigateToPage(tag);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Selection changed error: {ex.Message}");
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
                Debug.WriteLine($"Navigated to PostListPage with category: {category}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
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
                    case "SearchPage":
                        pageType = typeof(SearchPage);
                        break;
                    default:
                        Debug.WriteLine($"Unknown page tag: {tag}");
                        return;
                }

                contentFrame.Navigate(pageType);
                Debug.WriteLine($"Navigated to page: {tag}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }

        private async void CreatePostBtn_CreatePostRequested(object sender, RoutedEventArgs e) 
        {
            var dialogComponent = new DialogComponent();
            var result = await dialogComponent.ShowCreatePostDialog(this.XamlRoot);
        }
    }
}