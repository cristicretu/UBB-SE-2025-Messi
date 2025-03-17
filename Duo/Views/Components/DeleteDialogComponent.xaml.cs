using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System;

namespace Duo.Views.Components
{
    public sealed partial class DeleteDialogComponent
    {
        public async Task<bool> ShowDeleteConfirmationDialog(string title, string content, XamlRoot xamlRoot)
        {
            var dialogContent = new DeleteDialogContent
            {
                ContentText = content
            };

            ContentDialog deleteDialog = new ContentDialog
            {
                XamlRoot = xamlRoot,
                Title = title,
                Content = dialogContent,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary
            };

            // Apply accent button style to the confirmation button
            deleteDialog.PrimaryButtonStyle = Application.Current.Resources["AccentButtonStyle"] as Style;

            ContentDialogResult result = await deleteDialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}