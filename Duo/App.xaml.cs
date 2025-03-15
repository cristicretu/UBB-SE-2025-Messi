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
using Microsoft.UI.Composition.SystemBackdrops;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Duo
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            
            // Add exception handling to catch unhandled exceptions
            this.UnhandledException += App_UnhandledException;
        }
        
        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Log the exception
            Debug.WriteLine($"UNHANDLED EXCEPTION: {e.Message}");
            Debug.WriteLine(e.Exception.ToString());
            
            // Mark as handled to prevent app termination
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                Debug.WriteLine("App.OnLaunched - Creating login window");
                
                // For debugging purposes - skip login for now
                // Comment this out to return to normal login flow
                m_window = new MainWindow();
                m_window.Activate();
                Debug.WriteLine("App.OnLaunched - Main window activated directly");
                return;
                
                // Normal login flow
                // var loginWindow = new LoginWindow();
                // loginWindow.LoginSuccessful += LoginWindow_LoginSuccessful;
                // loginWindow.Activate();
                // Debug.WriteLine("App.OnLaunched - Login window activated");
                // m_loginWindow = loginWindow;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in OnLaunched: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }
        
        private void LoginWindow_LoginSuccessful(object? sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("LoginWindow_LoginSuccessful - Creating main window");
                
                // show the main window, after login
                m_window = new MainWindow();
                m_window.Activate();
                Debug.WriteLine("LoginWindow_LoginSuccessful - Main window activated");
                
                // Close login window
                if (m_loginWindow != null)
                {
                    m_loginWindow.Close();
                    m_loginWindow = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in LoginWindow_LoginSuccessful: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private Window? m_window;
        private LoginWindow? m_loginWindow;

        // Helper method to apply backdrop effect to a window
        public static void ApplySystemBackdrop(Window window)
        {
            // Temporarily disabled to resolve compilation issues
            /*
            try
            {
                if (MicaController.IsSupported())
                {
                    // Use Mica for Windows 11
                    var micaController = new MicaController();
                    window.SystemBackdrop = micaController;
                }
                else if (DesktopAcrylicController.IsSupported())
                {
                    // Fallback to Acrylic for Windows 10
                    var acrylicController = new DesktopAcrylicController();
                    window.SystemBackdrop = acrylicController;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to apply system backdrop: {ex.Message}");
                // Continue without backdrop effect
            }
            */
        }
    }
}
