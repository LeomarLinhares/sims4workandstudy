using System;
using System.Timers;

namespace Sims_4_Work___Study
{
    /// <summary>
    /// Serviço responsável por gerenciar transições suaves (fade) entre channels de áudio.
    /// Quando o usuário muda de janela, este serviço cria uma transição gradual
    /// diminuindo o volume de um channel enquanto aumenta o de outro.
    /// </summary>
    public class ChannelFadeService
    {
        private System.Timers.Timer? fadeTimer;
        private int currentStep;
        private int fadeSteps;
        private int fadeIntervalMs;
        
        private AudioChannel? fadingOutChannel;
        private AudioChannel? fadingInChannel;
        
        private float sourceVolume;
        private float targetVolume;

        /// <summary>
        /// Evento disparado quando o fade é concluído.
        /// </summary>
        public event EventHandler? FadeCompleted;

        /// <summary>
        /// Indica se há um fade em andamento.
        /// </summary>
        public bool IsFading { get; private set; }

        public ChannelFadeService(int fadeSteps = 20, int fadeIntervalMs = 50)
        {
            this.fadeSteps = fadeSteps;
            this.fadeIntervalMs = fadeIntervalMs;
            IsFading = false;
        }

        /// <summary>
        /// Inicia um fade entre dois channels.
        /// </summary>
        /// <param name="fromChannel">Channel que terá o volume diminuído</param>
        /// <param name="toChannel">Channel que terá o volume aumentado</param>
        /// <param name="fromVolume">Volume inicial do channel de origem (padrão: 1.0)</param>
        /// <param name="toVolume">Volume final do channel de destino (padrão: 1.0)</param>
        public void StartFade(AudioChannel fromChannel, AudioChannel toChannel, 
            float fromVolume = 1.0f, float toVolume = 1.0f)
        {
            if (fromChannel == null || toChannel == null)
                throw new ArgumentNullException("Channels não podem ser nulos");

            if (IsFading)
            {
                StopFade();
            }

            fadingOutChannel = fromChannel;
            fadingInChannel = toChannel;
            sourceVolume = fromVolume;
            targetVolume = toVolume;
            
            currentStep = 0;
            IsFading = true;

            if (fadeTimer == null)
            {
                fadeTimer = new System.Timers.Timer(fadeIntervalMs);
                fadeTimer.Elapsed += FadeTimer_Elapsed;
            }
            
            fadeTimer.Start();
        }

        private void FadeTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            currentStep++;

            float delta = (sourceVolume - 0f) / fadeSteps;

            // Diminui o volume do channel que está saindo
            if (fadingOutChannel != null)
            {
                float newVolume = fadingOutChannel.CurrentVolume - delta;
                fadingOutChannel.CurrentVolume = Math.Max(0f, newVolume);
            }

            // Aumenta o volume do channel que está entrando
            if (fadingInChannel != null)
            {
                float newVolume = fadingInChannel.CurrentVolume + (targetVolume / fadeSteps);
                fadingInChannel.CurrentVolume = Math.Min(targetVolume, newVolume);
            }

            // Verifica se o fade foi concluído
            if (currentStep >= fadeSteps)
            {
                CompleteFade();
            }
        }

        private void CompleteFade()
        {
            fadeTimer?.Stop();
            
            // Garante que os volumes finais estejam corretos
            if (fadingOutChannel != null)
            {
                fadingOutChannel.CurrentVolume = 0f;
                fadingOutChannel.IsActive = false;
            }
            
            if (fadingInChannel != null)
            {
                fadingInChannel.CurrentVolume = targetVolume;
                fadingInChannel.IsActive = true;
            }

            IsFading = false;
            
            // Limpa as referências
            fadingOutChannel = null;
            fadingInChannel = null;

            // Dispara o evento de conclusão
            FadeCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Para o fade atual imediatamente.
        /// </summary>
        public void StopFade()
        {
            if (fadeTimer != null && fadeTimer.Enabled)
            {
                fadeTimer.Stop();
            }
            
            IsFading = false;
            fadingOutChannel = null;
            fadingInChannel = null;
        }

        /// <summary>
        /// Libera os recursos do serviço.
        /// </summary>
        public void Dispose()
        {
            StopFade();
            
            if (fadeTimer != null)
            {
                fadeTimer.Dispose();
                fadeTimer = null;
            }
        }
    }
}
