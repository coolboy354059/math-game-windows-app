using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace MathGameWindowsApp
{
    public class MainForm : Form
    {
        private readonly WebView2 webView;

        public MainForm()
        {
            Text = "Math Game";
            Width = 640;
            Height = 440;

            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(webView);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // WebView2 has to finish starting up before we can navigate.
            await webView.EnsureCoreWebView2Async();

            // Load the game page that sits next to the .exe.
            string indexPath = Path.Combine(AppContext.BaseDirectory, "index.html");
            webView.CoreWebView2.Navigate(new Uri(indexPath).AbsoluteUri);
        }
    }
}
