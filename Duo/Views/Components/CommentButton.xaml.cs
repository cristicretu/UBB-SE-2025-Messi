using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Duo.Views.Components
{
    public sealed partial class CommentButton : UserControl
    {
        public CommentButton()
        {
            this.InitializeComponent();
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            // For now, this button doesn't do anything
            // In the future, it will allow replying to comments
        }
    }
} 