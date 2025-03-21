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
        private ObservableCollection<Post> _allPosts;
        private ObservableCollection<Post> _posts;
        private int _currentPage;
        private HashSet<string> _selectedHashtags = new HashSet<string>();
        private const int ItemsPerPage = 5;
        private int _totalPages = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public PostListViewModel(PostService postService = null, CategoryService categoryService = null)
        {
            _postService = postService;
            _categoryService = categoryService;
            _posts = new ObservableCollection<Post>();
            _allPosts = new ObservableCollection<Post>();
            _currentPage = 1;
            _selectedHashtags.Add("All");

            // Initialize commands
            LoadPostsCommand = new RelayCommand(LoadPosts);
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            FilterPostsCommand = new RelayCommand(FilterPosts);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
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

        public ObservableCollection<Post> AllPosts
        {
            get => _allPosts;
            set
            {
                _allPosts = value;
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

        public ICommand LoadPostsCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand FilterPostsCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public void LoadPosts()
        {
            if (_postService == null) return;

            // Convert to List<Post> to ensure consistent type
            IEnumerable<Post> postsData = _categoryID.HasValue 
                ? _postService.GetPostsByCategory(CategoryID, CurrentPage, ItemsPerPage)
                : _postService.GetAllPosts();
                
            AllPosts.Clear();
            foreach (var post in postsData)
            {
                var user = App.userService.GetUserById(post.UserID);
                post.Username = user?.Username ?? "Unknown User";
                
                post.Date = Helpers.DateTimeHelper.GetRelativeTime(post.CreatedAt);
                
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
                }
                
                AllPosts.Add(post);
            }
            
            FilterPosts();
        }

        public void LoadPageItems(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= TotalPages) return;
            
            CurrentPage = pageIndex + 1;
            
            var filteredPosts = AllPosts.Where(Filter).ToList();
            
            // Calculate start and end indices
            int startIndex = pageIndex * ItemsPerPage;
            int endIndex = Math.Min(startIndex + ItemsPerPage, filteredPosts.Count);
            
            Posts.Clear();
            // Add items for the current page
            for (int i = startIndex; i < endIndex; i++)
            {
                Posts.Add(filteredPosts[i]);
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadPageItems(CurrentPage - 1);
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadPageItems(CurrentPage - 1);
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
            
            FilterPosts();
        }

        public void FilterPosts()
        {
            var filteredPosts = AllPosts.Where(Filter).ToList();
            
            // Update pagination
            TotalPages = (int)Math.Ceiling(filteredPosts.Count / (double)ItemsPerPage);
            
            // Reset to first page
            LoadPageItems(0);
            
            OnPropertyChanged(nameof(TotalPages));
        }

        private bool Filter(Post post)
        {
            bool titleMatch = string.IsNullOrEmpty(FilterText) || 
                post.Title.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase);
            
            bool hashtagMatch = true;
            if (_selectedHashtags.Count > 0 && !_selectedHashtags.Contains("All"))
            {
                // If "All" is not selected, need to check if post has selected hashtags
                // Retrieve hashtags for this post
                if (_postService != null)
                {
                    try
                    {
                        // Get hashtags for this post
                        var hashtags = _postService.GetHashtagsByPostId(post.Id);
                        var hashtagNames = hashtags.Select(h => h.Name).ToList();
                        
                        // Ensure all selected hashtags are present in the post
                        hashtagMatch = _selectedHashtags.All(tag => hashtagNames.Contains(tag));
                    }
                    catch
                    {
                        // If there's an error retrieving hashtags, don't filter by hashtags
                        hashtagMatch = false;
                    }
                }
                else
                {
                    // If no post service, can't filter by hashtags
                    hashtagMatch = false;
                }
            }
            
            return titleMatch && hashtagMatch;
        }

        public void ClearFilters()
        {
            FilterText = string.Empty;
            _selectedHashtags.Clear();
            _selectedHashtags.Add("All");
            FilterPosts();
            OnPropertyChanged(nameof(SelectedHashtags));
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
