using CGMAnalyzerCore.Display;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MetafileDefaultsReplacementCommand : BaseCgmCommand
    {
        public BaseCgmCommand EmbeddedCommand { get; private set; }

        public MetafileDefaultsReplacementCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[MetafileDefaultsReplacementCommand] ArgsLength={Args?.Length ?? 0}");
            try
            {
                var argReader = new ExtractedArgumentReader(this);
                // Lire l'en-tête de la commande embarquée (16 bits)
                int k = argReader.MakeUInt(16);

                // Extraire les informations de la commande
                int elementClass = k >> 12;           // Bits 12-15: classe (0-7)
                int elementId = (k >> 5) & 127;       // Bits 5-11: ID élément (0-127)
                int nArgs = k & 31;                   // Bits 0-4: longueur paramètres (0-31)

                // Gérer le format long si nécessaire
                if (nArgs == 31)
                {
                    nArgs = argReader.MakeUInt(16);
                    System.Diagnostics.Debug.Assert((nArgs & (1 << 15)) == 0, "Partitioned data not supported");
                }

                Debug.WriteLine($"[MetafileDefaultsReplacementCommand] " +
                                $"Embedded command: " +
                                $"Class={elementClass}, " +
                                $"ID={elementId}, " +
                                $"nArgs={nArgs}");


                // Copier les arguments de la commande embarquée
                byte[] commandArguments = new byte[nArgs];
                for (int c = 0; c < nArgs; c++)
                {
                    commandArguments[c] = argReader.MakeByte();
                }

                // Créer la commande embarquée à partir des bytes lus
                using (var ms = new MemoryStream(commandArguments))
                using (var reader = new BinaryReader(ms))
                {
                    EmbeddedCommand = CgmCommand.ReadCommand(reader, elementClass, elementId, commandArguments.Length);
                }

                if (EmbeddedCommand == null)
                {
                    Debug.WriteLine("[MetafileDefaultsReplacementCommand WARNING] Failed to create embedded command");
                }
                else
                {
                    Debug.WriteLine($"[MetafileDefaultsReplacementCommand] Successfully created: {EmbeddedCommand.GetType().Name}");
                }

                ValidateArgumentsRead("MetafileDefaultsReplacementCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MetafileDefaultsReplacementCommand ERROR]  {ex.Message}");
                HasReadErrors = true;
            }
        }

        /// <summary>
        /// Applique la commande embarquée comme valeur par défaut au display
        /// </summary>
        public void ApplyToDisplay(CgmDisplay display)
        {
            if (display == null)
            {
                Debug.WriteLine("[MetafileDefaultsReplacementCommand] WARNING: Display is null");
                return;
            }

            if (EmbeddedCommand == null)
            {
                Debug.WriteLine("[MetafileDefaultsReplacementCommand] WARNING: No embedded command to apply");
                return;
            }

            display.ApplyAsDefault(EmbeddedCommand);
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            if (EmbeddedCommand != null)
            {
                return $"METAFILE_DEFAULTS_REPLACEMENT [{EmbeddedCommand}]";
            }
            return "METAFILE_DEFAULTS_REPLACEMENT [No embedded command]";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
