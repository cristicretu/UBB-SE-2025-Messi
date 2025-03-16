using Duo.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Duo.Views.Components
{
    public sealed partial class Comment : UserControl
    {
        private MockComment _commentData;

        public Comment()
        {
            this.InitializeComponent();
        }

        public void SetCommentData(MockComment comment, Dictionary<int, List<MockComment>> commentsByParent)
        {
            _commentData = comment;

            // Set the comment data
            UserTextBlock.Text = $"u/{comment.User}";
            DescriptionTextBlock.Text = comment.Description;
            DateTextBlock.Text = FormatDate(comment.Date);

            // Generate lines based on tree level
            var lineCount = new List<int>();
            for (int i = 0; i <= comment.TreeLevel; i++)
            {
                lineCount.Add(i);
            }
            LevelLinesRepeater.ItemsSource = lineCount;

            // Add child comments if any
            if (commentsByParent.ContainsKey(comment.Id))
            {
                foreach (var childComment in commentsByParent[comment.Id])
                {
                    var childCommentControl = new Comment();
                    childCommentControl.SetCommentData(childComment, commentsByParent);
                    ChildCommentsPanel.Children.Add(childCommentControl);
                }
            }
        }

        private string FormatDate(DateTime date)
        {
            // For simplicity, just display the short date
            return date.ToString("MMM dd");
        }
    }
} 