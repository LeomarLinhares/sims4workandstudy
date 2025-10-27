using CSCore;
using CSCore.Streams;

namespace Sims_4_Work___Study
{
    /// <summary>
    /// Representa um único channel de áudio dentro de uma música.
    /// Cada música é composta por 8 channels que tocam simultaneamente.
    /// </summary>
    public class AudioChannel
    {
        /// <summary>
        /// Caminho do arquivo de áudio deste channel.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Source de áudio original (IWaveSource).
        /// </summary>
        public IWaveSource AudioSource { get; private set; }

        /// <summary>
        /// VolumeSource que controla o volume deste channel.
        /// </summary>
        public VolumeSource VolumeSource { get; private set; }

        /// <summary>
        /// Volume atual do channel (0.0 = mudo, 1.0 = volume máximo).
        /// </summary>
        public float CurrentVolume
        {
            get => VolumeSource?.Volume ?? 0f;
            set
            {
                if (VolumeSource != null)
                {
                    VolumeSource.Volume = value;
                }
            }
        }

        /// <summary>
        /// Indica se este channel é o channel ativo (audível) no momento.
        /// </summary>
        public bool IsActive { get; set; }

        public AudioChannel(string filePath, IWaveSource audioSource, VolumeSource volumeSource)
        {
            FilePath = filePath;
            AudioSource = audioSource;
            VolumeSource = volumeSource;
            IsActive = false;
        }

        /// <summary>
        /// Define este channel como ativo com volume máximo.
        /// </summary>
        public void SetAsActive(float maxVolume = 1.0f)
        {
            IsActive = true;
            CurrentVolume = maxVolume;
        }

        /// <summary>
        /// Muta este channel (volume = 0).
        /// </summary>
        public void Mute()
        {
            IsActive = false;
            CurrentVolume = 0f;
        }

        /// <summary>
        /// Libera os recursos deste channel.
        /// </summary>
        public void Dispose()
        {
            AudioSource?.Dispose();
        }
    }
}
