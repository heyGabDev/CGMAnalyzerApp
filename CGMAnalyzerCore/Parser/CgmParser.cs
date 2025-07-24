using CGMAnalyzerCore.Commands;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Parser
{
    public class CgmParser
    {
        private const int INITIAL_NUM_COMMANDS = 500;

        public List<BaseCgmCommand> Commands { get; private set; } = new(INITIAL_NUM_COMMANDS);

        public List<string> Messages { get; private set; } = new();
        
        public static int CurrentLayerId = 0;

        private readonly List<ICommandListener> _commandListeners = new();

        public void AddListener(ICommandListener listener)
        {
            _commandListeners.Add(listener);
        }


        public void Load(Stream stream, string filename)
        {
            Stream inputStream = stream;

            if (filename.EndsWith(".cgm.gz", StringComparison.OrdinalIgnoreCase) ||
                filename.EndsWith(".cgmz", StringComparison.OrdinalIgnoreCase))
            {
                inputStream = new GZipStream(stream, CompressionMode.Decompress);
            }

            using var buffered = new BufferedStream(inputStream);
            using var reader = new BinaryReader(buffered);

            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            Reset();
            Commands = new List<BaseCgmCommand>(INITIAL_NUM_COMMANDS);

            try
            {
                while (true)
                {
                    var command = CgmCommand.Read(reader);
                    if (command == null)
                    {
                        Messages.Add("End of file reached or no more commands to read.");
                        break;
                    }
                        

                    // Notifier les listeners (observateurs)
                    foreach (var listener in _commandListeners)
                    {
                        listener.CommandProcessed(command);
                    }

                    command.CleanUpArguments();

                    if (command.ElementClass == 9)
                    {
                        Console.WriteLine(command.ToString());
                    }

                    Commands.Add(command); ;
                }
            }
            catch (EndOfStreamException ex)
            {
                Messages.Add("End of stream reached unexpectedly. " + ex.Message);
            }
            catch (Exception ex)
            {
                Messages.Add("An error occurred while parsing the CGM file: " + ex.Message);
                Console.WriteLine($"[Parser Error] {ex.Message}");
            }
        }

        private void Reset()
        {
            Commands.Clear();
            CurrentLayerId = 0;
        }

        public void AddCommandListener(ICommandListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            _commandListeners.Add(listener);
        }
    }
}
