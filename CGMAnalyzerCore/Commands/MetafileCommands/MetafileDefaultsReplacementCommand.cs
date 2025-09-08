using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MetafileDefaultsReplacementCommand : CgmCommand
    {
        public BaseCgmCommand EmbeddedCommand { get; private set; }

        public MetafileDefaultsReplacementCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int k = argReader.MakeUInt(16);

            // Extract element class and ID
            int elementClass = k >> 12;
            int elementId = (k >> 5) & 127;

            int nArgs = k & 31;
            if (nArgs == 31)
            {
                // Long form command
                nArgs = argReader.MakeUInt(16);
                // Note: we don't support partitioned data here
                System.Diagnostics.Debug.Assert((nArgs & (1 << 15)) == 0, "Partitioned data not supported");
            }

            // Copy remaining arguments
            byte[] commandArguments = new byte[nArgs];
            for (int c = 0; c < nArgs; c++)
            {
                commandArguments[c] = argReader.MakeByte();
            }

            // Create embedded command
            using (var ms = new MemoryStream(commandArguments))
            using (var reader = new BinaryReader(ms))
            {
                EmbeddedCommand = ReadCommand(reader, elementClass, elementId, commandArguments.Length);
            }

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in MetafileDefaultsReplacement");
        }

        public override string ToString()
        {
            return $"MetafileDefaultsReplacement {EmbeddedCommand}";
        }

        // Méthode paint équivalente du Java
        public void ApplyToDisplay(object display)
        {
            // Dans le Java original : this.embeddedCommand.paint(d);
            // À implémenter selon votre architecture de rendu
        }
    }

}
