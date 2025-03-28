using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class AudioManager
{
    private IWavePlayer outputDevice;
    private MixingSampleProvider mixer;
    private Random random = new Random();

    // Lista de faixas (VolumeSampleProvider) para controlar o volume de cada uma
    private List<VolumeSampleProvider> tracks = new List<VolumeSampleProvider>();

    // Índice da faixa atualmente em destaque
    private int currentHighlightIndex = -1;

    // Volumes pré-definidos
    private float volumeAlto = 1.0f;
    private float volumeMudo = 0.0f;

    private System.Timers.Timer fadeTimer;
    private const int fadeSteps = 20;
    private const int fadeIntervalMs = 50;
    private int currentStep;
    private VolumeSampleProvider fadingOutTrack;
    private VolumeSampleProvider fadingInTrack;

    public AudioManager()
    {
        mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
        {
            ReadFully = true
        };
        outputDevice = new WaveOutEvent();
    }

    public void LoadRandomMusicFolder(string assetsPath)
    {
        var musicFolders = Directory.GetDirectories(assetsPath);
        if (musicFolders.Length == 0)
        {
            throw new DirectoryNotFoundException("Nenhuma subpasta encontrada em: " + assetsPath);
        }

        string selectedFolder = musicFolders[random.Next(musicFolders.Length)];

        var mp3Files = Directory.GetFiles(selectedFolder, "*.mp3");
        if (mp3Files.Length < 8)
        {
            throw new FileNotFoundException($"Pasta {selectedFolder} não tem pelo menos 8 faixas MP3.");
        }

        // 4. Carregar as 8 faixas
        // (Se quiser garantir ordem, pode ordenar mp3Files; ou se quiser só as primeiras 8, etc.)
        foreach (var filePath in mp3Files.Take(8))
        {
            AddTrack(filePath);
        }

        // 5. Selecionar uma faixa aleatória para ficar com volume alto
        if (tracks.Count == 8)
        {
            currentHighlightIndex = random.Next(8);
            tracks[currentHighlightIndex].Volume = volumeAlto;
        }
    }

    public void InitializePlayback()
    {
        outputDevice.Init(mixer);
        outputDevice.Play();
    }

    /// <summary>
    /// Adiciona uma faixa MP3 ao mixer, com volume inicial mudo (0).
    /// </summary>
    private void AddTrack(string filePath)
    {
        var reader = new Mp3FileReader(filePath);
        var sampleProvider = reader.ToSampleProvider();

        var volumeProvider = new VolumeSampleProvider(sampleProvider)
        {
            Volume = volumeMudo  // começa mudo
        };

        tracks.Add(volumeProvider);
        mixer.AddMixerInput(volumeProvider);
    }

    /// <summary>
    /// Quando detecta mudança de janela, faz o fade entre a faixa atual e outra aleatória.
    /// </summary>
    public void OnWindowFocusChanged()
    {
        if (tracks.Count < 8) return;

        // Escolher uma faixa diferente da atual
        int newIndex;
        do
        {
            newIndex = random.Next(tracks.Count);
        } while (newIndex == currentHighlightIndex);

        fadingOutTrack = tracks[currentHighlightIndex];
        fadingInTrack = tracks[newIndex];

        currentHighlightIndex = newIndex;

        // Iniciar o fade (1 seg)
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

        // Quanto mudar por passo
        float delta = (volumeAlto - volumeMudo) / fadeSteps;

        // Diminuir a faixa que estava em destaque
        if (fadingOutTrack != null)
        {
            float newVolume = fadingOutTrack.Volume - delta;
            fadingOutTrack.Volume = Math.Max(volumeMudo, newVolume);
        }

        // Aumentar a faixa que vai entrar
        if (fadingInTrack != null)
        {
            float newVolume = fadingInTrack.Volume + delta;
            fadingInTrack.Volume = Math.Min(volumeAlto, newVolume);
        }

        // Se já concluímos todos os passos, paramos o timer
        if (currentStep >= fadeSteps)
        {
            fadeTimer.Stop();
            // Garante volumes finais
            if (fadingOutTrack != null) fadingOutTrack.Volume = volumeMudo;
            if (fadingInTrack != null) fadingInTrack.Volume = volumeAlto;
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
