using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Duo.Helpers;

namespace Duo.Views
{
    public sealed partial class LoginWindow : Window
    {
        public event EventHandler? LoginSuccessful;

        public LoginWindow()
        {
            this.InitializeComponent();
            
            // login on enter 
            UsernameTextBox.KeyDown += (sender, e) => 
            {
                DebounceHelper.Debounce(() => {
                   Console.WriteLine("Current text is: " + UsernameTextBox.Text);
                }, 200);

                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    e.Handled = true;
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
                ValidationHelper.ValidateUsername(username);
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
            Console.WriteLine("Username: " + UsernameTextBox.Text);
            
            this.Close();
        }
    }
} 