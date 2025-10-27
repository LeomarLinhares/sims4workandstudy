using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace Sims_4_Work___Study
{
    /// <summary>
    /// Model para representar uma música na UI
    /// </summary>
    public class MusicFolderViewModel : INotifyPropertyChanged
    {
        private string _folderPath = string.Empty;
        private string _displayName = string.Empty;
        private string _status = string.Empty;
        private bool _isCurrentlyPlaying;

        public string FolderPath
        {
            get => _folderPath;
            set { _folderPath = value; OnPropertyChanged(nameof(FolderPath)); }
        }

        public string DisplayName
        {
            get => _displayName;
            set { _displayName = value; OnPropertyChanged(nameof(DisplayName)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public bool IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set { _isCurrentlyPlaying = value; OnPropertyChanged(nameof(IsCurrentlyPlaying)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowFocusMonitor _windowFocusMonitor;
        private CSAudioManager _audioManager;
        private bool _isPaused = false;
        private DispatcherTimer _updateTimer;
        private ObservableCollection<MusicFolderViewModel> _musicFolders;
        private NotifyIcon? _trayIcon;
        private static string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");

        private static void LogError(string message)
        {
            try
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n\n");
            }
            catch { }
        }

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                LogError("InitializeComponent OK");
            }
            catch (Exception ex)
            {
                string errorMsg = $"Erro no InitializeComponent:\n\n{ex.Message}\n\n{ex.InnerException?.Message}\n\nStack Trace:\n{ex.StackTrace}";
                LogError(errorMsg);
                
                System.Windows.MessageBox.Show(
                    errorMsg,
                    "Erro no XAML",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }

            try
            {
                // Inicializa coleção de músicas
                LogError("Iniciando coleção de músicas");
                _musicFolders = new ObservableCollection<MusicFolderViewModel>();
                lstMusicFolders.ItemsSource = _musicFolders;

                // Inicializa componentes de áudio
                LogError("Criando WindowFocusMonitor");
                _windowFocusMonitor = new WindowFocusMonitor();
                LogError("Criando CSAudioManager");
                _audioManager = new CSAudioManager();

                // Timer para atualizar UI periodicamente
                LogError("Criando DispatcherTimer");
                _updateTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(500)
                };
                _updateTimer.Tick += UpdateTimer_Tick;

                // Configurar eventos
                LogError("Configurando eventos");
                _windowFocusMonitor.OnFocusChanged = () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        _audioManager.OnWindowFocusChanged();
                        UpdateChannelList();
                    });
                };

                _audioManager.PlaybackFinished += OnPlaybackFinished;

                // Configurar tray icon
                LogError("Configurando tray icon");
                SetupTrayIcon();

                // Iniciar aplicação
                LogError("Registrando evento Loaded");
                Loaded += MainWindow_Loaded;
                
                LogError("Construtor concluído com sucesso");
            }
            catch (Exception ex)
            {
                string errorMsg = $"Erro no construtor:\n\n{ex.Message}\n\n{ex.InnerException?.Message}\n\nStack Trace:\n{ex.StackTrace}";
                LogError(errorMsg);
                
                System.Windows.MessageBox.Show(
                    errorMsg,
                    "Erro de Inicialização",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        private void SetupTrayIcon()
        {
            try
            {
                LogError("Criando NotifyIcon");
                _trayIcon = new NotifyIcon
                {
                    Visible = true,
                    Text = "The Sims 4 Work & Study"
                };

                // Tentar carregar o ícone, se falhar usa o ícone padrão da aplicação
                try
                {
                    LogError("Tentando carregar ícone");
                    string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "icon.ico");
                    LogError($"Caminho do ícone: {iconPath}");
                    if (System.IO.File.Exists(iconPath))
                    {
                        LogError("Ícone encontrado, carregando");
                        _trayIcon.Icon = new Icon(iconPath);
                    }
                    else
                    {
                        LogError("Ícone não encontrado, usando ícone padrão");
                        _trayIcon.Icon = SystemIcons.Application;
                    }
                }
                catch (Exception iconEx)
                {
                    LogError($"Erro ao carregar ícone: {iconEx.Message}");
                    _trayIcon.Icon = SystemIcons.Application;
                }

                LogError("Configurando eventos do tray icon");
                _trayIcon.DoubleClick += (s, e) =>
                {
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                };
                LogError("SetupTrayIcon concluído com sucesso");
            }
            catch (Exception ex)
            {
                LogError($"Erro em SetupTrayIcon: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Carregar configurações salvas
                volumeSlider.Value = Properties.Settings.Default.volume;
                txtChangeChannelChance.Text = Properties.Settings.Default.change_channel_chance.ToString();
                
                // NÃO minimiza no início para debug
                // WindowState = WindowState.Minimized;
                // Hide();

                _audioManager.SetBasePath("songs");
                LoadMusicList(); // Carrega lista de todas as músicas
                _audioManager.LoadRandomMusicFolder();
                _audioManager.InitializePlayback();
                _windowFocusMonitor.StartMonitoring();

                UpdateMusicInfo();

                _updateTimer.Start();
                
                // Carregar ícones da barra de tarefas
                LoadTaskbarIcons();

                if (_trayIcon != null)
                {
                    _trayIcon.ShowBalloonTip(3000,
                        "The Sims 4 Work & Study",
                        "O programa está rodando.",
                        ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {
                // Mostra janela em caso de erro
                Show();
                WindowState = WindowState.Normal;
                
                System.Windows.MessageBox.Show(
                    $"Erro ao inicializar o programa:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Erro de Inicialização",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                    
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            UpdateMusicInfo();
        }
        
        private void LoadTaskbarIcons()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string assetsPath = Path.Combine(basePath, "assets");
                
                // Carregar ícones
                thumbPrevious.ImageSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(Path.Combine(assetsPath, "previous.png")));
                thumbPlayPause.ImageSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(Path.Combine(assetsPath, "pause.png")));
                thumbNext.ImageSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(Path.Combine(assetsPath, "next.png")));
                    
                LogError("Ícones da barra de tarefas carregados com sucesso");
            }
            catch (Exception ex)
            {
                LogError($"Erro ao carregar ícones da barra de tarefas: {ex.Message}");
            }
        }
        
        private void LoadMusicList()
        {
            try
            {
                string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "songs");
                if (!Directory.Exists(basePath))
                    return;

                _musicFolders.Clear();
                var folders = Directory.GetDirectories(basePath).OrderBy(f => f).ToList();
                
                foreach (var folder in folders)
                {
                    string folderName = Path.GetFileName(folder);
                    _musicFolders.Add(new MusicFolderViewModel
                    {
                        FolderPath = folder,
                        DisplayName = $"Música {folderName}",
                        Status = "",
                        IsCurrentlyPlaying = false
                    });
                }
            }
            catch (Exception ex)
            {
                LogError($"Erro ao carregar lista de músicas: {ex.Message}");
            }
        }

        private void UpdateMusicInfo()
        {
            try
            {
                // Obter informações da música atual
                string? currentFolder = _audioManager?.GetCurrentMusicFolder();
                
                if (!string.IsNullOrEmpty(currentFolder))
                {
                    string folderName = Path.GetFileName(currentFolder);
                    txtMusicName.Text = $"Música {folderName}";
                    
                    // Atualizar status na lista
                    foreach (var music in _musicFolders)
                    {
                        music.IsCurrentlyPlaying = music.FolderPath == currentFolder;
                        music.Status = music.IsCurrentlyPlaying ? "▶ Tocando" : "";
                    }
                    
                    // Selecionar item atual na lista
                    var selectedItem = _musicFolders.FirstOrDefault(m => m.FolderPath == currentFolder);
                    if (selectedItem != null)
                    {
                        lstMusicFolders.SelectedItem = selectedItem;
                        lstMusicFolders.ScrollIntoView(selectedItem);
                    }
                }
                else
                {
                    txtMusicName.Text = "Nenhuma música carregada";
                }
                
                // Atualizar contador
                var currentItem = _musicFolders.FirstOrDefault(m => m.IsCurrentlyPlaying);
                int currentIndex = currentItem != null ? _musicFolders.IndexOf(currentItem) + 1 : 0;
                int total = _musicFolders.Count;
                txtMusicNumber.Text = $"{currentIndex} / {total}";
            }
            catch (Exception ex)
            {
                LogError($"Erro ao atualizar informações da música: {ex.Message}");
            }
        }

        private void UpdateChannelList()
        {
            // Removida - não usamos mais lista de canais
        }

        private void OnPlaybackFinished(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _audioManager.ClearAll();
                _audioManager.CreateMixerIfNeeded();
                _audioManager.LoadRandomMusicFolder();
                _audioManager.InitializePlayback();

                UpdateMusicInfo();
            });
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_audioManager != null)
            {
                float newVolume = (float)(volumeSlider.Value / 100.0);
                _audioManager.SetMainTrackVolume(newVolume);
            }
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string assetsPath = Path.Combine(basePath, "assets");
            
            if (_isPaused)
            {
                _audioManager.ResumePlayback();
                iconPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                txtPlayPause.Text = "Pausar";
                _isPaused = false;
                
                // Atualiza ícone da barra de tarefas
                thumbPlayPause.ImageSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(Path.Combine(assetsPath, "pause.png")));
            }
            else
            {
                _audioManager.PausePlayback();
                iconPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                txtPlayPause.Text = "Continuar";
                _isPaused = true;
                
                // Atualiza ícone da barra de tarefas
                thumbPlayPause.ImageSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(Path.Combine(assetsPath, "play.png")));
            }

            UpdateMusicInfo();
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            _audioManager.PreviousSong();
            UpdateMusicInfo();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            _audioManager.NextSong();
            UpdateMusicInfo();
        }

        private void TxtChangeChannelChance_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Verifica se o audioManager já foi inicializado
            if (_audioManager == null) return;
            
            if (int.TryParse(txtChangeChannelChance.Text, out int value))
            {
                _audioManager.chanceTarget = value;
                Properties.Settings.Default.change_channel_chance = value;
                Properties.Settings.Default.Save();
            }
        }
        
        // Handlers para os botões da taskbar (ThumbButtonInfo)
        private void TaskbarPrevious_Click(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => BtnPrevious_Click(sender, new RoutedEventArgs()));
        }
        
        private void TaskbarPlayPause_Click(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => BtnPlayPause_Click(sender, new RoutedEventArgs()));
        }
        
        private void TaskbarNext_Click(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => BtnNext_Click(sender, new RoutedEventArgs()));
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                _trayIcon?.ShowBalloonTip(2000,
                    "The Sims 4 Work & Study",
                    "O programa foi minimizado para a bandeja do sistema.",
                    ToolTipIcon.Info);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                "Deseja encerrar o aplicativo?\n\nSim: Encerrar completamente\nNão: Minimizar para a bandeja do sistema",
                "The Sims 4 Work & Study",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _updateTimer?.Stop();
                _audioManager?.StopAll();
                _windowFocusMonitor?.StopMonitoring();
                
                if (_trayIcon != null)
                {
                    _trayIcon.Visible = false;
                    _trayIcon.Dispose();
                }
            }
            else
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
            }
        }
    }
}
