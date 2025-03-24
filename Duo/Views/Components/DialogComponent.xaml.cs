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
                DefaultButton = ContentDialogButton.Primary,
                MinWidth = 500,
                MinHeight = 450
            };

            // Apply accent button style to the create button
            dialog.PrimaryButtonStyle = Application.Current.Resources["AccentButtonStyle"] as Style;
            
            dialog.PrimaryButtonClick += (s, e) => 
            {
                if (!dialogContent.IsFormValid())
                {
                    e.Cancel = true;
                }
            };
            
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

        public async Task<(bool Success, string Title, string Content, List<string> Hashtags)> ShowEditPostDialog(XamlRoot xamlRoot, string title = "", string content = "", List<string> hashtags = null)
        {
            var dialogContent = new PostDialogContent();
            
            // Prefill the dialog with existing post data
            dialogContent.PostTitle = title;
            dialogContent.PostContent = content;
            
            // Add existing hashtags if provided
            if (hashtags != null)
            {
                foreach (var hashtag in hashtags)
                {
                    dialogContent.Hashtags.Add(hashtag);
                }
            }

            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = xamlRoot,
                Title = "Edit Post",
                Content = dialogContent,
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                MinWidth = 500,
                MinHeight = 450
            };

            // Apply accent button style to the save button
            dialog.PrimaryButtonStyle = Application.Current.Resources["AccentButtonStyle"] as Style;
            
            dialog.PrimaryButtonClick += (s, e) => 
            {
                if (!dialogContent.IsFormValid())
                {
                    e.Cancel = true;
                }
            };
            
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