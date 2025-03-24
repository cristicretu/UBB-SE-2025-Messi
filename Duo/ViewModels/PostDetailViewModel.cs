using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Duo.Commands;
using Duo.Models;
using Duo.Services;
using Microsoft.UI.Xaml.Controls;
using static Duo.App;

namespace Duo.ViewModels
{
    public class PostDetailViewModel : INotifyPropertyChanged
    {
        private readonly PostService _postService;
        private readonly CommentService _commentService;
        private Post _post;
        private ObservableCollection<Comment> _comments;
        private bool _isLoading;
        private StackPanel _commentsPanel;
        private string _lastProcessedReply;

        public static Dictionary<int, bool> CollapsedComments { get; } = new Dictionary<int, bool>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CommentsLoaded;

        public PostDetailViewModel()
        {
            _postService = _postService ?? App._postService;
            _commentService = _commentService ?? new CommentService(_commentRepository, _postRepository, userService);

            _post = new Post { Title = "", Description = "" };

            Comments = new ObservableCollection<Comment>();

            LoadPostDetailsCommand = new RelayCommandWithParameter<int>(LoadPostDetails);
            AddCommentCommand = new RelayCommandWithParameter<string>(AddComment);
        }

        public Post Post
        {
            get => _post;
            set
            {
                _post = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Comment> Comments
        {
            get => _comments;
            set
            {
                _comments = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public StackPanel CommentsPanel
        {
            get => _commentsPanel;
            set
            {
                _commentsPanel = value;
                if (_commentsPanel != null && Post != null && Post.Id > 0)
                {
                    LoadComments(Post.Id);
                }
            }
        }

        public ICommand LoadPostDetailsCommand { get; private set; }
        public ICommand AddCommentCommand { get; private set; }

        public void LoadPostDetails(int postId)
        {
            IsLoading = true;

            try
            {
                if (postId <= 0)
                {
                    throw new ArgumentException("Invalid post ID", nameof(postId));
                }

                var post = _postService.GetPostById(postId);
                if (post != null)
                {
                    if (post.Id <= 0)
                    {
                        post.Id = postId;
                    }

                    var user = userService.GetUserById(post.UserID);
                    post.Username = $"u/{user?.Username ?? "Unknown User"}";

                    if (string.IsNullOrEmpty(post.Date) && post.CreatedAt != default)
                    {
                        post.Date = FormatDate(post.CreatedAt);
                    }

                    var hashtags = _postService.GetHashtagsByPostId(post.Id);
                    if (hashtags != null)
                    {
                        post.Hashtags = hashtags.Select(h => h.Name ?? h.Tag).ToList();
                    }

                    Post = post;

                    if (CommentsPanel != null)
                    {
                        LoadComments(post.Id);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsLoading = false;
            }
        }

        public void LoadComments(int postId)
        {
            if (CommentsPanel == null) return;

            CommentsPanel.Children.Clear();

            try
            {
                if (postId <= 0)
                {
                    throw new ArgumentException("Invalid post ID", nameof(postId));
                }

                var comments = _commentService.GetCommentsByPostId(postId);

                Comments.Clear();

                if (comments != null && comments.Any())
                {
                    foreach (var comment in comments)
                    {
                        Comments.Add(comment);
                    }

                    var topLevelComments = comments.Where(c => c.ParentCommentId == null).ToList();
                    

                    var repliesByParentId = comments
                                        .Where(c => c.ParentCommentId != null)
                                        .GroupBy(c => c.ParentCommentId)
                                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var comment in topLevelComments)
                    {
                        comment.Level = 1;
                    }

                    foreach (var parentId in repliesByParentId.Keys)
                    {
                        var parentComment = comments.FirstOrDefault(c => c.Id == parentId);
                        if (parentComment != null)
                        {
                            foreach (var reply in repliesByParentId[parentId])
                            {
                                reply.Level = parentComment.Level + 1;
                            }
                        }
                    }

                    foreach (var comment in topLevelComments)
                    {
                        var commentComponent = new Views.Components.Comment();
                        commentComponent.SetCommentData(comment, repliesByParentId);

                        if (CollapsedComments.TryGetValue(comment.Id, out bool isCollapsed) && isCollapsed)
                        {
                            commentComponent.SetChildrenCollapsed(true);
                        }

                        CommentsPanel.Children.Add(commentComponent);
                    }

                    CommentsLoaded?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    TextBlock noCommentsText = new TextBlock
                    {
                        Text = "No comments yet. Be the first to comment!",
                        Margin = new Microsoft.UI.Xaml.Thickness(0, 16, 0, 16)
                    };
                    CommentsPanel.Children.Add(noCommentsText);
                }
            }
            catch (Exception ex)
            {
                TextBlock errorText = new TextBlock
                {
                    Text = "Could not load comments. " + ex.Message,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red)
                };
                CommentsPanel.Children.Add(errorText);
            }
        }

        private void AddComment(string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText) || Post == null || Post.Id <= 0)
                return;

            try
            {
                _commentService.CreateComment(commentText, Post.Id, null);

                LoadComments(Post.Id);
            }
            catch (Exception ex)
            {
                
            }
        }

        public void AddReplyToComment(int parentCommentId, string replyText)
        {
            if (string.IsNullOrWhiteSpace(replyText) || Post == null || Post.Id <= 0 || parentCommentId <= 0)
                return;

            try
            {
                string replySignature = $"{parentCommentId}_{replyText}";

                bool isDuplicate = false;
                foreach (var comment in Comments)
                {
                    if (comment.ParentCommentId == parentCommentId && 
                        comment.Content.Equals(replyText, StringComparison.OrdinalIgnoreCase))
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (_lastProcessedReply == replySignature)
                {
                    
                    isDuplicate = true;
                }

                if (isDuplicate)
                {
                    return;
                }

                _lastProcessedReply = replySignature;

                _commentService.CreateComment(replyText, Post.Id, parentCommentId);

                LoadComments(Post.Id);
            }
            catch (Exception ex)
            {
                
            }
        }

        private string FormatDate(DateTime date)
        {
            return date.ToString("MMM dd");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 