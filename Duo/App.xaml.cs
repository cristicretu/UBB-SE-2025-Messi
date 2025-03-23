using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System.Diagnostics;
using Duo.Views;
using Microsoft.Extensions.Configuration;
using Duo.ViewModels;
using Duo.Services;
using Duo.Data;
using Duo.Repositories;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Duo
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static UserService userService;
        private static IConfiguration _configuration;
        public static DataLink _dataLink;
        public static UserRepository userRepository;
        public static PostRepository _postRepository;
        public static HashtagRepository _hashtagRepository;
        public static CommentRepository _commentRepository;
        public static PostService _postService;
        public static CategoryService _categoryService;
        public static SearchService _searchService;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            
            // Initialize configuration
            _configuration = InitializeConfiguration();
            
            // Initialize data link
            _dataLink = new DataLink(_configuration);
            
            // Initialize repositories
            userRepository = new UserRepository(_dataLink);
            _postRepository = new PostRepository(_dataLink);
            _hashtagRepository = new HashtagRepository(_dataLink);
            _commentRepository = new CommentRepository(_dataLink);
            var categoryRepository = new CategoryRepository(_dataLink);
            
            // Initialize services
            userService = new UserService(userRepository);
            _searchService = new SearchService();
            _postService = new PostService(_postRepository, _hashtagRepository, userService, _searchService);
            _categoryService = new CategoryService(categoryRepository);
        }

        private IConfiguration InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            return builder.Build();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Create view model
            var loginViewModel = new LoginViewModel(userService);
            
            // Show the login window first
            var loginWindow = new LoginWindow(loginViewModel);
            loginWindow.Activate();
            
            m_loginWindow = loginWindow;

            // Handle login success
            loginViewModel.LoginSuccessful += LoginViewModel_LoginSuccessful;
        }
        
        private void LoginViewModel_LoginSuccessful(object? sender, EventArgs e)
        {
            // Show the main window, after login
            m_window = new MainWindow();
            m_window.Activate();
            
            m_loginWindow?.Close();
            m_loginWindow = null;
        }

        private Window? m_window;
        private LoginWindow? m_loginWindow;
    }
}
