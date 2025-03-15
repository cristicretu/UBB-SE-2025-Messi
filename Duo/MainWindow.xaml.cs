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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Duo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            
            // Extend content into titlebar for modern Windows look
            ExtendsContentIntoTitleBar = true;
            
            // Set up Windows 11 style titlebar (if supported)
            try
            {
                if (AppWindowTitleBar.IsCustomizationSupported())
                {
                    var titleBar = AppWindow.TitleBar;
                    titleBar.ExtendsContentIntoTitleBar = true;
                    
                    // Set to default theme-aware colors rather than explicit transparency
                    titleBar.ButtonBackgroundColor = null; // Use system default
                    titleBar.ButtonForegroundColor = null; // Use system default
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