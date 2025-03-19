using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Duo.Helpers;

// is this the right way to access userService and its methods?
using static Duo.App;
using Duo.Models;

namespace Duo.Views
{
    public sealed partial class LoginWindow : Window
    {
        public event EventHandler? LoginSuccessful;

        public LoginWindow()
        {
            this.InitializeComponent();
            
            // login on enter 
            // Issue: When pressing Enter, the text entered by the user gets an extra 'Enter' / '\n' character, thus breaking the validation
            // Solution: Debounce the keydown event to prevent the extra character from being added?
            
            UsernameTextBox.KeyDown += (sender, e) => 
            {
                DebounceHelper.Debounce(() => {
                   Console.WriteLine("Current text is: " + UsernameTextBox.Text);
                }, 200);

                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    AttemptLogin();
                }
            };
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;
            
            string errorMessage = IsLoginValid(UsernameTextBox.Text);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
                return;
            }
            
            AttemptLogin();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        private string IsLoginValid(string username)
        {
            try 
            {
                // Validate the username
                ValidationHelper.ValidateUsername(username);

                // Create a new user
                // userService.CreateUser('1', username);

                // Only fot testing purposes
                userService.setUser(username);
                return null; 
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void AttemptLogin()
        {
            
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
            
            this.Close();
        }
    }
} 