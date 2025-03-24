using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Duo.Helpers;
using Duo.ViewModels;

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

            if (Content is FrameworkElement rootElement)
            {
                rootElement.DataContext = _viewModel;
            }

            UsernameTextBox.KeyDown += (sender, e) => 
            {
                if (e.Key == Windows.System.VirtualKey.Enter && _viewModel.LoginCommand.CanExecute(null))
                {
                    _viewModel.LoginCommand.Execute(null);
                }
            };

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