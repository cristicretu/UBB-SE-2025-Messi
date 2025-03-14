using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace Duo.Views.Pages
{
    public sealed partial class CategoryPage : Page
    {
        private readonly string[] categoryNames = new string[] 
        {
            "general-discussion",
            "lesson-help",
            "off-topic",
            "discovery",
            "announcements"
        };

        public CategoryPage()
        {
            try
            {
                this.InitializeComponent();

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
                    case "LoginPage":
                        pageType = typeof(LoginPage);
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
    }
}