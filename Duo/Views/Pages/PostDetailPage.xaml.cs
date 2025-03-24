using Duo.Models;
using Duo.Services;
using Duo.Views.Components;
using Duo.Repositories;
using Duo.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duo.ViewModels;

using static Duo.App;

namespace Duo.Views.Pages
{
    public sealed partial class PostDetailPage : Page
    {
        private readonly CommentService _commentService;

        public PostDetailPage()
        {
            this.InitializeComponent();

            _commentService = new CommentService(_commentRepository, _postRepository, userService);

            ViewModel.CommentsPanel = CommentsPanel;

            ViewModel.CommentsLoaded += ViewModel_CommentsLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Models.Post post && post.Id > 0)
            {

                ViewModel.LoadPostDetails(post.Id);
            }
            else
            {

                TextBlock errorText = new TextBlock
                {
                    Text = "Invalid post data received",
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
                    Margin = new Thickness(0, 20, 0, 0)
                };
                CommentsPanel.Children.Add(errorText);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void CommentInputControl_CommentSubmitted(object sender, RoutedEventArgs e)
        {
            if (sender is Components.CommentInput commentInput && commentInput.CommentText != null)
            {
                if (ViewModel.AddCommentCommand.CanExecute(commentInput.CommentText))
                {
                    ViewModel.AddCommentCommand.Execute(commentInput.CommentText);

                    commentInput.ClearComment();
                }
            }
        }

        private void ViewModel_CommentsLoaded(object sender, EventArgs e)
        {
            ConnectCommentReplyEvents();

            ConnectCommentLikeEvents();
        }

        private void ConnectCommentReplyEvents()
        {
            DisconnectCommentReplyEvents();

            var commentControls = GetAllChildrenOfType<Components.Comment>(CommentsPanel);

            foreach (var comment in commentControls)
            {
                comment.ReplySubmitted += Comment_ReplySubmitted;
            }
        }

        private void DisconnectCommentReplyEvents()
        {
            var commentControls = GetAllChildrenOfType<Components.Comment>(CommentsPanel);
            foreach (var comment in commentControls)
            {
                comment.ReplySubmitted -= Comment_ReplySubmitted;
            }
        }

        private void ConnectCommentLikeEvents()
        {

            DisconnectCommentLikeEvents();

            var commentControls = GetAllChildrenOfType<Components.Comment>(CommentsPanel);

            foreach (var comment in commentControls)
            {
                comment.CommentLiked += Comment_CommentLiked;
            }
        }

        private void DisconnectCommentLikeEvents()
        {
            var commentControls = GetAllChildrenOfType<Components.Comment>(CommentsPanel);
            foreach (var comment in commentControls)
            {
                comment.CommentLiked -= Comment_CommentLiked;
            }
        }

        private void Comment_ReplySubmitted(object sender, CommentReplyEventArgs e)
        {
            ViewModel.AddReplyToComment(e.ParentCommentId, e.ReplyText);
        }

        private void Comment_CommentLiked(object sender, CommentLikedEventArgs e)
        {
            
        }

        private List<T> GetAllChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            var list = new List<T>();
            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T childOfType)
                {
                    list.Add(childOfType);
                }

                if (child is FrameworkElement)
                {
                    var childrenOfType = GetAllChildrenOfType<T>(child);
                    if (childrenOfType.Any())
                    {
                        list.AddRange(childrenOfType);
                    }
                }
            }

            return list;
        }
    }
}