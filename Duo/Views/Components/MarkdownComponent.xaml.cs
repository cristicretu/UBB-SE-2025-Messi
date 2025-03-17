using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Duo.Views.Components
{
    public sealed partial class MarkdownComponent : UserControl, INotifyPropertyChanged
    {
        // Dependency property for markdown text
        public static readonly DependencyProperty MarkdownTextProperty =
            DependencyProperty.Register(
                nameof(MarkdownText),
                typeof(string),
                typeof(MarkdownComponent),
                new PropertyMetadata(string.Empty, OnMarkdownTextChanged));

        // Property for markdown text
        public string MarkdownText
        {
            get => (string)GetValue(MarkdownTextProperty);
            set => SetValue(MarkdownTextProperty, value);
        }

        // Called when MarkdownText property changes
        private static void OnMarkdownTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MarkdownComponent component && e.NewValue is string newText)
            {
                component.SetMarkdownText(newText);
                // Show preview automatically when text is provided
                if (!string.IsNullOrEmpty(newText) && !component.PreviewVisible)
                {
                    component.PreviewVisible = true;
                }
            }
        }

        private bool _previewVisible = false;
        public bool PreviewVisible
        {
            get => _previewVisible;
            set
            {
                if (_previewVisible != value)
                {
                    _previewVisible = value;
                    OnPropertyChanged(nameof(PreviewVisible));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MarkdownComponent()
        {
            this.InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await MarkdownPreview.EnsureCoreWebView2Async();
            
            // If markdown text was set before WebView was initialized
            if (!string.IsNullOrEmpty(MarkdownText))
            {
                SetMarkdownText(MarkdownText);
                PreviewVisible = true;
            }
        }

        public void TogglePreview()
        {
            PreviewVisible = !PreviewVisible;
            if (PreviewVisible)
            {
                RenderMarkdown();
            }
        }

        public void ClearMarkdown()
        {
            MarkdownInput.Text = string.Empty;
            MarkdownText = string.Empty;
            if (PreviewVisible)
            {
                RenderMarkdown();
            }
        }

        public string GetMarkdownText()
        {
            return MarkdownInput.Text ?? string.Empty;
        }

        public void SetMarkdownText(string text)
        {
            MarkdownInput.Text = text;
            if (PreviewVisible)
            {
                RenderMarkdown();
            }
        }

        private void RenderMarkdown()
        {
            string markdownText = MarkdownInput.Text ?? string.Empty;

            string html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1'>
                    <title>Markdown Preview</title>
                    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/github-markdown-css/github-markdown.min.css'>
                    <script src='https://cdn.jsdelivr.net/npm/marked/marked.min.js'></script>
                    <style>
                        .markdown-body {{
                            box-sizing: border-box;
                            min-width: 200px;
                            max-width: 980px;
                            margin: 0 auto;
                            padding: 15px;
                        }}
                    </style>
                </head>
                <body>
                    <div id='content' class='markdown-body'></div>
                    <script>
                        document.getElementById('content').innerHTML = marked.parse(`{EscapeJavaScriptString(markdownText)}`);
                    </script>
                </body>
                </html>";

            MarkdownPreview.NavigateToString(html);
        }

        private string EscapeJavaScriptString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input
                .Replace("\\", "\\\\")
                .Replace("`", "\\`")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }
    }
}
