using System;
using System.Windows.Forms;

namespace Sims_4_Work___Study
{
    public partial class MainForm : Form
    {
        private WindowFocusMonitor _windowFocusMonitor;
        private CSAudioManager _audioManager;


        public MainForm()
        {
            InitializeComponent();
            _windowFocusMonitor = new WindowFocusMonitor();
            _audioManager = new CSAudioManager();


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
            _audioManager.PlaybackFinished += OnPlaybackFinished;
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            _audioManager.SetBasePath("songs");
            _audioManager.LoadRandomMusicFolder();
            _audioManager.InitializePlayback();
            _windowFocusMonitor.StartMonitoring();

        }

        public void OnForegroundWindowChanged()
        {
            _audioManager.OnWindowFocusChanged();
        }

        private void OnPlaybackFinished(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnPlaybackFinished(sender, e)));
                return;
            }

            _audioManager.ClearAll();
            _audioManager.CreateMixerIfNeeded();
            _audioManager.LoadRandomMusicFolder();
            _audioManager.InitializePlayback();

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

        // --------------------------------------------
        // Métodos para DEBUG
        // --------------------------------------------
        private void btnSkip_Click(object sender, EventArgs e)
        {
            _audioManager.SkipToNearEnd(5.0);
        }
    }
}
