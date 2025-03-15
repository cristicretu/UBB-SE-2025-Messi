using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Duo.Helpers;

namespace Duo.Views.Components
{
    public sealed partial class PostComponent : UserControl
    {
        // Username Property
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register(nameof(Username), typeof(string), typeof(PostComponent), 
            new PropertyMetadata(string.Empty, OnPropertyChanged));

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        // UTC Date Property
        public static readonly DependencyProperty UtcDateProperty =
            DependencyProperty.Register(nameof(UtcDate), typeof(string), typeof(PostComponent), 
            new PropertyMetadata(string.Empty, OnUtcDateChanged));

        public string UtcDate
        {
            get { return (string)GetValue(UtcDateProperty); }
            set { SetValue(UtcDateProperty, value); }
        }

        // Formatted Date Property
        public static readonly DependencyProperty FormattedDateProperty =
            DependencyProperty.Register(nameof(FormattedDate), typeof(string), typeof(PostComponent), 
            new PropertyMetadata(string.Empty));

        public string FormattedDate
        {
            get { return (string)GetValue(FormattedDateProperty); }
            private set { SetValue(FormattedDateProperty, value); }
        }

        // Title Property
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(PostComponent), 
            new PropertyMetadata(string.Empty, OnPropertyChanged));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Hashtags Property
        public static readonly DependencyProperty HashtagsProperty =
            DependencyProperty.Register(nameof(Hashtags), typeof(List<string>), typeof(PostComponent), 
            new PropertyMetadata(new List<string>(), OnPropertyChanged));

        public List<string> Hashtags
        {
            get { return (List<string>)GetValue(HashtagsProperty); }
            set { SetValue(HashtagsProperty, value); }
        }

        // Full Content Property
        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register(nameof(ContentText), typeof(string), typeof(PostComponent), 
            new PropertyMetadata(string.Empty, OnContentTextChanged));

        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        // Truncated Content Property
        public static readonly DependencyProperty TruncatedContentProperty =
            DependencyProperty.Register(nameof(TruncatedContent), typeof(string), typeof(PostComponent), 
            new PropertyMetadata(string.Empty));

        public string TruncatedContent
        {
            get { return (string)GetValue(TruncatedContentProperty); }
            private set { SetValue(TruncatedContentProperty, value); }
        }

        // Like Count Property
        public static readonly DependencyProperty LikeCountProperty =
            DependencyProperty.Register(nameof(LikeCount), typeof(int), typeof(PostComponent), 
            new PropertyMetadata(0, OnPropertyChanged));

        public int LikeCount
        {
            get { return (int)GetValue(LikeCountProperty); }
            set { SetValue(LikeCountProperty, value); }
        }

        public PostComponent()
        {
            this.InitializeComponent();
            
            // Initialize properties when loaded
            this.Loaded += PostComponent_Loaded;
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PostComponent component)
            {
                Debug.WriteLine($"Property changed from '{e.OldValue}' to '{e.NewValue}'");
            }
        }

        private void PostComponent_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("PostComponent loaded with the following values:");
            Debug.WriteLine($"  Username: {Username}");
            Debug.WriteLine($"  Title: {Title}");
            Debug.WriteLine($"  ContentText: {ContentText}");
            Debug.WriteLine($"  UtcDate: {UtcDate}");
            Debug.WriteLine($"  Hashtags: {(Hashtags != null ? string.Join(", ", Hashtags) : "null")}");
            
            // Re-process content text to ensure truncated content is set
            if (!string.IsNullOrEmpty(ContentText))
            {
                UpdateTruncatedContent(ContentText);
            }
            else
            {
                Debug.WriteLine("WARNING: ContentText is empty or null");
            }
            
            // Re-process date to ensure formatted date is set
            if (!string.IsNullOrEmpty(UtcDate))
            {
                UpdateFormattedDate(UtcDate);
            }
            else
            {
                Debug.WriteLine("WARNING: UtcDate is empty or null");
            }
        }

        private static void OnUtcDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PostComponent postComponent && e.NewValue is string utcDateString)
            {
                Debug.WriteLine($"UtcDate changed to: {utcDateString}");
                postComponent.UpdateFormattedDate(utcDateString);
            }
        }

        private void UpdateFormattedDate(string utcDateString)
        {
            if (string.IsNullOrEmpty(utcDateString))
            {
                FormattedDate = string.Empty;
                return;
            }

            DateTime result;
            if (DateTimeHelper.TryParseDateTime(utcDateString, out result))
            {
                result = DateTimeHelper.ConvertUtcToLocal(result);
                FormattedDate = DateTimeHelper.GetRelativeTime(result);
                Debug.WriteLine($"Formatted date: {FormattedDate}");
            }
            else
            {
                FormattedDate = "Unknown Date";
                Debug.WriteLine($"Could not parse date: {utcDateString}");
            }
        }

        private static void OnContentTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PostComponent postComponent && e.NewValue is string content)
            {
                Debug.WriteLine($"ContentText changed to: {content}");
                postComponent.UpdateTruncatedContent(content);
            }
        }

        private void UpdateTruncatedContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                TruncatedContent = string.Empty;
                return;
            }

            // Truncate content to 1000 characters as requested
            if (content.Length > 1000)
            {
                TruncatedContent = content.Substring(0, 1000) + "...";
            }
            else
            {
                TruncatedContent = content;
            }
            
            Debug.WriteLine($"TruncatedContent set to: {TruncatedContent}");
        }
    }
}
