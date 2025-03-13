using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace Duo.Views.Pages
{
    public sealed partial class CategoryPage : Page
    {
        public CategoryPage()
        {
            try
            {
                this.InitializeComponent();

                // // Ensure frame starts on a default page
                // try
                // {
                //     contentFrame.Navigate(typeof(MainPage));
                // }
                // catch (Exception ex)
                // {
                //     Debug.WriteLine($"Initial navigation failed: {ex.Message}");
                // }
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

                    // Ignore parent items
                    if (selectedItem.MenuItems.Count > 0)
                    {
                        Debug.WriteLine($"Parent item '{selectedItem.Content}' selected. No navigation performed.");
                        return;
                    }

                    Debug.WriteLine($"Navigating to page with tag: {tag}");
                    NavigateToPage(tag);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Selection changed error: {ex.Message}");
            }
        }

        private void NavigateToPage(string pageTag)
        {
            try
            {
                // TODO implement navigation to different pages based on tag
                Type pageType = pageTag switch
                {
                    "BeginnerPage" => typeof(MainPage),
                    "IntermediaryPage" => typeof(MainPage),
                    "AdvancedPage" => typeof(MainPage),
                    "BeginnerQuizPage" => typeof(MainPage),
                    "IntermediaryQuizPage" => typeof(MainPage),
                    "AdvancedQuizPage" => typeof(MainPage),
                    _ => typeof(MainPage)
                };

                contentFrame.Navigate(pageType);
                Debug.WriteLine($"Navigated to {pageType.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }
    }
}
