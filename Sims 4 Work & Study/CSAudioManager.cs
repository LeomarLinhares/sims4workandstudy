using System;
using System.Diagnostics;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using CSCore.Streams;
using Sims_4_Work___Study;
using Sims_4_Work___Study.Properties;

public class CSAudioManager
{
    private readonly AudioConfiguration config;
    private readonly MusicLibraryManager libraryManager;
    private readonly ChannelFadeService fadeService;
    private readonly Random random;

    private ISoundOut? outputDevice;
    private SimpleMixer? mixer;
    private MusicTrack? currentTrack;

    // Armazena o volume do usuário para preservar entre músicas
    private float userVolume = 1.0f;

    public event EventHandler? PlaybackFinished;
    public int chanceTarget = Settings.Default.change_channel_chance;

    public CSAudioManager() : this(AudioConfiguration.Default)
    {
    }

    public CSAudioManager(AudioConfiguration configuration)
    {
        config = configuration ?? AudioConfiguration.Default;
        libraryManager = new MusicLibraryManager();
        fadeService = new ChannelFadeService(config.FadeSteps, config.FadeIntervalMs);
        random = new Random();

        // Inscreve no evento de mudança de música para preservar volume
        libraryManager.MusicChanged += OnMusicChanged;

        InitializeMixer();
        InitializeOutputDevice();
    }

    /// <summary>
    /// Chamado quando a música muda. Preserva o volume do usuário.
    /// </summary>
    private void OnMusicChanged(string? oldFolder, string newFolder)
    {
        Debug.WriteLine($"Música mudou de '{oldFolder}' para '{newFolder}'. Preservando volume: {userVolume}");
    }

    private void InitializeMixer()
    {
        mixer = new SimpleMixer(config.AudioChannels, config.SampleRate)
        {
            FillWithZeros = config.MixerFillWithZeros,
            DivideResult = config.MixerDivideResult
        };

        mixer.PlaybackFinished += (s, e) =>
        {
            PlaybackFinished?.Invoke(this, EventArgs.Empty);
        };
    }

    private void InitializeOutputDevice()
    {
        outputDevice = new WasapiOut();
    }

    public void SetBasePath(string basePath)
    {
        libraryManager.Initialize(basePath);
    }
    
    public string? GetCurrentMusicFolder()
    {
        return currentTrack?.FolderPath;
    }

    public TimeSpan GetCurrentPosition()
    {
        if (currentTrack != null && currentTrack.Channels.Count > 0)
        {
            var channel = currentTrack.Channels[0];
            if (channel.AudioSource != null && channel.AudioSource.CanSeek)
            {
                long position = channel.AudioSource.Position;
                long bytesPerSecond = channel.AudioSource.WaveFormat.BytesPerSecond;
                return TimeSpan.FromSeconds((double)position / bytesPerSecond);
            }
        }
        return TimeSpan.Zero;
    }

    public TimeSpan GetCurrentLength()
    {
        if (currentTrack != null && currentTrack.Channels.Count > 0)
        {
            var channel = currentTrack.Channels[0];
            if (channel.AudioSource != null)
            {
                long length = channel.AudioSource.Length;
                long bytesPerSecond = channel.AudioSource.WaveFormat.BytesPerSecond;
                return TimeSpan.FromSeconds((double)length / bytesPerSecond);
            }
        }
        return TimeSpan.Zero;
    }

    public void SetPosition(TimeSpan position)
    {
        if (currentTrack == null || currentTrack.Channels.Count == 0)
        {
            throw new InvalidOperationException("Nenhuma música carregada.");
        }

        foreach (var channel in currentTrack.Channels)
        {
            if (channel.AudioSource != null && channel.AudioSource.CanSeek)
            {
                long bytesPerSecond = channel.AudioSource.WaveFormat.BytesPerSecond;
                long newPosition = (long)(position.TotalSeconds * bytesPerSecond);
                
                // Garante que a posição está dentro dos limites
                newPosition = Math.Max(0, Math.Min(newPosition, channel.AudioSource.Length));
                
                channel.AudioSource.Position = newPosition;
                Debug.WriteLine($"Posição do canal alterada para: {position}");
            }
        }
    }

    public void LoadRandomMusicFolder()
    {
        string folderPath = libraryManager.GetNextRandomMusic();
        LoadMusicFromFolder(folderPath);
    }

    private void LoadMusicFromFolder(string folderPath)
    {
        Debug.WriteLine($"Carregando música: {folderPath}");

        currentTrack = new MusicTrack(folderPath);

        string[] audioFiles = libraryManager.GetMusicFiles(
            folderPath, 
            config.AudioFileExtension, 
            config.ChannelsPerTrack);

        foreach (var filePath in audioFiles)
        {
            AddChannelToCurrentTrack(filePath);
        }

        if (currentTrack.ChannelCount == config.ChannelsPerTrack)
        {
            int initialChannel = random.Next(config.ChannelsPerTrack);
            currentTrack.SetActiveChannel(initialChannel, userVolume);
        }
    }

    private void AddChannelToCurrentTrack(string filePath)
    {
        if (currentTrack == null || mixer == null)
            return;

        IWaveSource audioSource = CodecFactory.Instance.GetCodec(filePath);

        var sampleSource = audioSource
            .ToSampleSource()
            .ToStereo();

        var volumeSource = new VolumeSource(sampleSource)
        {
            Volume = config.MinVolume
        };

        var channel = new AudioChannel(filePath, audioSource, volumeSource);

        currentTrack.AddChannel(channel);
        mixer.AddSource(volumeSource);
    }

