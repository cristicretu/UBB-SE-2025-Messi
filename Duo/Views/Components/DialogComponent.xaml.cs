using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<(bool Success, string Title, string Content, List<string> Hashtags)> ShowCreatePostDialog(XamlRoot xamlRoot)
        {
            var dialogContent = new PostDialogContent();

            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = xamlRoot,
                Title = "Create New Post",
                Content = dialogContent,
                PrimaryButtonText = "Create",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };

            // Apply accent button style to the create button
            dialog.PrimaryButtonStyle = Application.Current.Resources["AccentButtonStyle"] as Style;

            ContentDialogResult result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                // Create a new list to return the hashtags
                var hashtagsList = new List<string>();
                foreach (var hashtag in dialogContent.Hashtags)
                {
                    hashtagsList.Add(hashtag);
                }

                return (true, dialogContent.PostTitle, dialogContent.PostContent, hashtagsList);
            }

            return (false, string.Empty, string.Empty, new List<string>());
        }
    }
}