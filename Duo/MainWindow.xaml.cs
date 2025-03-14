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
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Duo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private void RunTestCode()
        {
            try
            {
                Debug.WriteLine("Running Test");

                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                Debug.WriteLine("Configuration created");
                Debug.WriteLine(configuration["LocalDataSource"]);
                DataLink dataLink = new DataLink(configuration);
                Debug.WriteLine("DataLink created");

                CommentRepository commentRepository = new CommentRepository(dataLink);
                Debug.WriteLine("CommentRepository created");

                Comment comment = new Comment(0, "Test Comment", 1, 1, null, DateTime.Now, 0, 1);
                Debug.WriteLine("Comment created");

                int commentId = commentRepository.CreateComment(comment);
                Debug.WriteLine("Comment created");

                List<Comment> comments = commentRepository.GetAllComments();
                Debug.WriteLine("Comments retrieved");

                foreach (Comment c in comments)
                {
                    Debug.WriteLine(c.Content);
                }

                Debug.WriteLine("Test Complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public MainWindow()
        {
            this.InitializeComponent();

            RunTestCode();
        }

        // private void myButton_Click(object sender, RoutedEventArgs e)
        // {
        //     myButton.Content = "Clicked";
        // }
    }
}
