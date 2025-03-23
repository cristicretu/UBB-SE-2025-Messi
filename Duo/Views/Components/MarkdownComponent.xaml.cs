using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Threading.Tasks;

namespace Duo.Views.Components
{
    public sealed partial class MarkdownComponent : UserControl, INotifyPropertyChanged
    {
        private bool _isWebViewInitialized = false;
        private string _pendingMarkdownText = null;

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
            }
        }

        // Always visible now, keeping for compatibility
        private bool _previewVisible = true;
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
            PreviewVisible = true;
            InitializeWebView();
            
            // Subscribe to theme changes
            ActualThemeChanged += MarkdownComponent_ActualThemeChanged;
        }

        private void MarkdownComponent_ActualThemeChanged(FrameworkElement sender, object args)
        {
            // Re-render markdown when theme changes
            if (_isWebViewInitialized)
            {
                RenderMarkdown();
            }
        }

        private async void InitializeWebView()
        {
            try
            {
                await MarkdownPreview.EnsureCoreWebView2Async();
                _isWebViewInitialized = true;
                
                // If we have pending markdown to render
                if (_pendingMarkdownText != null)
                {
                    // Store the text in the input
                    MarkdownInput.Text = _pendingMarkdownText;
                    _pendingMarkdownText = null;
                    
                    // Now render it
                    RenderMarkdown();
                }
                else if (!string.IsNullOrEmpty(MarkdownText))
                {
                    // If markdown text was set from property
                    MarkdownInput.Text = MarkdownText;
                    RenderMarkdown();
                }
            }
            catch (Exception ex)
            {
                // Handle any exception during initialization
                System.Diagnostics.Debug.WriteLine($"WebView2 initialization error: {ex.Message}");
            }
        }

        public void TogglePreview()
        {
            // Kept for compatibility, no longer used
            if (_isWebViewInitialized)
            {
                RenderMarkdown();
            }
        }

        public void ClearMarkdown()
        {
            MarkdownInput.Text = string.Empty;
            MarkdownText = string.Empty;
            
            if (_isWebViewInitialized)
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
            if (_isWebViewInitialized)
            {
                MarkdownInput.Text = text;
                RenderMarkdown();
            }
            else
            {
                // Store the text for when WebView gets initialized
                _pendingMarkdownText = text;
            }
        }

        private void RenderMarkdown()
        {
            // Safety check to prevent calling NavigateToString before the WebView is initialized
            if (!_isWebViewInitialized)
            {
                return;
            }
            
            string markdownText = MarkdownInput.Text ?? string.Empty;
            bool isDarkTheme = IsDarkTheme();

            string html = $@"
                <!DOCTYPE html>
                <html data-theme=""{(isDarkTheme ? "dark" : "light")}"">
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1'>
                    <title>Markdown Preview</title>
                    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/github-markdown-css/github-markdown.min.css'>
                    <script src='https://cdn.jsdelivr.net/npm/marked/marked.min.js'></script>
                    <style>
                        body {{
                            margin: 0;
                            padding: 0;
                            background-color: transparent;
                            color: {(isDarkTheme ? "#ffffff" : "#000000")};
                        }}
                        
                        html[data-theme='dark'] {{
                            color-scheme: dark;
                            background-color: transparent;
                        }}
                        
                        html[data-theme='light'] {{
                            color-scheme: light;
                            background-color: transparent;
                        }}
                        
                        .markdown-body {{
                            box-sizing: border-box;
                            margin: 0;
                            padding: 6px 0;
                            color: {(isDarkTheme ? "#d4d4d4" : "#24292e")};
                            background-color: transparent !important;
                        }}
                        
                        .markdown-body a {{
                            color: {(isDarkTheme ? "#6cb5ff" : "#0366d6")};
                        }}
                        
                        .markdown-body hr {{
                            background-color: {(isDarkTheme ? "#444" : "#e1e4e8")};
                        }}
                        
                        .markdown-body blockquote {{
                            color: {(isDarkTheme ? "#bbb" : "#6a737d")};
                            border-left-color: {(isDarkTheme ? "#444" : "#dfe2e5")};
                            background-color: transparent;
                        }}
                        
                        .markdown-body code {{
                            background-color: {(isDarkTheme ? "#2b2b2b99" : "#f6f8fa99")};
                        }}
                        
                        .markdown-body pre {{
                            background-color: {(isDarkTheme ? "#2b2b2b" : "#f6f8fa")};
                        }}
                        
                        .markdown-body h1, .markdown-body h2, .markdown-body h3, 
                        .markdown-body h4, .markdown-body h5, .markdown-body h6 {{
                            margin-top: 12px;
                            margin-bottom: 10px;
                        }}
                        
                        .markdown-body p, .markdown-body ul, .markdown-body ol {{
                            margin-top: 6px;
                            margin-bottom: 6px;
                        }}
                        
                        /* Override any potential github-markdown-css backgrounds */
                        .markdown-body table tr, .markdown-body table th, .markdown-body table td {{
                            background-color: transparent !important;
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

            try
            {
                MarkdownPreview.NavigateToString(html);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rendering markdown: {ex.Message}");
            }
        }

        // Helper method to determine if in dark theme
        private bool IsDarkTheme()
        {
            return ActualTheme == ElementTheme.Dark;
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
