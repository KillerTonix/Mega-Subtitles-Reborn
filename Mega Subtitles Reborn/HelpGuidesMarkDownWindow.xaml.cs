using Markdig;
using System.IO;
using System.Windows;


namespace Mega_Subtitles_Reborn
{
    public partial class HelpGuidesMarkDownWindow : Window
    {
        public HelpGuidesMarkDownWindow(string markdownFilePath)
        {
            InitializeComponent();
            string markdownPath = markdownFilePath;
            if (File.Exists(markdownPath))
            {
                string markdown = File.ReadAllText(markdownPath);
                string html = Markdown.ToHtml(markdown);

                // Wrap in basic HTML tags
                string fullHtml = $@"
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <style>
                            body {{
                                background-color: #1e1e1e;
                                color: #f0f0f0;
                                font-family: Consolas, monospace;
                                padding: 20px;
                            }}
                            h1, h2, h3 {{
                                color: #ffd700;
                            }}
                            code {{
                                background-color: #333;
                                padding: 2px 4px;
                                border-radius: 3px;
                            }}
                            pre {{
                                background-color: #2d2d2d;
                                padding: 10px;
                                border-radius: 5px;
                                overflow-x: auto;
                            }}
                        </style>
                    </head>
                    <body>{html}</body>
                    </html>";
                MarkdownWebBrowser.NavigateToString(fullHtml);
            }
        }

    }
}
