using System;
using System.Collections.Generic;
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

    private List<VolumeSource> tracks = new List<VolumeSource>();

    private int currentHighlightIndex = -1;

    private const float volumeAlto = 1.0f;
    private const float volumeMudo = 0.0f;

    private System.Timers.Timer fadeTimer;
    private const int fadeSteps = 20;
    private const int fadeIntervalMs = 50;
    private int currentStep;
    private VolumeSource fadingOutTrack;
    private VolumeSource fadingInTrack;

    public CSAudioManager()
    {
        mixer = new SimpleMixer(2, 44100)
        {
            FillWithZeros = true,
            DivideResult = false
        };

        outputDevice = new WasapiOut();
    }

    public void LoadRandomMusicFolder(string assetsPath)
    {
        var musicFolders = Directory.GetDirectories(assetsPath);
        if (musicFolders.Length == 0)
        {
            throw new DirectoryNotFoundException("Nenhuma subpasta encontrada em: " + assetsPath);
        }

        string selectedFolder = musicFolders[random.Next(musicFolders.Length)];

        var flacFiles = Directory.GetFiles(selectedFolder, "*.flac");
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
        outputDevice.Initialize(mixer.ToWaveSource(32));
        outputDevice.Play();
    }

    /// <summary>
    /// Adiciona uma faixa FLAC ao mixer, iniciando com volume mudo (0).
    /// </summary>
    private void AddTrack(string filePath)
    {
        var source = CodecFactory.Instance.GetCodec(filePath)
            .ToSampleSource()
            .ToStereo();

        var volumeSource = new VolumeSource(source)
        {
            Volume = volumeMudo
        };

        tracks.Add(volumeSource);

        mixer.AddSource(volumeSource);
    }

    /// <summary>
    /// Quando ocorre mudança de foco na janela, faz o fade entre a faixa atual e outra aleatória.
    /// </summary>
    public void OnWindowFocusChanged()
    {
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
            outputDevice.Dispose();
            outputDevice = null;
        }
    }
}
