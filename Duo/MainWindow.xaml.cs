using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace Duo
{

    public sealed partial class MainWindow : Window
    {
        public static event EventHandler LoggedIn;
        public static void RaiseLoggedInEvent()
        {
            LoggedIn?.Invoke(null, EventArgs.Empty);
        }

        private AppWindow _apw;
        private OverlappedPresenter _presenter;

        private const int TitleBarHeight = 32;

        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;

            try
            {
                if (AppWindowTitleBar.IsCustomizationSupported())
                {
                    var titleBar = AppWindow.TitleBar;
                    titleBar.ExtendsContentIntoTitleBar = true;

                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                    var buttonHoverBackgroundBrush = Application.Current.Resources["SystemControlBackgroundListLowBrush"] as SolidColorBrush;
                    var buttonPressedBackgroundBrush = Application.Current.Resources["SystemControlBackgroundListMediumBrush"] as SolidColorBrush;

                    if (buttonHoverBackgroundBrush != null)
                        titleBar.ButtonHoverBackgroundColor = buttonHoverBackgroundBrush.Color;
                    else
                        titleBar.ButtonHoverBackgroundColor = Colors.Transparent;

                    if (buttonPressedBackgroundBrush != null)
                        titleBar.ButtonPressedBackgroundColor = buttonPressedBackgroundBrush.Color;
                    else
                        titleBar.ButtonPressedBackgroundColor = Colors.Transparent;

                    var foregroundBrush = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as SolidColorBrush;
                    var foregroundHoverBrush = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as SolidColorBrush;
                    var foregroundPressedBrush = Application.Current.Resources["SystemControlForegroundBaseMediumBrush"] as SolidColorBrush;

                    if (foregroundBrush != null)
                        titleBar.ButtonForegroundColor = foregroundBrush.Color;

                    if (foregroundHoverBrush != null)
                        titleBar.ButtonHoverForegroundColor = foregroundHoverBrush.Color;

                    if (foregroundPressedBrush != null)
                        titleBar.ButtonPressedForegroundColor = foregroundPressedBrush.Color;

                    int windowWidth = (int)AppWindow.Size.Width;
                    int systemButtonsWidth = 138; 

                    titleBar.SetDragRectangles(new RectInt32[]
                    {
                        new RectInt32(0, 0, windowWidth - systemButtonsWidth, TitleBarHeight)
                    });

                    this.SizeChanged += (s, e) =>
                    {
                        int newWidth = (int)AppWindow.Size.Width;
                        titleBar.SetDragRectangles(new RectInt32[]
                        {
                            new RectInt32(0, 0, newWidth - systemButtonsWidth, TitleBarHeight)
                        });
                    };

                    ContentRoot.Padding = new Thickness(0, TitleBarHeight, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Titlebar customization not supported: {ex.Message}");
            }

            // Size and position window
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = AppWindow.GetFromWindowId(windowId);
            _presenter = _apw.Presenter as OverlappedPresenter;
            
            SetDefaultWindowPosition();
            SetCustomWindowStyle();
            
            LoggedIn += OnLoginSuccess;
        }

        private void OnLoginSuccess(object sender, EventArgs e)
        {
            // Update UI
        }

        private void SetDefaultWindowPosition()
        {
            try
            {
                // Center window on screen
                var displayArea = DisplayArea.GetFromWindowId(
                    _apw.Id, DisplayAreaFallback.Primary);
                
                // Calculate center position based on desired window size
                int desiredWidth = 1200;
                int desiredHeight = 700;
                
                var centerX = (displayArea.WorkArea.Width - desiredWidth) / 2;
                var centerY = (displayArea.WorkArea.Height - desiredHeight) / 2;
                
                // Set window size and position
                _apw.MoveAndResize(new RectInt32(
                    centerX, 
                    centerY, 
                    desiredWidth, 
                    desiredHeight));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting window position: {ex.Message}");
            }
        }
        
        private void SetCustomWindowStyle()
        {
            try
            {
                if (_presenter != null)
                {
                    // Set default size constraints
                    _presenter.IsResizable = true;
                    _presenter.IsMaximizable = true;
                    _presenter.IsMinimizable = true;
                    _presenter.SetBorderAndTitleBar(true, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting window style: {ex.Message}");
            }
        }
    }
}