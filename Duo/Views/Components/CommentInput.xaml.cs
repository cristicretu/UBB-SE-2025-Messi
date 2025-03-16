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

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            // In the future, this will handle posting the comment
            // For now, just raise the event
            CommentSubmitted?.Invoke(this, e);
        }
    }
} 