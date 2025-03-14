using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq; // Ensure this is included for LINQ operations
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace Duo.Views.Pages
{
    // MockPost class to represent a post until we have a real Post class
    public class MockPost
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Hashtags { get; set; } = new List<string>();

        public MockPost()
        {
        }

        public MockPost(string title, string content, params string[] hashtags)
        {
            Title = title;
            Content = content;
            if (hashtags != null)
            {
                Hashtags.AddRange(hashtags);
            }
        }
    }

    public sealed partial class PostListPage : Page
    {
        private ObservableCollection<MockPost> allPosts = new ObservableCollection<MockPost>();
        public ObservableCollection<MockPost> postsFiltered { get; private set; } = new ObservableCollection<MockPost>();
        private HashSet<string> selectedHashtags = new HashSet<string>();
        private Dictionary<string, Button> hashtagButtons = new Dictionary<string, Button>();

        public PostListPage()
        {
            this.InitializeComponent();

            // Populate the allPosts collection with some data and hashtags
            allPosts.Add(new MockPost("Technology Trends 2023", "This post discusses the latest technology trends.", "technology", "news"));
            allPosts.Add(new MockPost("Programming Best Practices", "Learn about programming best practices.", "programming", "education"));
            allPosts.Add(new MockPost("UI/UX Design Principles", "A guide to modern UI/UX design principles.", "design", "art"));
            allPosts.Add(new MockPost("Latest Science Discoveries", "Recent breakthroughs in science.", "science", "news"));
            allPosts.Add(new MockPost("Healthy Eating Habits", "Tips for maintaining a healthy diet.", "health", "food"));
            
            // Initialize filtered posts with all posts
            foreach (var post in allPosts)
            {
                postsFiltered.Add(post);
            }
            
            // Set up pagination
            PostsPager.SelectedIndexChanged += PostsPager_SelectedIndexChanged;
            
            // Store references to hashtag buttons
            foreach (var item in HashtagsContainer.Items)
            {
                if (item is Button button && button.Tag is string tag)
                {
                    hashtagButtons[tag] = button;
                }
            }
        }
        
        private void PostsPager_SelectedIndexChanged(PipsPager sender, PipsPagerSelectedIndexChangedEventArgs args)
        {
            // Handle pagination - this would be implemented with actual paging logic
            // For now, we're just displaying all posts
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs args)
        {
            ApplyFilters();
        }

        private void Hashtag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string hashtag)
            {
                // Toggle hashtag selection
                if (selectedHashtags.Contains(hashtag))
                {
                    selectedHashtags.Remove(hashtag);
                    button.Style = Resources["HashtagButtonStyle"] as Style;
                }
                else
                {
                    selectedHashtags.Add(hashtag);
                    button.Style = Resources["SelectedHashtagButtonStyle"] as Style;
                }
                
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            var filtered = allPosts.Where(Filter).ToList();
            UpdateFilteredPosts(filtered);
        }

        private bool Filter(MockPost post)
        {
            bool titleMatch = post.Title.Contains(FilterByTitle.Text ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);
            
            bool hashtagMatch = true;
            if (selectedHashtags.Count > 0)
            {
                hashtagMatch = selectedHashtags.Any(tag => post.Hashtags.Contains(tag));
            }
            
            return titleMatch && hashtagMatch;
        }

        private void UpdateFilteredPosts(IEnumerable<MockPost> filteredData)
        {
            // Clear and repopulate the filtered list
            postsFiltered.Clear();
            
            foreach (var post in filteredData)
            {
                postsFiltered.Add(post);
            }
        }
    }
}
