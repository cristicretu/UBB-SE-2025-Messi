using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Duo.Views.Components
{
    public sealed partial class CommentInput : UserControl
    {

        public event RoutedEventHandler CommentSubmitted;

        public CommentInput()
        {
            this.InitializeComponent();
        }

        public string CommentText
        {
            get { return CommentTextBox.Text; }
        }

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