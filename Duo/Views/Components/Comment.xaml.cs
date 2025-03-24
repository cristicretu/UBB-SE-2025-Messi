using Duo.Models;
using Duo.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Duo.ViewModels;
using static Duo.App;

namespace Duo.Views.Components
{
    public sealed partial class Comment : UserControl
    {
        private Models.Comment _commentData;
        private Dictionary<int?, List<Models.Comment>> _commentsByParent;
        private readonly CommentService _commentService;
        private const int MAX_NESTING_LEVEL = 3; 
        private bool _childrenVisible = true; 

        public event EventHandler<CommentReplyEventArgs> ReplySubmitted;

        public event EventHandler<CommentLikedEventArgs> CommentLiked;

        public Comment()
        {
            this.InitializeComponent();

            _commentService = new CommentService(_commentRepository, _postRepository, userService);

            CommentReplyButton.Click += CommentReplyButton_Click;

            LikeButton.LikeClicked += LikeButton_LikeClicked;

            ReplyInputControl.CommentSubmitted += ReplyInput_CommentSubmitted;
        }

        public void SetCommentData(Models.Comment comment, Dictionary<int?, List<Models.Comment>> commentsByParent)
        {
            _commentData = comment;
            _commentsByParent = commentsByParent;

            UserTextBlock.Text = $"u/{comment.Username}";
            DescriptionTextBlock.Text = comment.Content;
            DateTextBlock.Text = FormatDate(comment.CreatedAt);

            LikeButton.LikeCount = comment.LikeCount;
            LikeButton.CommentId = comment.Id;

            var indentationLevels = new List<int>();

            for (int i = 1; i <= comment.Level; i++)
            {
                indentationLevels.Add(i);
            }

            LevelLinesRepeater.ItemsSource = indentationLevels;

            CommentReplyButton.Visibility = (comment.Level >= MAX_NESTING_LEVEL) 
                ? Visibility.Collapsed 
                : Visibility.Visible;

            if (commentsByParent.ContainsKey(comment.Id))
            {
                foreach (var childComment in commentsByParent[comment.Id])
                {
                    var childCommentControl = new Comment();
                    childCommentControl.SetCommentData(childComment, commentsByParent);
                    childCommentControl.ReplySubmitted += ChildComment_ReplySubmitted;
                    childCommentControl.CommentLiked += ChildComment_CommentLiked;
                    ChildCommentsPanel.Children.Add(childCommentControl);
                }

                ToggleChildrenButton.Visibility = Visibility.Visible;

                ToggleIcon.Glyph = "\uE108";
            }
            else
            {
                ToggleChildrenButton.Visibility = Visibility.Collapsed;
            }

            HideReplyInput();
        }

        public void SetChildrenCollapsed(bool collapsed)
        {

            if (ChildCommentsPanel.Children.Count > 0)
            {
                _childrenVisible = !collapsed;
                ChildCommentsPanel.Visibility = _childrenVisible ? Visibility.Visible : Visibility.Collapsed;
                ToggleIcon.Glyph = _childrenVisible ? "\uE108" : "\uE109"; 
            }
        }

        private void ToggleChildrenButton_Click(object sender, RoutedEventArgs e)
        {
            _childrenVisible = !_childrenVisible;

            ChildCommentsPanel.Visibility = _childrenVisible ? Visibility.Visible : Visibility.Collapsed;

            ToggleIcon.Glyph = _childrenVisible ? "\uE108" : "\uE109"; 

            if (_commentData != null)
            {
                PostDetailViewModel.CollapsedComments[_commentData.Id] = !_childrenVisible;
            }
        }

        private void LikeButton_LikeClicked(object sender, LikeButtonClickedEventArgs e)
        {
            if (e.TargetType == LikeTargetType.Comment && e.TargetId == _commentData.Id)
            {
                try
                {
                    if (_commentService.LikeComment(_commentData.Id))
                    {
                        LikeButton.LikeCount++;

                        CommentLiked?.Invoke(this, new CommentLikedEventArgs(_commentData.Id));
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void ChildComment_CommentLiked(object sender, CommentLikedEventArgs e)
        {
            CommentLiked?.Invoke(this, e);
        }

        private void CommentReplyButton_Click(object sender, RoutedEventArgs e)
        {
            ShowReplyInput();
        }

        private void ReplyInput_CommentSubmitted(object sender, RoutedEventArgs e)
        {
            if (sender is Components.CommentInput commentInput && !string.IsNullOrWhiteSpace(commentInput.CommentText))
            {

                ReplySubmitted?.Invoke(this, new CommentReplyEventArgs(_commentData.Id, commentInput.CommentText));

                commentInput.ClearComment();
                HideReplyInput();
            }
        }

        private void ChildComment_ReplySubmitted(object sender, CommentReplyEventArgs e)
        {
            ReplySubmitted?.Invoke(this, e);
        }

        public void AddReplyComment(Models.Comment replyComment)
        {
            var childCommentControl = new Comment();
            childCommentControl.SetCommentData(replyComment, _commentsByParent);
            childCommentControl.ReplySubmitted += ChildComment_ReplySubmitted;
            childCommentControl.CommentLiked += ChildComment_CommentLiked;
            ChildCommentsPanel.Children.Add(childCommentControl);

            if (!_commentsByParent.ContainsKey(_commentData.Id))
            {
                _commentsByParent[_commentData.Id] = new List<Models.Comment>();
            }

            _commentsByParent[_commentData.Id].Add(replyComment);

            ToggleChildrenButton.Visibility = Visibility.Visible;

            ToggleIcon.Glyph = "\uE108"; 
            _childrenVisible = true;
            ChildCommentsPanel.Visibility = Visibility.Visible;
        }

        private void ShowReplyInput()
        {
            ReplyInputControl.Visibility = Visibility.Visible;
            ReplyInputControl.Focus(FocusState.Programmatic);
        }

        private void HideReplyInput()
        {
            ReplyInputControl.Visibility = Visibility.Collapsed;
            ReplyInputControl.ClearComment();
        }

        private string FormatDate(DateTime dateTime)
        {
            if (dateTime.Date == DateTime.Today)
            {
                return "Today";
            }
            else if (dateTime.Date == DateTime.Today.AddDays(-1))
            {
                return "Yesterday";
            }
            else if ((DateTime.Today - dateTime.Date).TotalDays < 7)
            {
                return dateTime.ToString("ddd"); 
            }
            else
            {
                return dateTime.ToString("MMM d"); 
            }
        }
    }

    public class CommentReplyEventArgs : EventArgs
    {
        public int ParentCommentId { get; private set; }
        public string ReplyText { get; private set; }

        public CommentReplyEventArgs(int parentCommentId, string replyText)
        {
            ParentCommentId = parentCommentId;
            ReplyText = replyText;
        }
    }

    public class CommentLikedEventArgs : EventArgs
    {
        public int CommentId { get; private set; }

        public CommentLikedEventArgs(int commentId)
        {
            CommentId = commentId;
        }
    }
} 