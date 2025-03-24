using System;
using System.Windows.Input;
using Duo.Models;
using Duo.Services;
using Duo.Commands;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Duo.ViewModels
{
    public class PostListViewModel : INotifyPropertyChanged
    {
        private readonly PostService _postService;
        private readonly CategoryService _categoryService;
        private int? _categoryID;
        private string _categoryName;
        private string _filterText;
        private ObservableCollection<Post> _posts;
        private int _currentPage;
        private HashSet<string> _selectedHashtags = new HashSet<string>();
        private const int ItemsPerPage = 5;
        private int _totalPages = 1;
        private List<string> _allHashtags = new List<string>();
        private int _totalPostCount = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public PostListViewModel(PostService postService = null, CategoryService categoryService = null)
        {
            _postService = postService;
            _categoryService = categoryService;
            _posts = new ObservableCollection<Post>();
            _currentPage = 1;
            _selectedHashtags.Add("All");

            // Initialize commands
            LoadPostsCommand = new RelayCommand(LoadPosts);
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            FilterPostsCommand = new RelayCommand(FilterPosts);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            
            // Load all available hashtags
            LoadAllHashtags();
        }

        public ObservableCollection<Post> Posts
        {
            get => _posts;
            set
            {
                _posts = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged();
                FilterPosts();
            }
        }

        public int CategoryID
        {
            get => _categoryID ?? 0;
            set
            {
                _categoryID = value;
                OnPropertyChanged();
                
                // Reload hashtags when category changes
                LoadAllHashtags();
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        public HashSet<string> SelectedHashtags => _selectedHashtags;
        
        public List<string> AllHashtags 
        {
            get => _allHashtags;
            set
            {
                _allHashtags = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadPostsCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand FilterPostsCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        private void LoadAllHashtags()
        {
            if (_postService == null) return;
            
            try
            {
                _allHashtags.Clear();
                _allHashtags.Add("All");
                
                List<Hashtag> hashtags;
                
                // Get hashtags based on category selection
                if (_categoryID.HasValue && _categoryID.Value > 0)
                {
                    // Get hashtags for the specific category
                    hashtags = _postService.GetHashtagsByCategory(_categoryID.Value);
                }
                else
                {
                    // Get all hashtags
                    hashtags = _postService.GetAllHashtags();
                }
                
                // Add hashtags to the list
                foreach (var hashtag in hashtags)
                {
                    if (!_allHashtags.Contains(hashtag.Name))
                    {
                        _allHashtags.Add(hashtag.Name);
                    }
                }
                
                OnPropertyChanged(nameof(AllHashtags));
            }
            catch (Exception ex)
            {
                // Handle the error
            }
        }

        public void LoadPosts()
        {
            if (_postService == null) return;

            Posts.Clear();
            
            // Calculate offset for pagination
            int offset = (CurrentPage - 1) * ItemsPerPage;
            
            IEnumerable<Post> postsData;
            
            try
            {
                // Check if we're filtering by hashtags
                if (_selectedHashtags.Count > 0 && !_selectedHashtags.Contains("All"))
                {
                    // Get posts by hashtags with pagination
                    postsData = _postService.GetPostsByHashtags(_selectedHashtags.ToList(), CurrentPage, ItemsPerPage);
                    
                    // Get total count to calculate pages
                    _totalPostCount = _postService.GetPostCountByHashtags(_selectedHashtags.ToList());
                }
                else if (_categoryID.HasValue)
                {
                    // Get posts by category with pagination
                    postsData = _postService.GetPostsByCategory(CategoryID, CurrentPage, ItemsPerPage);
                    
                    // Get total count to calculate pages
                    _totalPostCount = _postService.GetPostCountByCategory(CategoryID);
                }
                else
                {
                    // Get all posts with pagination
                    postsData = _postService.GetPaginatedPosts(CurrentPage, ItemsPerPage);
                    
                    // Get total count to calculate pages
                    _totalPostCount = _postService.GetTotalPostCount();
                }
                
                // Process posts for display
                foreach (var post in postsData)
                {
                    // Already set by the query
                    if (string.IsNullOrEmpty(post.Username))
                    {
                        var user = App.userService.GetUserById(post.UserID);
                        post.Username = user?.Username ?? "Unknown User";
                    }
                    
                    post.Date = Helpers.DateTimeHelper.GetRelativeTime(post.CreatedAt);
                    
                    // Fetch hashtags for the post
                    post.Hashtags.Clear();
                    try 
                    {
                        var hashtags = _postService.GetHashtagsByPostId(post.Id);
                        foreach (var hashtag in hashtags)
                        {
                            post.Hashtags.Add(hashtag.Name);
                        }
                    }
                    catch
                    {
                        // Silently handle errors loading hashtags
                    }
                    
                    // Apply search filter if we have a search term
                    bool shouldAdd = true;
                    if (!string.IsNullOrEmpty(FilterText))
                    {
                        // Only add posts that match the search term
                        shouldAdd = App._searchService.Search(FilterText, new[] { post.Title }).Any();
                    }
                    
                    if (shouldAdd)
                    {
                        Posts.Add(post);
                    }
                }
                
                // If search is active, adjust the count for pagination
                if (!string.IsNullOrEmpty(FilterText))
                {
                    _totalPostCount = Posts.Count;
                }
                
                // Calculate total pages (minimum 1)
                TotalPages = Math.Max(1, (int)Math.Ceiling(_totalPostCount / (double)ItemsPerPage));
                
                OnPropertyChanged(nameof(TotalPages));
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during loading
                // Could display an error message here
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadPosts();
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadPosts();
            }
        }

        public void ToggleHashtag(string hashtag)
        {
            if (hashtag == "All")
            {
                _selectedHashtags.Clear();
                _selectedHashtags.Add("All");
            }
            else
            {
                if (_selectedHashtags.Contains(hashtag))
                {
                    _selectedHashtags.Remove(hashtag);
                    
                    if (_selectedHashtags.Count == 0)
                    {
                        _selectedHashtags.Add("All");
                    }
                }
                else
                {
                    _selectedHashtags.Add(hashtag);
                    
                    if (_selectedHashtags.Contains("All"))
                    {
                        _selectedHashtags.Remove("All");
                    }
                }
            }
            
            // Reset to first page and reload when hashtags change
            CurrentPage = 1;
            LoadPosts();
            OnPropertyChanged(nameof(SelectedHashtags));
        }

        public void FilterPosts()
        {
            // Reset to first page when filtering
            CurrentPage = 1;
            LoadPosts();
        }

        public void ClearFilters()
        {
            FilterText = string.Empty;
            _selectedHashtags.Clear();
            _selectedHashtags.Add("All");
            CurrentPage = 1;
            LoadPosts();
            OnPropertyChanged(nameof(SelectedHashtags));
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
