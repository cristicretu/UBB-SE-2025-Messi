using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

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
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    AttemptLogin();
                }
            };
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
            
            // close the window
            this.Close();
        }
    }
} 