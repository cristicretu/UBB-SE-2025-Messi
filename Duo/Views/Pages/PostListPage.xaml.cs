using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
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

        public MockPost()
        {
        }

        public MockPost(string title, string content)
        {
            Title = title;
            Content = content;
            // Hashtags ...
        }
    }

    public sealed partial class PostListPage : Page
    {
        private ObservableCollection<MockPost> allPosts = new ObservableCollection<MockPost>();
        public ObservableCollection<MockPost> postsFiltered { get; private set; } = new ObservableCollection<MockPost>();

        public PostListPage()
        {
            this.InitializeComponent();

            // Populate the allPosts collection with some data
            allPosts.Add(new MockPost("Post 1", "This is the first post."));
            allPosts.Add(new MockPost("Post 2", "This is the second post."));
            allPosts.Add(new MockPost("Post 3", "This is the third post."));
            allPosts.Add(new MockPost("Post 4", "This is the fourth post."));
            allPosts.Add(new MockPost("Post 5", "This is the fifth post."));
            
            // Initialize filtered posts with all posts
            foreach (var post in allPosts)
            {
                postsFiltered.Add(post);
            }
            
            // Set up pagination
            PostsPager.SelectedIndexChanged += PostsPager_SelectedIndexChanged;
        }
        
        private void PostsPager_SelectedIndexChanged(PipsPager sender, PipsPagerSelectedIndexChangedEventArgs args)
        {
            // Handle pagination - this would be implemented with actual paging logic
            // For now, we're just displaying all posts
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs args)
        {
            // Ensure the sender is a TextBox to avoid potential issues
            if (sender is TextBox)
            {
                var filtered = allPosts.Where(Filter).ToList();
                UpdateFilteredContacts(filtered);
            }
        }

        private bool Filter(MockPost post)
        {
            return post.Title.Contains(FilterByTitle.Text ?? string.Empty, StringComparison.InvariantCultureIgnoreCase) &&
                   post.Content.Contains(FilterByContent.Text ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);
                   // Add filtering by hashtags here
        }

        private void UpdateFilteredContacts(IEnumerable<MockPost> filteredData)
        {
            // Remove non-matching posts
            for (int i = postsFiltered.Count - 1; i >= 0; i--)
            {
                if (!filteredData.Contains(postsFiltered[i]))
                {
                    postsFiltered.RemoveAt(i);
                }
            }

            // Add back matching posts
            foreach (var post in filteredData)
            {
                if (!postsFiltered.Contains(post))
                {
                    postsFiltered.Add(post);
                }
            }
        }
    }
}
