namespace Sims_4_Work___Study
{
    /// <summary>
    /// Centraliza todas as configurações de áudio do aplicativo.
    /// Permite fácil modificação dos parâmetros sem alterar código de lógica.
    /// </summary>
    public class AudioConfiguration
    {
        /// <summary>
        /// Número de channels por música (padrão do The Sims 4: 8).
        /// </summary>
        public int ChannelsPerTrack { get; set; } = 8;

        /// <summary>
        /// Número de canais de áudio (estéreo).
        /// </summary>
        public int AudioChannels { get; set; } = 2;

        /// <summary>
        /// Taxa de amostragem em Hz.
        /// </summary>
        public int SampleRate { get; set; } = 44100;

        /// <summary>
        /// Volume máximo (audível) de um channel ativo.
        /// </summary>
        public float MaxVolume { get; set; } = 1.0f;

        /// <summary>
        /// Volume mínimo (silencioso) de um channel inativo.
        /// </summary>
        public float MinVolume { get; set; } = 0.0f;

        /// <summary>
        /// Número de passos na transição de fade entre channels.
        /// Quanto maior, mais suave a transição (mas mais demorada).
        /// </summary>
        public int FadeSteps { get; set; } = 20;

        /// <summary>
        /// Intervalo em milissegundos entre cada passo do fade.
        /// </summary>
        public int FadeIntervalMs { get; set; } = 50;

        /// <summary>
        /// Duração total do fade em segundos.
        /// Calculada automaticamente com base em FadeSteps e FadeIntervalMs.
        /// </summary>
        public double TotalFadeDurationSeconds => (FadeSteps * FadeIntervalMs) / 1000.0;

        /// <summary>
        /// Extensão padrão dos arquivos de áudio.
        /// </summary>
        public string AudioFileExtension { get; set; } = "*.mp3";

        /// <summary>
        /// Nome da pasta base que contém as músicas.
        /// </summary>
        public string MusicFolderName { get; set; } = "songs";

        /// <summary>
        /// Se true, o mixer preencherá com zeros quando necessário.
        /// </summary>
        public bool MixerFillWithZeros { get; set; } = true;

        /// <summary>
        /// Se true, o mixer dividirá o resultado pelo número de fontes.
        /// </summary>
        public bool MixerDivideResult { get; set; } = false;

        /// <summary>
        /// Retorna uma configuração padrão (valores do The Sims 4).
        /// </summary>
        public static AudioConfiguration Default => new AudioConfiguration();

        /// <summary>
        /// Configuração para testes/debug com transições mais rápidas.
        /// </summary>
        public static AudioConfiguration FastFade => new AudioConfiguration
        {
            FadeSteps = 10,
            FadeIntervalMs = 30
        };

        /// <summary>
        /// Configuração para transições muito suaves e lentas.
        /// </summary>
        public static AudioConfiguration SmoothFade => new AudioConfiguration
        {
            FadeSteps = 40,
            FadeIntervalMs = 75
        };
    }
}
