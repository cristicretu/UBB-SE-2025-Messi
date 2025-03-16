using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Duo.Views.Pages;

namespace Duo.Views.Pages
{
    public sealed partial class PostDetailPage : Page
    {
        private MockPost? _post;

        public PostDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.Parameter is MockPost post)
            {
                _post = post;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (_post != null)
            {
                PostTitle.Text = _post.Title;
                PostContent.Text = _post.Content;
                PostUsername.Text = _post.Username;
                PostDate.Text = _post.Date;
            }
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back to the previous page
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