    public void InitializePlayback()
    {
        if (mixer == null)
        {
            InitializeMixer();
        }

        if (outputDevice == null)
        {
            InitializeOutputDevice();
        }

        if (outputDevice != null && mixer != null)
        {
            outputDevice.Initialize(mixer.ToWaveSource(32));
            outputDevice.Play();
        }
    }

    public void StartPlayback()
    {
        if (outputDevice == null)
            InitializeOutputDevice();

        if (mixer == null)
            InitializeMixer();

        if (outputDevice != null && mixer != null)
        {
            outputDevice.Initialize(mixer.ToWaveSource(32));
            outputDevice.Play();
        }
    }

    public void OnWindowFocusChanged()
    {
        int chanceResult = random.Next(100);
        // Se o resultado aleatório for MAIOR OU IGUAL ao valor alvo, não troca
        // Exemplo: chanceTarget=20 significa 20% de chance de trocar (0-19), 80% de não trocar (20-99)
        if (chanceResult >= chanceTarget) 
            return;

        if (currentTrack == null || currentTrack.ChannelCount < 2)
            return;

        if (fadeService.IsFading)
            return;

        int newChannelIndex = currentTrack.GetRandomChannelIndexExcludingCurrent(random);

        var fadingOutChannel = currentTrack.ActiveChannel;
        var fadingInChannel = currentTrack.Channels[newChannelIndex];

        if (fadingOutChannel == null || fadingInChannel == null)
            return;

        fadeService.StartFade(fadingOutChannel, fadingInChannel, userVolume, userVolume);

        fadeService.FadeCompleted += (s, e) =>
        {
            if (currentTrack != null)
            {
                currentTrack.SetActiveChannel(newChannelIndex, userVolume);
            }
        };
    }

    public void StopAll()
    {
        fadeService?.StopFade();
        
        if (outputDevice != null)
        {
            try
            {
                // Para o mixer primeiro para evitar tentativas de leitura
                if (mixer != null)
                {
                    mixer.FillWithZeros = true;
                }

                // Se estiver rodando, pause primeiro (mais rápido que Stop)
                if (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    outputDevice.Pause();
                }

                // Pequeno delay para garantir que o pause foi processado
                System.Threading.Thread.Sleep(50);

                // Agora pare de forma segura
                outputDevice.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao parar reprodução: {ex.Message}");
            }
        }
    }

    public void PausePlayback()
    {
        outputDevice?.Pause();
    }

    public void ResumePlayback()
    {
        outputDevice?.Play();
    }

    public void SetMainTrackVolume(float volume)
    {
        userVolume = volume; // Salva o volume do usuário
        
        if (currentTrack != null)
        {
            currentTrack.SetActiveChannelVolume(volume);
        }
    }

    public void PreviousSong()
    {
        if (!libraryManager.HasPreviousMusic)
        {
            Debug.WriteLine("Nenhuma música anterior para reproduzir.");
            return;
        }

        string previousFolder = libraryManager.GoToPreviousMusic();
        Debug.WriteLine($"Reproduzindo música anterior: {previousFolder}");

        ReloadMusic(previousFolder);
    }

    public void NextSong()
    {
        string nextFolder = libraryManager.GoToNextMusic();
        Debug.WriteLine($"Carregando próxima música: {nextFolder}");

        ReloadMusic(nextFolder);
    }

    private void ReloadMusic(string folderPath)
    {
        fadeService?.StopFade();
        
        // Descarta o dispositivo antigo completamente
        if (outputDevice != null)
        {
            try
            {
                if (outputDevice.PlaybackState != PlaybackState.Stopped)
                {
                    outputDevice.Pause();
                    System.Threading.Thread.Sleep(50);
                }
                outputDevice.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao descartar dispositivo de áudio: {ex.Message}");
            }
            outputDevice = null;
        }
        
        ClearMixer();
        
        if (currentTrack != null)
        {
            currentTrack.Dispose();
            currentTrack = null;
        }

        // Cria novo dispositivo
        InitializeOutputDevice();
        CreateMixerIfNeeded();
        LoadMusicFromFolder(folderPath);
        InitializePlayback();
    }

    public void ClearMixer()
    {
        if (mixer != null)
        {
            mixer.Dispose();
            mixer = null;
        }
    }

    public void CreateMixerIfNeeded()
    {
        if (mixer == null)
        {
            InitializeMixer();
        }
    }

    public void ClearAll()
    {
        StopAll();

        if (outputDevice != null)
        {
            outputDevice.Dispose();
            outputDevice = null;
        }

        ClearMixer();

        if (currentTrack != null)
        {
            currentTrack.Dispose();
            currentTrack = null;
        }

        fadeService?.Dispose();
    }

    public void SkipToNearEnd(double secondsBeforeEnd = 5.0)
    {
        if (currentTrack == null)
            return;

        foreach (var channel in currentTrack.Channels)
        {
            if (channel.AudioSource != null && channel.AudioSource.CanSeek)
            {
                long bytesPerSecond = channel.AudioSource.WaveFormat.BytesPerSecond;
                long newPos = channel.AudioSource.Length - (long)(secondsBeforeEnd * bytesPerSecond);
                
                if (newPos < 0)
                    newPos = 0;

                channel.AudioSource.Position = newPos;
            }
        }
    }
}
