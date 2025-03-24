using System;
using System.Windows.Input;
using Duo.Models;
using Duo.Services;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Duo.Commands;
using System.Collections.Generic;
using System.Diagnostics;
using static Duo.App;
using System.Collections.ObjectModel;
using Duo.Views.Components;
using System.Threading.Tasks;

namespace Duo.ViewModels
{
    /// <summary>
    /// The PostCreationViewModel is responsible for managing the creation and editing of posts.
    /// It provides properties and methods for interacting with the post creation UI,
    /// and handles the communication with the database through the PostService.
    /// 
    /// Features:
    /// - Managing post title and content
    /// - Handling hashtags (add, remove)
    /// - Community selection
    /// - Post creation with validation
    /// - Error handling
    /// </summary>
    public class PostCreationViewModel : INotifyPropertyChanged
    {
        // Services
        private readonly PostService _postService;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;

        // Properties
        private string _title = string.Empty;
        private string _content = string.Empty;
        private int _selectedCategoryId;
        private ObservableCollection<string> _hashtags = new ObservableCollection<string>();
        private ObservableCollection<CommunityItem> _communities = new ObservableCollection<CommunityItem>();
        private string _error = string.Empty;
        private bool _isLoading;
        private bool _isSuccess;

        // Commands
        public ICommand CreatePostCommand { get; private set; }
        public ICommand AddHashtagCommand { get; private set; }
        public ICommand RemoveHashtagCommand { get; private set; }
        public ICommand SelectCommunityCommand { get; private set; }

        // Property changed event
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler PostCreationSuccessful;

        #region Public Properties

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                if (_selectedCategoryId != value)
                {
                    _selectedCategoryId = value;
                    OnPropertyChanged();
                    UpdateSelectedCommunity();
                }
            }
        }

        public ObservableCollection<string> Hashtags => _hashtags;

        public ObservableCollection<CommunityItem> Communities => _communities;

        public string Error
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSuccess
        {
            get => _isSuccess;
            set
            {
                if (_isSuccess != value)
                {
                    _isSuccess = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public PostCreationViewModel()
        {
            // Get services from App
            _postService = _postService ?? App._postService;
            _categoryService = _categoryService ?? App._categoryService;
            _userService = _userService ?? App.userService;

            // Initialize commands
            CreatePostCommand = new RelayCommand(CreatePost);
            AddHashtagCommand = new RelayCommandWithParameter<string>(AddHashtag);
            RemoveHashtagCommand = new RelayCommandWithParameter<string>(RemoveHashtag);
            SelectCommunityCommand = new RelayCommandWithParameter<int>(SelectCommunity);

            // Load initial data
            LoadCommunities();
        }

        #region Public Methods

        public void CreatePost()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
            {
                Error = "Title and content are required.";
                return;
            }

            if (SelectedCategoryId <= 0)
            {
                Error = "Please select a community for your post.";
                return;
            }

            IsLoading = true;
            Error = string.Empty;

            try
            {
                // Get current user ID
                var currentUser = _userService.GetCurrentUser();
                
                // Create a new Post object
                var post = new Duo.Models.Post
                {
                    Title = Title,
                    Description = Content,
                    UserID = currentUser.UserId,
                    CategoryID = SelectedCategoryId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                // Create post in database using the original CreatePost method
                int postId = _postService.CreatePost(post);
                
                // Add hashtags if any
                if (Hashtags.Count > 0)
                {
                    foreach (var tag in Hashtags)
                    {
                        try
                        {
                            _postService.AddHashtagToPost(postId, tag, currentUser.UserId);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error adding hashtag '{tag}' to post: {ex.Message}");
                            // Continue with other hashtags even if one fails
                        }
                    }
                }

                // Handle success
                IsSuccess = true;
                PostCreationSuccessful?.Invoke(this, EventArgs.Empty);

                // Clear form
                ClearForm();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating post: {ex.Message}");
                Error = $"Failed to create post: {ex.Message}";
                IsSuccess = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> CreatePostAsync(string title, string content, int categoryId, List<string> hashtags = null)
        {
            // Set properties
            Title = title;
            Content = content;
            SelectedCategoryId = categoryId;
            
            // Clear and add hashtags
            Hashtags.Clear();
            if (hashtags != null)
            {
                foreach (var hashtag in hashtags)
                {
                    AddHashtag(hashtag);
                }
            }
            
            // Create the post
            IsLoading = true;
            Error = string.Empty;

            try
            {
                // Get current user ID
                var currentUser = _userService.GetCurrentUser();
                
                // Create a new Post object
                var post = new Duo.Models.Post
                {
                    Title = Title,
                    Description = Content,
                    UserID = currentUser.UserId,
                    CategoryID = SelectedCategoryId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                // Create post in database using the original CreatePost method
                int postId = _postService.CreatePost(post);
                
                // Add hashtags if any
                if (Hashtags.Count > 0)
                {
                    foreach (var tag in Hashtags)
                    {
                        try
                        {
                            _postService.AddHashtagToPost(postId, tag, currentUser.UserId);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error adding hashtag '{tag}' to post: {ex.Message}");
                            // Continue with other hashtags even if one fails
                        }
                    }
                }

                // Handle success
                IsSuccess = true;
                PostCreationSuccessful?.Invoke(this, EventArgs.Empty);
                
                // Clear form
                ClearForm();
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating post: {ex.Message}");
                Error = $"Failed to create post: {ex.Message}";
                IsSuccess = false;
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AddHashtag(string hashtag)
        {
            if (string.IsNullOrWhiteSpace(hashtag))
                return;

            string trimmedHashtag = hashtag.Trim();
            
            // Add the hashtag if it doesn't already exist
            if (!Hashtags.Contains(trimmedHashtag))
            {
                Hashtags.Add(trimmedHashtag);
                
                // Debug output
                System.Diagnostics.Debug.WriteLine($"Added hashtag to ViewModel: {trimmedHashtag}, Count now: {Hashtags.Count}");
                
                // Explicitly notify that the Hashtags collection has changed
                OnPropertyChanged(nameof(Hashtags));
            }
        }

        public void RemoveHashtag(string hashtag)
        {
            if (!string.IsNullOrWhiteSpace(hashtag) && Hashtags.Contains(hashtag))
            {
                Hashtags.Remove(hashtag);
                
                // Debug output
                System.Diagnostics.Debug.WriteLine($"Removed hashtag from ViewModel: {hashtag}, Count now: {Hashtags.Count}");
                
                // Explicitly notify that the Hashtags collection has changed
                OnPropertyChanged(nameof(Hashtags));
            }
        }

        public void SelectCommunity(int communityId)
        {
            SelectedCategoryId = communityId;
        }

        public void ClearForm()
        {
            Title = string.Empty;
            Content = string.Empty;
            SelectedCategoryId = 0;
            Hashtags.Clear();
            UpdateSelectedCommunity();
            Error = string.Empty;
            IsSuccess = false;
        }

        #endregion

        #region Private Methods

        private void LoadCommunities()
        {
            try
            {
                var categories = _categoryService.GetAllCategories();
                
                Communities.Clear();
                foreach (var category in categories)
                {
                    Communities.Add(new CommunityItem
                    {
                        Id = category.Id,
                        Name = category.Name,
                        IsSelected = (category.Id == SelectedCategoryId)
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading communities: {ex.Message}");
                Error = $"Failed to load communities: {ex.Message}";
            }
        }

        private void UpdateSelectedCommunity()
        {
            foreach (var community in Communities)
            {
                community.IsSelected = (community.Id == SelectedCategoryId);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
