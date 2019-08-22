using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BrowserCollectionViewer.Properties;
using static WinFormsEvents.WinFormsEvents;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BrowserCollectionViewer
{
    public partial class frmAnalyzer : Form
    {
        private static Settings Settings { get; } = Settings.Default;
        private ToolTipHandler<TextBox> toolTipForTxtSearcher { get; set; }

        private static string NullSearchText { get; } = "A URL must be typed here.";
        private static string InvalidSearchText { get; } = "The format of this text isn't a URL.";
        public static string[] Urls { get; private set; }

        private static bool LastAllChecked;

        public frmAnalyzer()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            btnSearch.On<MouseEventHandler>(ButtonEvents.MouseClick, (s, _e) =>
            {
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    F.CreateToolTip("Error", ToolTipIcon.Error).Show(NullSearchText, txtSearch);
                }
                else
                {
                    string url = txtSearch.Text;

                    if (!url.IsValidURL())
                    {
                        F.CreateToolTip("Error", ToolTipIcon.Error).Show(InvalidSearchText, txtSearch);
                    }
                    else
                    {
                        clbUrlList.Items.RemoveAll();

                        var uri = new Uri(url);

                        string source;
                        using (var wc = new WebClient())
                            source = wc.DownloadString(url);

                        var doc = new HtmlDocument();
                        doc.LoadHtml(source);

                        var html = doc.DocumentNode;
                        var links = html
                            .Descendants()
                            .Where(item => item.Name.ToLowerInvariant() == "a")
                            .Select(link => link.GetAttributeValue("href", string.Empty))
                            .Where(text => !string.IsNullOrEmpty(text));

                        if (!string.IsNullOrEmpty(Settings.SearchPattern))
                        {
                            try
                            {
                                links = links.Select(item => Regex.Match(item, Settings.SearchPattern).Value);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        links = links.Select(item =>
                            item.IsAbsoluteUrl() ? item : uri.Scheme + Uri.SchemeDelimiter + uri.Host + item);

                        object[] values;
                        if (searchOnGoogleToolStripMenuItem.CheckState == CheckState.Checked)
                        {
                            var handlers = links.Select(link =>
                                new LinkHandler(link, $"https://google.com/?q={WebUtility.UrlEncode(link)}"));

                            Urls = handlers.Select(h => h.GoogleQuery).ToArray();
                            values = handlers.ToArray();
                        }
                        else
                        {
                            Urls = links.ToArray();
                            values = links.Cast<object>().ToArray();
                        }

                        clbUrlList.Items.AddRange(values);
                        btnSelectToggle.Enabled = true;

                        Settings.SearchHandle = txtSearch.Text;

                        if (frmMain.SaveSettings)
                            Settings.Save();
                    }
                }
            });
        }

        private void setRegexExpressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.SearchPattern = Interaction.InputBox("Set regular expression to filter results", "Prompt", string.Empty);

            if (frmMain.SaveSettings)
                Settings.Save();
        }

        private void btnSelectToggle_Click(object sender, EventArgs e)
        {
            if (clbUrlList.Items.Count == 0)
                return;

            clbUrlList.ToggleAll(!LastAllChecked);

            //bool isAllSelected = clbUrlList.CheckedItems.Count == clbUrlList.Items.Count;
            btnSelectToggle.Text = !LastAllChecked ? "Deselect All" : "Select All";

            LastAllChecked = !LastAllChecked;
        }

        private void searchOnGoogleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.SearchOnGoogle = searchOnGoogleToolStripMenuItem.CheckState == CheckState.Checked;

            if (frmMain.SaveSettings)
                Settings.Save();
        }
    }

    public class ToolTipHandler<T>
        where T : Control
    {
        public ToolTip ToolTip { get; }
        public string Text { get; }
        public T OwnerControl { get; }

        private ToolTipHandler()
        {
        }

        public ToolTipHandler(T control, string text, ToolTip toolTip)
        {
            OwnerControl = control;
            ToolTip = toolTip;
            Text = text;
        }

        public void Show()
        {
            ToolTip.Show(Text, OwnerControl.GetWindowFromHandle());
        }
    }

    public class LinkHandler
    {
        public string Url { get; }
        public string GoogleQuery { get; }

        private LinkHandler()
        {
        }

        public LinkHandler(string url, string googleQuery)
        {
            Url = url;
            GoogleQuery = googleQuery;
        }

        public override string ToString()
        {
            return Url;
        }
    }
}