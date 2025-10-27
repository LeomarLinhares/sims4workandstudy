using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sims_4_Work___Study
{
    /// <summary>
    /// Gerencia a biblioteca de músicas do aplicativo, incluindo:
    /// - Lista de todas as pastas de música disponíveis
    /// - Controle de músicas já reproduzidas vs. não reproduzidas
    /// - Seleção aleatória de músicas sem repetição
    /// - Navegação entre músicas (anterior/próxima)
    /// </summary>
    public class MusicLibraryManager
    {
        private List<string> allFolders;
        private List<string> unplayedFolders;
        private List<string> playedFolders;
        private string? currentFolderPath;
        private Random random;

        /// <summary>
        /// Evento disparado quando a música atual muda.
        /// Parâmetros: (oldFolderPath, newFolderPath)
        /// </summary>
        public event Action<string?, string>? MusicChanged;

        /// <summary>
        /// Caminho da pasta de música atualmente sendo reproduzida.
        /// </summary>
        public string? CurrentFolderPath => currentFolderPath;

        /// <summary>
        /// Número total de pastas de música disponíveis.
        /// </summary>
        public int TotalMusicCount => allFolders?.Count ?? 0;

        /// <summary>
        /// Número de músicas ainda não reproduzidas.
        /// </summary>
        public int UnplayedMusicCount => unplayedFolders?.Count ?? 0;

        /// <summary>
        /// Número de músicas já reproduzidas.
        /// </summary>
        public int PlayedMusicCount => playedFolders?.Count ?? 0;

        /// <summary>
        /// Indica se há músicas anteriores disponíveis.
        /// </summary>
        public bool HasPreviousMusic => playedFolders != null && playedFolders.Count > 0;

        /// <summary>
        /// Indica se há músicas seguintes disponíveis.
        /// Como o ciclo é infinito, retorna true se houver ao menos uma pasta carregada.
        /// </summary>
        public bool HasNextMusic => allFolders != null && allFolders.Count > 0;

        public MusicLibraryManager()
        {
            random = new Random();
            allFolders = new List<string>();
            unplayedFolders = new List<string>();
            playedFolders = new List<string>();
        }

        /// <summary>
        /// Inicializa a biblioteca de músicas a partir de um diretório base.
        /// </summary>
        /// <param name="basePath">Caminho do diretório que contém as pastas de música</param>
        public void Initialize(string basePath)
        {
            if (!Directory.Exists(basePath))
            {
                throw new DirectoryNotFoundException($"Diretório não encontrado: {basePath}");
            }

            allFolders = Directory.GetDirectories(basePath).ToList();
            
            if (allFolders.Count == 0)
            {
                throw new InvalidOperationException($"Nenhuma pasta de música encontrada em: {basePath}");
            }

            ResetUnplayedFolders();
        }

        /// <summary>
        /// Força a reinicialização do ciclo (reembaralha).
        /// </summary>
        public void ForceResetCycle()
        {
            ResetUnplayedFolders();
        }

        /// <summary>
        /// Reseta a lista de músicas não reproduzidas, embaralhando todas as músicas.
        /// Remove a música atual da lista para evitar repetição imediata.
        /// </summary>
        private void ResetUnplayedFolders()
        {
            var shuffledFolders = new List<string>(allFolders);

            // Evita que a música atual seja a primeira escolhida no novo ciclo
            if (!string.IsNullOrEmpty(currentFolderPath))
            {
                shuffledFolders.RemoveAll(p => string.Equals(p, currentFolderPath, StringComparison.OrdinalIgnoreCase));
            }

            Shuffle(shuffledFolders);

            // Se só houver uma pasta no total e foi removida acima, re-adiciona-a (caso limite)
            if (shuffledFolders.Count == 0 && !string.IsNullOrEmpty(currentFolderPath) && allFolders.Contains(currentFolderPath))
            {
                shuffledFolders.Add(currentFolderPath);
            }

            unplayedFolders = shuffledFolders;
            playedFolders.Clear();
        }

        /// <summary>
        /// Embaralha uma lista usando o algoritmo Fisher-Yates.
        /// </summary>
        private void Shuffle(List<string> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// Seleciona a próxima música aleatória não reproduzida.
        /// Se todas foram reproduzidas, reseta a lista e começa novamente (ciclo infinito).
        /// Dispara o evento MusicChanged(old, new).
        /// </summary>
        /// <returns>Caminho da pasta da música selecionada</returns>
        public string GetNextRandomMusic()
        {
            if (unplayedFolders == null || unplayedFolders.Count == 0)
            {
                ResetUnplayedFolders();
            }

            // Garantir que unplayedFolders não é nulo após ResetUnplayedFolders
            if (unplayedFolders == null || unplayedFolders.Count == 0)
            {
                throw new InvalidOperationException("Nenhuma música disponível após reinicialização.");
            }

            string selectedFolder = unplayedFolders[0];
            unplayedFolders.RemoveAt(0);

            // Adiciona a música atual à lista de reproduzidas antes de mudar (evita duplicatas)
            string? oldFolder = currentFolderPath;
            if (!string.IsNullOrEmpty(oldFolder))
            {
                if (!playedFolders.Contains(oldFolder))
                    playedFolders.Add(oldFolder);
            }

            currentFolderPath = selectedFolder;

            // Notifica quem assina o evento para que possa preservar volume/fades entre old->current
            MusicChanged?.Invoke(oldFolder, currentFolderPath);

            return selectedFolder;
        }

        /// <summary>
        /// Retorna à música anterior na lista de reprodução.
        /// </summary>
        /// <returns>Caminho da pasta da música anterior</returns>
        public string GetPreviousMusic()
        {
            if (!HasPreviousMusic)
            {
                throw new InvalidOperationException("Nenhuma música anterior disponível.");
            }

            // Move a música atual de volta para não reproduzidas (evita duplicatas)
            if (!string.IsNullOrEmpty(currentFolderPath))
            {
                unplayedFolders.RemoveAll(p => string.Equals(p, currentFolderPath, StringComparison.OrdinalIgnoreCase));
                unplayedFolders.Insert(0, currentFolderPath);
            }

            // Pega a última música reproduzida
            string previousFolder = playedFolders[playedFolders.Count - 1];
            playedFolders.RemoveAt(playedFolders.Count - 1);

            string? oldFolder = currentFolderPath;
            currentFolderPath = previousFolder;

            MusicChanged?.Invoke(oldFolder, currentFolderPath);

            return previousFolder;
        }

        /// <summary>
        /// Avança para a próxima música (mesmo que GetNextRandomMusic, mas mais explícito).
        /// </summary>
        /// <returns>Caminho da pasta da próxima música</returns>
        public string GoToNextMusic()
        {
            return GetNextRandomMusic();
        }

        /// <summary>
        /// Retorna à música anterior.
        /// </summary>
        /// <returns>Caminho da pasta da música anterior</returns>
        public string GoToPreviousMusic()
        {
            return GetPreviousMusic();
        }

        /// <summary>
        /// Valida se uma pasta contém o número mínimo de arquivos de áudio.
        /// </summary>
        /// <param name="folderPath">Caminho da pasta a validar</param>
        /// <param name="minimumFiles">Número mínimo de arquivos necessários</param>
        /// <param name="fileExtension">Extensão dos arquivos (padrão: *.mp3)</param>
        /// <returns>True se a pasta é válida</returns>
        public bool ValidateMusicFolder(string folderPath, int minimumFiles = 8, string fileExtension = "*.mp3")
        {
            if (!Directory.Exists(folderPath))
            {
                return false;
            }

            var files = Directory.GetFiles(folderPath, fileExtension);
            return files.Length >= minimumFiles;
        }

        /// <summary>
        /// Obtém os arquivos de áudio de uma pasta específica.
        /// </summary>
        /// <param name="folderPath">Caminho da pasta</param>
        /// <param name="fileExtension">Extensão dos arquivos (padrão: *.mp3)</param>
        /// <param name="maxFiles">Número máximo de arquivos a retornar (padrão: 8)</param>
        /// <returns>Array com os caminhos dos arquivos</returns>
        public string[] GetMusicFiles(string folderPath, string fileExtension = "*.mp3", int maxFiles = 8)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Pasta não encontrada: {folderPath}");
            }

            var files = Directory.GetFiles(folderPath, fileExtension);
            
            if (files.Length < maxFiles)
            {
                throw new InvalidOperationException(
                    $"A pasta {Path.GetFileName(folderPath)} não contém o mínimo de {maxFiles} arquivos {fileExtension}. Encontrados: {files.Length}");
            }

            return files.Take(maxFiles).ToArray();
        }
    }
}
