using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Duo.Views.Components
{
    public sealed partial class MoreDropdown : UserControl
    {
        // Events to expose the edit and delete actions
        public event EventHandler<RoutedEventArgs> EditClicked;
        public event EventHandler<RoutedEventArgs> DeleteClicked;

        public MoreDropdown()
        {
            this.InitializeComponent();
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Raise the EditClicked event when the Edit menu item is clicked
            EditClicked?.Invoke(this, e);
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Raise the DeleteClicked event when the Delete menu item is clicked
            DeleteClicked?.Invoke(this, e);
        }
    }
}
