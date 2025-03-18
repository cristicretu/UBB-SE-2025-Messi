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

        private async void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Raise the EditClicked event when the Edit menu item is clicked
            EditClicked?.Invoke(this, e);

            // Handle the edit logic here

            bool succesfullyEdited = true; // Placeholder for actual logic
            
            if (succesfullyEdited)
            {
                // Send/confirm
                ContentDialog successDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Edited",
                    Content = "The item has been successfully edited.",
                    CloseButtonText = "OK"
                };
                await successDialog.ShowAsync();
            } else {
                // Handle the error logic here
                ContentDialog errorDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Error",
                    Content = "An error occurred while editing the item. Please try again.",
                    CloseButtonText = "OK"
                };
                await errorDialog.ShowAsync();
            }
        }

        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Raise the DeleteClicked event when the Delete menu item is clicked
            DeleteClicked?.Invoke(this, e);

            // Instantiate a DeleteDialog
            var deleteDialog = new DialogComponent();

            // Create the deletion confirmation (add whatever text you wish to display)
            bool isConfirmed = await deleteDialog.ShowConfirmationDialog(
                "Confirm Deletion",
                "Are you sure you want to delete this item?",
                this.XamlRoot
            );

            // if User confirms...
            if (isConfirmed)
            {
                // Handle the deletion logic here
                // Send/confirm
                ContentDialog successDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Deleted",
                    Content = "The item has been successfully deleted.",
                    CloseButtonText = "OK"
                };
                await successDialog.ShowAsync();
            }
        }
    }
}
