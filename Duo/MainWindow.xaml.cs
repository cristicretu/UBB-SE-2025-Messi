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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Duo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const int TitleBarHeight = 32;
        
        public MainWindow()
        {
            this.InitializeComponent();
            
            // Extend content into titlebar for modern Windows look
            ExtendsContentIntoTitleBar = true;
            
            try
            {
                if (AppWindowTitleBar.IsCustomizationSupported())
                {
                    var titleBar = AppWindow.TitleBar;
                    titleBar.ExtendsContentIntoTitleBar = true;
                    
                    // Make button background transparent
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
                    
                    // Set button foreground colors from theme resources
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
        }


        // private void myButton_Click(object sender, RoutedEventArgs e)
        // {
        //     myButton.Content = "Clicked";
        // }
    }
}