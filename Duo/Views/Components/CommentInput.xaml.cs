using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Duo.Helpers;

namespace Duo.Views.Components
{
    public sealed partial class CommentInput : UserControl
    {
        private string _commentText = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError = false;

        public event EventHandler CommentSubmitted;

        public CommentInput()
        {
            this.InitializeComponent();
        }

        public string CommentText
        {
            get => _commentText;
            set
            {
                _commentText = value;
                try
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        ValidationHelper.ValidateComment(value);
                        ErrorMessage = string.Empty;
                        HasError = false;
                    }
                }
                catch (ArgumentException ex)
                {
                    ErrorMessage = ex.Message;
                    HasError = true;
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                _errorMessage = value;
                Bindings.Update();
            }
        }

        public bool HasError
        {
            get => _hasError;
            private set
            {
                _hasError = value;
                Bindings.Update();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommentText))
            {
                ErrorMessage = "Comment cannot be empty.";
                HasError = true;
                return;
            }

            try
            {
                ValidationHelper.ValidateComment(CommentText);
                CommentSubmitted?.Invoke(this, EventArgs.Empty);
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }

        public void ClearComment()
        {
            CommentText = string.Empty;
            ErrorMessage = string.Empty;
            HasError = false;
        }

        public void Focus(FocusState focusState)
        {
            CommentTextBox.Focus(focusState);
        }
    }
} 