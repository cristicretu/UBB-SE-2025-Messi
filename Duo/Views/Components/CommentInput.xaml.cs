using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Duo.Views.Components
{
    public sealed partial class CommentInput : UserControl
    {
        // Event that will be raised when the Post button is clicked
        public event RoutedEventHandler CommentSubmitted;

        public CommentInput()
        {
            this.InitializeComponent();
        }
        
        // Property to get the current comment text
        public string CommentText
        {
            get { return CommentTextBox.Text; }
        }
        
        // Method to clear the comment text
        public void ClearComment()
        {
            CommentTextBox.Text = string.Empty;
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            CommentSubmitted?.Invoke(this, e);
        }
    }
} 