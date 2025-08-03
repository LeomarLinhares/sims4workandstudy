using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.FLAC;
using CSCore.SoundOut;
using CSCore.Streams;
using Sims_4_Work___Study;


public class CSAudioManager
{
    private ISoundOut outputDevice;
    private SimpleMixer mixer;
    private Random random = new Random();

    private List<string> allFolders;
    private List<string> unplayedFolders;
    private List<string> playedFolders;
    private string playingFolder;


    private List<VolumeSource> tracks = new List<VolumeSource>();
    public event EventHandler PlaybackFinished;
    public int chanceTarget;

    private int currentHighlightIndex = -1;

    private const float volumeAlto = 1.0f;
    private const float volumeMudo = 0.0f;

    private System.Timers.Timer fadeTimer;
    private const int fadeSteps = 20;
    private const int fadeIntervalMs = 50;
    private int currentStep;
    private VolumeSource fadingOutTrack;
    private VolumeSource fadingInTrack;

    private List<IWaveSource> readers = new List<IWaveSource>();

    public CSAudioManager()
    {
        mixer = new SimpleMixer(2, 44100)
        {
            FillWithZeros = true,
            DivideResult = false
        };

        mixer.PlaybackFinished += (s, e) =>
        {
            PlaybackFinished?.Invoke(this, EventArgs.Empty);
        };

        outputDevice = new WasapiOut();

    }

    public void LoadRandomMusicFolder()
    {
        if (unplayedFolders.Count == 0)
        {
            ResetUnplayedFolders();
        }

        string selectedFolder = unplayedFolders[0];

        if (playingFolder == null)
        {
            playingFolder = selectedFolder;
        }

        unplayedFolders.RemoveAt(0);

        Debug.WriteLine("Carregando pasta: " + selectedFolder);

        var flacFiles = Directory.GetFiles(selectedFolder, "*.mp3");
        if (flacFiles.Length < 8)
        {
            throw new FileNotFoundException($"Pasta {selectedFolder} não tem pelo menos 8 faixas FLAC.");
        }

        foreach (var filePath in flacFiles.Take(8))
        {
            AddTrack(filePath);
        }

        if (tracks.Count == 8)
        {
            currentHighlightIndex = random.Next(8);
            tracks[currentHighlightIndex].Volume = volumeAlto;
        }
    }

    public void InitializePlayback()
    {
        if (mixer == null)
        {
            mixer = new SimpleMixer(2, 44100)
            {
                FillWithZeros = true,
                DivideResult = false
            };
            mixer.PlaybackFinished += (s, e) =>
            {
                PlaybackFinished?.Invoke(this, EventArgs.Empty);
            };
        }

        if (outputDevice == null)
            outputDevice = new WasapiOut();

        outputDevice.Initialize(mixer.ToWaveSource(32));
        outputDevice.Play();
    }

    /// <summary>
    /// Adiciona uma faixa FLAC ao mixer, iniciando com volume mudo (0).
    /// </summary>
    private void AddTrack(string filePath)
    {
        IWaveSource reader = CodecFactory.Instance.GetCodec(filePath);
        readers.Add(reader);

        var source = reader
            .ToSampleSource()
            .ToStereo();

        var volumeSource = new VolumeSource(source)
        {
            Volume = volumeMudo
        };

        tracks.Add(volumeSource);
        mixer.AddSource(volumeSource);
    }

    public void CreateMixerIfNeeded()
    {
        if (mixer == null)
        {
            mixer = new SimpleMixer(2, 44100)
            {
                FillWithZeros = true,
                DivideResult = false
            };
            mixer.PlaybackFinished += (s, e) =>
            {
                PlaybackFinished?.Invoke(this, EventArgs.Empty);
            };
        }
    }

    public void StartPlayback()
    {
        if (outputDevice == null)
            outputDevice = new WasapiOut();

        if (mixer == null)
            CreateMixerIfNeeded();

        outputDevice.Initialize(mixer.ToWaveSource(32));
        outputDevice.Play();
    }

    public void ClearMixer()
    {
        if (mixer != null)
        {
            mixer.Dispose();
            mixer = null;
        }
    }

    public void ClearAll()
    {
        if (outputDevice != null)
        {
            outputDevice.Stop();
            outputDevice.Dispose();
            outputDevice = null;
        }
        ClearMixer();
        tracks.Clear();
        readers.Clear();
    }

    public void SetBasePath(string assetsPath)
    {
        allFolders = Directory.GetDirectories(assetsPath).ToList();
        ResetUnplayedFolders();
    }

