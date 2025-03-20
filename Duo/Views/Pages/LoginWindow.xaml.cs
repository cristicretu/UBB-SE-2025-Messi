using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Duo.Helpers;
using Duo.ViewModels;

// is this the right way to access userService and its methods?
using static Duo.App;
using Duo.Models;

namespace Duo.Views
{
    public sealed partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel)
        {
            this.InitializeComponent();
            
            _viewModel = viewModel;
            
            // Set the DataContext for data binding
            // In WinUI 3, we need to set it on the content root element rather than the window
            if (Content is FrameworkElement rootElement)
            {
                rootElement.DataContext = _viewModel;
            }
            
            // login on enter
            UsernameTextBox.KeyDown += (sender, e) => 
            {
                if (e.Key == Windows.System.VirtualKey.Enter && _viewModel.LoginCommand.CanExecute(null))
                {
                    _viewModel.LoginCommand.Execute(null);
                }
            };
            
            // Handle button click
            LoginButton.Click += (sender, e) => 
            {
                if (_viewModel.LoginCommand.CanExecute(null))
                {
                    _viewModel.LoginCommand.Execute(null);
                }
            };
        }
    }
} 