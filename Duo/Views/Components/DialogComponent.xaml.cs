using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System;

namespace Duo.Views.Components
{
    public sealed partial class DialogComponent
    {
        public async Task<bool> ShowConfirmationDialog(string title, string content, XamlRoot xamlRoot)
        {
            var dialogContent = new DialogContent
            {
                ContentText = content
            };

            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = xamlRoot,
                Title = title,
                Content = dialogContent,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary
            };

            // Apply accent button style to the confirmation button
            dialog.PrimaryButtonStyle = Application.Current.Resources["AccentButtonStyle"] as Style;

            ContentDialogResult result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}