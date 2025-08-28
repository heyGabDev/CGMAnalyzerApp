using CGMAnalyzerCore.Commands;
using CGMAnalyzerCore.Commands.GraphicCommands;
using CGMAnalyzerCore.Modeles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Parser
{
    public class CgmParser
    {
        private const int INITIAL_NUM_COMMANDS = 500;
        private const int MAX_COMMANDS = 50000; // Limite de sécurité

        public List<BaseCgmCommand> Commands { get; private set; } = new(INITIAL_NUM_COMMANDS);

        public List<string> Messages { get; private set; } = new();
        public CgmMetadata Metadata { get; private set; } = new();

        public static int CurrentLayerId = 0;

        private readonly List<ICommandListener> _commandListeners = new();
        private readonly Dictionary<int, int> _commandStats = new();

        public void AddListener(ICommandListener listener)
        {
            if(listener != null)
                _commandListeners.Add(listener);
        }

        // <summary>
        /// Charge et parse un fichier CGM depuis un stream
        /// </summary>
        public async Task LoadAsync(Stream stream, string filename)
        {
            Stream inputStream = stream;

            // Détection automatique de la compression
            if (IsCompressedFile(filename))
            {
                inputStream = new GZipStream(stream, CompressionMode.Decompress);
                Metadata.IsCompressed = true;
            }

            using var buffered = new BufferedStream(inputStream, 8192); // Buffer plus grand
            using var reader = new BinaryReader(buffered);

            await Task.Run(() => Read(reader));

            // Finaliser les métadonnées
            FinalizeMetadata();
        }

        // <summary>
        /// Version synchrone pour compatibilité
        /// </summary>
        public void Load(Stream stream, string filename)
        {
            LoadAsync(stream, filename).GetAwaiter().GetResult();
        }

        // TODO SGC - Delete
        //public void Load(Stream stream, string filename)
        //{
        //    Stream inputStream = stream;

        //    if (filename.EndsWith(".cgm.gz", StringComparison.OrdinalIgnoreCase) ||
        //        filename.EndsWith(".cgmz", StringComparison.OrdinalIgnoreCase))
        //    {
        //        inputStream = new GZipStream(stream, CompressionMode.Decompress);
        //    }

        //    using var buffered = new BufferedStream(inputStream);
        //    using var reader = new BinaryReader(buffered);

        //    Read(reader);
        //}

        public void Read(BinaryReader reader)
        {
            Reset();
            var stopwatch = Stopwatch.StartNew();
            Commands = new List<BaseCgmCommand>(INITIAL_NUM_COMMANDS);

            try
            {
                int commandCount = 0;
                while (commandCount < MAX_COMMANDS)
                {
                    try
                    {
                        var command = CgmCommand.Read(reader);
                        if (command == null)
                        {
                            Messages.Add("End of file reached or no more commands to read.");
                            break;
                        }

                        // Statistiques des commandes
                        TrackCommandUsage(command);

                        // Notifier les listeners en parallèle pour éviter les blocages
                        NotifyListenersAsync(command);

                        // Nettoyage des arguments pour libérer la mémoire
                        command.CleanUpArguments();

                        // Debug spécifique pour certaines classes d'éléments
                        if (command.ElementClass == 9 && Debugger.IsAttached)
                        {
                            Debug.WriteLine($"[CGM] {command}");
                        }

                        Commands.Add(command);
                        commandCount++;

                        // Optimisation : redimensionner la liste si nécessaire
                        if (Commands.Count == Commands.Capacity && Commands.Capacity < MAX_COMMANDS)
                        {
                            Commands.Capacity = Math.Min(Commands.Capacity * 2, MAX_COMMANDS);
                        }

                        //    // Notifier les listeners (observateurs)
                        //    foreach (var listener in _commandListeners)
                        //{
                        //    listener.CommandProcessed(command);
                        //}


                        //if (command.ElementClass == 9)
                        //{
                        //    System.Diagnostics.Debug.WriteLine(command.ToString());
                        //}

                        Commands.Add(command);
                    }
                    catch (EndOfStreamException ex)
                    {
                        Messages.Add("End of stream reached unexpectedly. " + ex.Message);
                        System.Diagnostics.Debug.WriteLine($"[Stream reached Error] {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Messages.Add("An error occurred while parsing the CGM file: " + ex.Message);
                        System.Diagnostics.Debug.WriteLine($"[Parser Error] {ex.Message}");

                        // Continuer le parsing même en cas d'erreur sur une commande
                        // (optionnel : ajouter un compteur d'erreurs max)
                        continue;
                    }
                }
                if (commandCount >= MAX_COMMANDS)
                {
                    // TODO SGC - translate
                    Messages.Add($"Limite de commandes atteinte ({MAX_COMMANDS}). Parsing interrompu pour des raisons de sécurité.");
                }

            }
            catch (Exception ex)
            {
                Messages.Add($"Erreur critique lors de l'analyse du fichier CGM: {ex.Message}");
                Debug.WriteLine($"[Critical Parser Error] {ex}");
            }
            finally
            {
                stopwatch.Stop();
                Messages.Add($"Parsing terminé en {stopwatch.ElapsedMilliseconds}ms. {Commands.Count} commandes traitées.");
            }
        }

        public void AddCommandListener(ICommandListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            _commandListeners.Add(listener);
        }

        private void Reset()
        {
            Commands.Clear();
            CurrentLayerId = 0;
        }

        private static bool IsCompressedFile(string filename)
        {
            return filename.EndsWith(".cgm.gz", StringComparison.OrdinalIgnoreCase) ||
                   filename.EndsWith(".cgmz", StringComparison.OrdinalIgnoreCase);
        }

        private void FinalizeMetadata()
        {
            Metadata.CommandCount = Commands.Count;
            Metadata.UsedCommands = _commandStats
                .OrderByDescending(cgmMetadata => cgmMetadata.Value)
                .Take(10)
                .Select(cgmMetadata => $"{cgmMetadata.Key >> 0}: Id{cgmMetadata.Key & 0xFF}({cgmMetadata.Value}x))")
                .ToList();

            // Extraire la version si disponible
            var versionCommand = Commands.FirstOrDefault(c => c.ElementClass == 1 && c.ElementId == 1);
            if (versionCommand != null)
            {
                Metadata.Version = versionCommand.ToString();
            }
        }

        private void TrackCommandUsage(BaseCgmCommand command)
        {
            var key = (command.ElementClass << 8) | command.ElementId;
            _commandStats.TryGetValue(key, out var count);
            _commandStats[key] = count + 1;
        }

        private void NotifyListenersAsync(BaseCgmCommand command)
        {
            if (!_commandListeners.IsEmpty)
            {
                // Notification asynchrone pour éviter de bloquer le parsing
                Task.Run(() =>
                {
                    Parallel.ForEach(_commandListeners, listener =>
                    {
                        try
                        {
                            listener.CommandProcessed(command);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[Listener Error] {ex.Message}");
                        }
                    });
                });
            }
        }

    }
}
