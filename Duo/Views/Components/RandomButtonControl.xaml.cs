using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Duo.Views.Components
{
    public sealed partial class RandomButtonControl : UserControl
    {
        private readonly string[] _randomMessages = new string[]
        {
            "Hello, World!",
            "Have a nice day!",
            "WinUI 3 is awesome!",
            "Random messages are fun!",
            "This component works!"
        };

        private readonly Random _random = new Random();

        public RandomButtonControl()
        {
            this.InitializeComponent();
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            // Get a random message
            int index = _random.Next(0, _randomMessages.Length);
            string message = _randomMessages[index];
            
            // Display the message
            MessageText.Text = message;
            MessageText.Visibility = Visibility.Visible;
        }
    }
} 