using System.Windows;
using System.IO;

namespace Sims_4_Work___Study
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private static string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");

        private static void LogError(string message)
        {
            try
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n\n");
            }
            catch { }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Limpa log anterior
            try { File.Delete(LogFilePath); } catch { }
            
            LogError("=== Aplicativo Iniciado ===");
            
            // Gera ícones da barra de tarefas
            try 
            {
                GenerateIcons.CreateTaskbarIcons();
                LogError("Ícones da barra de tarefas gerados com sucesso");
            }
            catch (Exception ex)
            {
                LogError($"Erro ao gerar ícones: {ex.Message}");
            }
            
            // Capturar erros não tratados
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                string errorMsg = $"Erro não tratado:\n{ex?.Message}\n\nStack Trace:\n{ex?.StackTrace}\n\nInner Exception:\n{ex?.InnerException?.Message}";
                LogError(errorMsg);
                
                System.Windows.MessageBox.Show(
                    errorMsg,
                    "Erro Fatal",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (s, args) =>
            {
                string errorMsg = $"Erro no Dispatcher:\n{args.Exception.Message}\n\nStack Trace:\n{args.Exception.StackTrace}\n\nInner Exception:\n{args.Exception.InnerException?.Message}";
                LogError(errorMsg);
                
                System.Windows.MessageBox.Show(
                    errorMsg,
                    "Erro Fatal",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}
