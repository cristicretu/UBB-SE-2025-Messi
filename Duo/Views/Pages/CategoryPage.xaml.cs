using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using Duo.Models;
using System.Collections.Generic;
using Duo.ViewModels;

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
                        return;
                    }

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
    }
}