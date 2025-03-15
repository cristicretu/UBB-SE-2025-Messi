using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Duo.Helpers;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;

namespace Duo.Views.Pages
{
    public sealed partial class PostListPage : Page
    {
        private string _categoryName = string.Empty;
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
            }
        }
        
        // Tag filters collection
        public ObservableCollection<string> TagFilters { get; } = new ObservableCollection<string>();
        
        // Track the currently selected filter
        private RadioButton _currentSelectedFilter;
        
        // Variables for drag scrolling
        private double _previousPosition;
        private bool _isDragging;

        public PostListPage()
        {
            this.InitializeComponent();
            
            // Initialize tag filters
            InitializeTagFilters();
            
            // Subscribe to the Loaded event
            this.Loaded += PostListPage_Loaded;
            
            // Setup drag scrolling for filter chips
            SetupDragScrolling();
        }
        
        private void SetupDragScrolling()
        {
            // Use the ScrollViewer's PointerEvents to implement drag scrolling
            FilterScrollViewer.PointerPressed += FilterScrollViewer_PointerPressed;
            FilterScrollViewer.PointerMoved += FilterScrollViewer_PointerMoved;
            FilterScrollViewer.PointerReleased += FilterScrollViewer_PointerReleased;
            FilterScrollViewer.PointerExited += FilterScrollViewer_PointerReleased;
            FilterScrollViewer.PointerCaptureLost += FilterScrollViewer_PointerReleased;
        }
        
        private void FilterScrollViewer_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isDragging = true;
            _previousPosition = e.GetCurrentPoint(FilterScrollViewer).Position.X;
            
            // Capture the pointer to receive events outside the control
            FilterScrollViewer.CapturePointer(e.Pointer);
            
            // Mark the event as handled to prevent standard scrolling behavior
            e.Handled = true;
        }
        
        private void FilterScrollViewer_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isDragging)
            {
                var currentPosition = e.GetCurrentPoint(FilterScrollViewer).Position.X;
                var delta = _previousPosition - currentPosition;
                
                // Update scroll position
                FilterScrollViewer.ChangeView(FilterScrollViewer.HorizontalOffset + delta, null, null);
                
                _previousPosition = currentPosition;
                e.Handled = true;
            }
        }
        
        private void FilterScrollViewer_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                FilterScrollViewer.ReleasePointerCapture(e.Pointer);
                e.Handled = true;
            }
        }
        
        private void PostListPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Select the "All" filter by default if this is the first load
            if (_currentSelectedFilter == null)
            {
                // Using FindFirstChild helper method to find the "All" radio button
                SelectDefaultFilter();
            }
        }
        
        private void InitializeTagFilters()
        {
            // Add the "All" filter first (selected by default)
            TagFilters.Add("All");
            
            // Add additional tag filters
            TagFilters.Add("#Apps");
            TagFilters.Add("#Documents");
            TagFilters.Add("#Web");
            TagFilters.Add("#People");
            TagFilters.Add("#IMG");
            TagFilters.Add("#JPG");
            TagFilters.Add("#GIF");
            TagFilters.Add("#iCloud");
            TagFilters.Add("#OneDrive");
            TagFilters.Add("#SkyDrive");
            TagFilters.Add("#Pictures");
            TagFilters.Add("#Videos");
            TagFilters.Add("#Audio");
            TagFilters.Add("#Code");
            TagFilters.Add("#Projects");
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.Parameter is string categoryName)
            {
                CategoryName = categoryName;
            }
        }
        
        private void FilterChip_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                // Only handle new selections to avoid double-triggering
                if (_currentSelectedFilter != radioButton)
                {
                    // Set this as the current selected filter
                    _currentSelectedFilter = radioButton;
                    
                    string selectedFilter = radioButton.Content.ToString();
                    Debug.WriteLine($"Filter selected: {selectedFilter}");
                    
                    // This is where you would filter posts based on the selected tag
                    // For now, just logging to console
                }
            }
        }
        
        // Helper method to select the default "All" filter
        private void SelectDefaultFilter()
        {
            // Use a direct approach since we're in the Loaded event handler
            foreach (var child in FilterChips.GetChildren())
            {
                if (child is RadioButton rb && rb.Content.ToString() == "All")
                {
                    rb.IsChecked = true;
                    _currentSelectedFilter = rb;
                    break;
                }
            }
        }
        
        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                string searchText = sender.Text;
                Debug.WriteLine($"Search query: {searchText}");
                
                // This is where you would implement actual search functionality
                // For now, just logging to console as requested
            }
        }
    }
}
