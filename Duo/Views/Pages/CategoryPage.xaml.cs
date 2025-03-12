using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace Duo.Views.Pages
{
    public sealed partial class CategoryPage : Page
    {
        public CategoryPage()
        {
            this.InitializeComponent();
            
            // Set initial page when component loads
            contentFrame.Navigate(typeof(Page)); // Navigate to a default page
            
            // Subscribe to NavigationView events
            nvSample.ItemInvoked += NvSample_ItemInvoked;
            nvSample.BackRequested += NvSample_BackRequested;
        }
        
        private void NvSample_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Handle settings navigation if needed
                // contentFrame.Navigate(typeof(SettingsPage));
                return;
            }
            
            // Get the tag of the selected item
            var tag = args.InvokedItemContainer?.Tag?.ToString();
            
            // Navigate based on the tag
            NavigateToPage(tag);
        }
        
        private void NvSample_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }
        }
        
        private void NavigateToPage(string pageTag)
        {
            // Return early if pageTag is null
            if (string.IsNullOrEmpty(pageTag))
            {
                return;
            }
            
            Type? pageType = null;
            
            // Map the tag to the appropriate page type
            switch (pageTag)
            {
                case "SamplePage1":
                    // pageType = typeof(SamplePage1);
                    break;
                case "SamplePage2":
                    // pageType = typeof(SamplePage2);
                    break;
                case "SamplePage3":
                    // pageType = typeof(SamplePage3);
                    break;
                case "SamplePage4":
                    // pageType = typeof(SamplePage4);
                    break;
                default:
                    // Default page if no match is found
                    // pageType = typeof(DefaultPage);
                    break;
            }
            
            // Navigate to the page if a type was set
            if (pageType != null)
            {
                contentFrame.Navigate(pageType);
            }
        }
    }
}
