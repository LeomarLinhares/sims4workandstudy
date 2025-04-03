using System;
using System.Windows.Forms;

namespace Sims_4_Work___Study
{
    public partial class MainForm : Form
    {
        private WindowFocusMonitor _windowFocusMonitor;
        private CSAudioManager _audioManager;
        private bool isPaused = false;

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
            playPauseButton.Click += btnPauseResume_Click;
            trackBarMainVolume.ValueChanged += trackBarMainVolume_ValueChanged;
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

        private void trackBarMainVolume_ValueChanged(object sender, EventArgs e)
        {
            // Converte o valor do TrackBar (0 a 100) para um float entre 0.0 e 1.0
            float newVolume = trackBarMainVolume.Value / 100f;
            _audioManager.SetMainTrackVolume(newVolume);
        }

        private void btnPauseResume_Click(object sender, EventArgs e)
        {
            if (isPaused)
            {
                _audioManager.ResumePlayback();
                playPauseButton.Text = "Pausar";
                isPaused = false;
            }
            else
            {
                _audioManager.PausePlayback();
                playPauseButton.Text = "Continuar";
                isPaused = true;
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
