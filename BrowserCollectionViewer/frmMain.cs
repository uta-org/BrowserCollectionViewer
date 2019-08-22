using System;
using System.Windows.Forms;

namespace BrowserCollectionViewer
{
    public partial class frmMain : Form
    {
        private static frmMain Instance;

        public static bool SaveSettings { get; } =
            Instance?.saveSettingsToolStripMenuItem?.CheckState == CheckState.Checked;

        private static int? Length { get; } = frmAnalyzer.Urls?.Length;
        private static bool IsReady { get; } = Length.HasValue && Length.Value > 0;

        private static int SelectedIndex;

        public frmMain()
        {
            InitializeComponent();
            Instance = this;
        }

        private void analyzeURLsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmAnalyzer();
            form.ShowDialog();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (!IsReady)
                return;

            --SelectedIndex;

            if (SelectedIndex < 0)
                SelectedIndex = Length.Value - 1;

            Navigate();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!IsReady)
                return;

            ++SelectedIndex;

            if (SelectedIndex >= Length)
                SelectedIndex = 0;

            Navigate();
        }

        private void Navigate()
        {
            webBrowser1.Navigate(frmAnalyzer.Urls[SelectedIndex]);
        }
    }
}