    private void ResetUnplayedFolders()
    {
        allFolders = new List<string>(allFolders);
        Shuffle(allFolders);
        unplayedFolders = new List<string>(allFolders);
        playedFolders = new List<string>();
    }
    private void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    /// <summary>
    /// Quando ocorre mudança de foco na janela, faz o fade entre a faixa atual e outra aleatória.
    /// </summary>
    public void OnWindowFocusChanged()
    {
        int chanceResult = random.Next(100);
        if (chanceResult >= chanceTarget) return;

        if (tracks.Count < 8) return;

        int newIndex;
        do
        {
            newIndex = random.Next(tracks.Count);
        } while (newIndex == currentHighlightIndex);

        fadingOutTrack = tracks[currentHighlightIndex];
        fadingInTrack = tracks[newIndex];
        currentHighlightIndex = newIndex;

        currentStep = 0;
        if (fadeTimer == null)
        {
            fadeTimer = new System.Timers.Timer(fadeIntervalMs);
            fadeTimer.Elapsed += FadeTimer_Elapsed;
        }
        fadeTimer.Start();
    }

    private void FadeTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        currentStep++;

        float delta = (volumeAlto - volumeMudo) / fadeSteps;

        if (fadingOutTrack != null)
        {
            float newVolume = fadingOutTrack.Volume - delta;
            fadingOutTrack.Volume = Math.Max(volumeMudo, newVolume);
        }

        if (fadingInTrack != null)
        {
            float newVolume = fadingInTrack.Volume + delta;
            fadingInTrack.Volume = Math.Min(volumeAlto, newVolume);
        }

        if (currentStep >= fadeSteps)
        {
            fadeTimer.Stop();
            if (fadingOutTrack != null)
                fadingOutTrack.Volume = volumeMudo;
            if (fadingInTrack != null)
                fadingInTrack.Volume = volumeAlto;
        }
    }

    public void StopAll()
    {
        if (outputDevice != null)
        {
            outputDevice.Stop();
        }
    }

    public void SetMainTrackVolume(float volume)
    {
        if (currentHighlightIndex >= 0 && currentHighlightIndex < tracks.Count)
        {
            tracks[currentHighlightIndex].Volume = volume;
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

    public void PreviousSong()
    {
        if (playedFolders.Count == 0)
        {
            Debug.WriteLine("Nenhuma música anterior para reproduzir.");
            return;
        }
        unplayedFolders.Insert(0, playingFolder);
        playingFolder = playedFolders.Last();
        playedFolders.RemoveAt(playedFolders.Count - 1);
        Debug.WriteLine("Reproduzindo pasta anterior: " + playingFolder);
        ClearMixer();
        CreateMixerIfNeeded();
        tracks.Clear();
        var flacFiles = Directory.GetFiles(playingFolder, "*.mp3");
        if (flacFiles.Length < 8)
        {
            throw new FileNotFoundException($"Pasta {playingFolder} não tem pelo menos 8 faixas FLAC.");
        }
        foreach (var filePath in flacFiles.Take(8))
        {
            AddTrack(filePath);
        }
        if (tracks.Count == 8)
        {
            currentHighlightIndex = random.Next(8);
            tracks[currentHighlightIndex].Volume = volumeAlto;
            StopAll();
            InitializePlayback();
        }
    }

    public void NextSong()
    {
        if (unplayedFolders.Count == 0)
        {
            ResetUnplayedFolders();
        }
        playedFolders.Add(playingFolder);
        string selectedFolder = unplayedFolders[0];
        unplayedFolders.RemoveAt(0);
        playingFolder = selectedFolder;
        Debug.WriteLine("Carregando pasta: " + selectedFolder);
        ClearMixer();
        CreateMixerIfNeeded();
        tracks.Clear();

        var flacFiles = Directory.GetFiles(selectedFolder, "*.mp3");
        if (flacFiles.Length < 8)
        {
            throw new FileNotFoundException($"Pasta {selectedFolder} não tem pelo menos 8 faixas FLAC.");
        }
        foreach (var filePath in flacFiles.Take(8))
        {
            AddTrack(filePath);
        }
        if (tracks.Count == 8)
        {
            currentHighlightIndex = random.Next(8);
            tracks[currentHighlightIndex].Volume = volumeAlto;
            StopAll();
            InitializePlayback();
        }
    }

    //-----------------------------------------------------------
    // Métodos para DEBUG
    //-----------------------------------------------------------

    public void SkipToNearEnd(double secondsBeforeEnd = 5.0)
    {
        foreach (var reader in readers)
        {
            if (reader.CanSeek)
            {
                long bytesPerSecond = reader.WaveFormat.BytesPerSecond;
                long newPos = reader.Length - (long)(secondsBeforeEnd * bytesPerSecond);
                if (newPos < 0)
                    newPos = 0;
                reader.Position = newPos;
            }
        }
    }
}
