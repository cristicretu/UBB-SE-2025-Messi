using Microsoft.UI.Xaml.Controls;

namespace Duo.Views.Components
{
    public sealed partial class DeleteDialogContent : UserControl
    {
        public string ContentText { get; set; }

        public DeleteDialogContent()
        {
            this.InitializeComponent();
        }
    }
}
