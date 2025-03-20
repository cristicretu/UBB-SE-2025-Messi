using System;
using System.Windows.Input;
using Duo.Models;
using Duo.Services;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Duo.Commands;

namespace Duo.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private string _username = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError = false;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler LoginSuccessful;

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                    HasError = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool HasError
        {
            get => _hasError;
            private set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            LoginCommand = new RelayCommand(Login, CanLogin);
        }

        private bool CanLogin() => !string.IsNullOrWhiteSpace(Username);

        private void Login()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate the username
                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Username cannot be empty";
                    return;
                }

                if (Username.Length < 3)
                {
                    ErrorMessage = "Username must be at least 3 characters";
                    return;
                }

                try
                {
                    // Set the user in the service (it will create if needed)
                    _userService.setUser(Username);

                    // Notify successful login
                    LoginSuccessful?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
