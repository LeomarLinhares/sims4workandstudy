using System;
using System.Windows.Forms;

namespace Sims_4_Work___Study
{
    public partial class MainForm : Form
    {
        private WindowFocusMonitor _windowFocusMonitor;
        private AudioManager _audioManager;

        public MainForm()
        {
            InitializeComponent();
            _windowFocusMonitor = new WindowFocusMonitor();
            _audioManager = new AudioManager();


            _windowFocusMonitor.OnFocusChanged = () =>
            {
                _audioManager.OnWindowFocusChanged();
            };

            TrayIcon.DoubleClick += TrayIcon_DoubleClick;
            ContextTrayAbrir.Click += TrayIcon_DoubleClick;
            ContextTraySair.Click += sairToolStripMenuItem_Click;
            TrayIcon.ContextMenuStrip = ContextMenuStripFromTray;
            this.Load += MainFormLoad;
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;


            _audioManager.LoadRandomMusicFolder("songs");
            _audioManager.InitializePlayback();
            _windowFocusMonitor.StartMonitoring();

        }

        public void OnForegroundWindowChanged()
        {
            _audioManager.OnWindowFocusChanged();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _audioManager.StopAll();
            _windowFocusMonitor.StopMonitoring();
            Application.Exit();
        }
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.Activate();
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;

                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;

                TrayIcon.Visible = true;
            }
        }
    }
}
