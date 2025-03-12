using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Duo.Views.Components
{
    public sealed partial class PostComponent : UserControl
    {
        // Username Property
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register(nameof(Username), typeof(string), typeof(PostComponent), new PropertyMetadata("u/username"));

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        // Date Property
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register(nameof(Date), typeof(string), typeof(PostComponent), new PropertyMetadata("2025-03-12"));

        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        // Title Property
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(PostComponent), new PropertyMetadata("No hardware errors detected - what could it be?"));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Content Property
        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register(nameof(ContentText), typeof(string), typeof(PostComponent), new PropertyMetadata("Any Macheads have an idea what could cause the following on a MPB M1 Max with NO hardware issues detected using Apple's \n hardware test tool? \n screen rapidly flickers brightness when under RAM/CPU pressure \n screen turns full black and then back on under same \n login screen goes black, flashes, & resets to the previous after choosing account (e.g. I go from one account, to the account picker, click one account and begin to type in the password — and then it suddenly flashes black, jitters, and resets to the account picker again) \n shutting off w/o battery warning (battery health is good) \n freezes sometimes (way more than I'm used to) \n won't let me switch apps sometimes (while other things are still clickable, the menu bar will change to the app I switched to, but the menu isn't clickable; switching to another app, the menu works again) \n graphics for apps just don't update sometimes \n generally incredibly slow at times (compared to my husband's MPB M1 Max with, admittedly, more RAM)"));

        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        public PostComponent()
        {
            this.InitializeComponent();
        }
    }
}
