using System;
using System.Collections.Generic;
using System.Linq;

namespace Sims_4_Work___Study
{
    /// <summary>
    /// Representa uma música completa, que é composta por múltiplos channels de áudio
    /// tocando simultaneamente. No The Sims 4, cada música no modo Construir/Comprar
    /// possui 8 channels diferentes.
    /// </summary>
    public class MusicTrack
    {
        /// <summary>
        /// Nome ou identificador da música (geralmente o caminho da pasta).
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Caminho completo da pasta que contém os arquivos de áudio desta música.
        /// </summary>
        public string FolderPath { get; private set; }

        /// <summary>
        /// Lista de todos os channels que compõem esta música.
        /// </summary>
        public List<AudioChannel> Channels { get; private set; }

        /// <summary>
        /// Índice do channel atualmente ativo (audível).
        /// </summary>
        public int ActiveChannelIndex { get; private set; }

        /// <summary>
        /// Retorna o channel atualmente ativo.
        /// </summary>
        public AudioChannel? ActiveChannel
        {
            get
            {
                if (ActiveChannelIndex >= 0 && ActiveChannelIndex < Channels.Count)
                {
                    return Channels[ActiveChannelIndex];
                }
                return null;
            }
        }

        /// <summary>
        /// Número total de channels nesta música.
        /// </summary>
        public int ChannelCount => Channels.Count;

        public MusicTrack(string folderPath)
        {
            FolderPath = folderPath;
            Name = System.IO.Path.GetFileName(folderPath);
            Channels = new List<AudioChannel>();
            ActiveChannelIndex = -1;
        }

        /// <summary>
        /// Adiciona um novo channel a esta música.
        /// </summary>
        public void AddChannel(AudioChannel channel)
        {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            Channels.Add(channel);
        }

        /// <summary>
        /// Define qual channel será o ativo (audível), mutando todos os outros.
        /// </summary>
        public void SetActiveChannel(int channelIndex, float activeVolume = 1.0f)
        {
            if (channelIndex < 0 || channelIndex >= Channels.Count)
                throw new ArgumentOutOfRangeException(nameof(channelIndex));

            // Muta todos os channels
            foreach (var channel in Channels)
            {
                channel.Mute();
            }

            // Ativa o channel selecionado
            ActiveChannelIndex = channelIndex;
            Channels[channelIndex].SetAsActive(activeVolume);
        }

        /// <summary>
        /// Seleciona um channel aleatório para ser o ativo, excluindo o channel atual.
        /// </summary>
        public int GetRandomChannelIndexExcludingCurrent(Random random)
        {
            if (Channels.Count <= 1)
                return ActiveChannelIndex;

            int newIndex;
            do
            {
                newIndex = random.Next(Channels.Count);
            } while (newIndex == ActiveChannelIndex);

            return newIndex;
        }

        /// <summary>
        /// Define o volume do channel ativo.
        /// </summary>
        public void SetActiveChannelVolume(float volume)
        {
            if (ActiveChannel != null)
            {
                ActiveChannel.CurrentVolume = volume;
            }
        }

        /// <summary>
        /// Libera todos os recursos dos channels desta música.
        /// </summary>
        public void Dispose()
        {
            foreach (var channel in Channels)
            {
                channel.Dispose();
            }
            Channels.Clear();
        }
    }
}
