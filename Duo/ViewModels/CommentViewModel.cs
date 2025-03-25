using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Duo.Commands;
using Duo.Models;
using Duo.ViewModels.Base;

namespace Duo.ViewModels
{
    public class CommentViewModel : ViewModelBase
    {
        private Models.Comment _comment;
        private ObservableCollection<CommentViewModel> _replies;
        private bool _isExpanded = true;
        private string _replyText;
        private bool _isReplyVisible;
        private int _likeCount;

        public CommentViewModel(Models.Comment comment, Dictionary<int, List<Models.Comment>> repliesByParentId)
        {
            _comment = comment ?? throw new ArgumentNullException(nameof(comment));
            _replies = new ObservableCollection<CommentViewModel>();
            _likeCount = comment.LikeCount;
            
            // Load any child comments/replies
            if (repliesByParentId != null && repliesByParentId.TryGetValue(comment.Id, out var childComments))
            {
                foreach (var reply in childComments)
                {
                    _replies.Add(new CommentViewModel(reply, repliesByParentId));
                }
            }
            
            ToggleRepliesCommand = new RelayCommand(ToggleReplies);
            ShowReplyFormCommand = new RelayCommand(ShowReplyForm);
            CancelReplyCommand = new RelayCommand(CancelReply);
            SubmitReplyCommand = new RelayCommand(SubmitReply);
            LikeCommentCommand = new RelayCommand(OnLikeComment);
        }

        public int Id => _comment.Id;
        public int UserId => _comment.UserId;
        public int? ParentCommentId => _comment.ParentCommentId;
        public string Content => _comment.Content;
        public string Username => _comment.Username;
        public string Date => FormatDate(_comment.CreatedAt);
        public int Level => _comment.Level;
        
        public int LikeCount
        {
            get => _likeCount;
            set => SetProperty(ref _likeCount, value);
        }

        public ObservableCollection<CommentViewModel> Replies
        {
            get => _replies;
            set => SetProperty(ref _replies, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public string ReplyText
        {
            get => _replyText;
            set => SetProperty(ref _replyText, value);
        }

        public bool IsReplyVisible
        {
            get => _isReplyVisible;
            set => SetProperty(ref _isReplyVisible, value);
        }

        public ICommand ToggleRepliesCommand { get; }
        public ICommand ShowReplyFormCommand { get; }
        public ICommand CancelReplyCommand { get; }
        public ICommand SubmitReplyCommand { get; }
        public ICommand LikeCommentCommand { get; }

        // Events
        public event EventHandler<Tuple<int, string>> ReplySubmitted;
        public event EventHandler<int> CommentLiked;

        private void ToggleReplies()
        {
            IsExpanded = !IsExpanded;
            PostDetailViewModel.CollapsedComments[Id] = !IsExpanded;
        }

        private void ShowReplyForm()
        {
            IsReplyVisible = true;
            ReplyText = string.Empty;
        }

        private void CancelReply()
        {
            IsReplyVisible = false;
            ReplyText = string.Empty;
        }

        private void SubmitReply()
        {
            if (string.IsNullOrWhiteSpace(ReplyText))
                return;

            ReplySubmitted?.Invoke(this, new Tuple<int, string>(Id, ReplyText));
            IsReplyVisible = false;
            ReplyText = string.Empty;
        }

        private void OnLikeComment()
        {
            CommentLiked?.Invoke(this, Id);
        }
        
        public void LikeComment()
        {
            LikeCount++;
            _comment.LikeCount = LikeCount;
        }

        private string FormatDate(DateTime date)
        {
            // Try to get local time
            try
            {
                if (date.Date == DateTime.Today)
                {
                    return "Today";
                }
                else if (date.Date == DateTime.Today.AddDays(-1))
                {
                    return "Yesterday";
                }
                else if ((DateTime.Today - date.Date).TotalDays < 7)
                {
                    return date.ToString("ddd"); // Day of week
                }
                
                DateTime localDate = Duo.Helpers.DateTimeHelper.ConvertUtcToLocal(date);
                return date.ToString("MMM d"); // Month + day
            }
            catch
            {
                // Fallback to simple format if helper methods fail
                return date.ToString("MMM dd, yyyy HH:mm");
            }
        }
    }
}
