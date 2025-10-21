using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public class MarkerTypeCommand : BaseCgmCommand
    {
        /// <summary>
        /// MARKER_TYPE (case 6) - Définit le type de marqueur
        /// </summary>
        public int MarkerType { get; private set; } = 1;
        private const int DEFAULT_MARKER_TYPE = 1;

        public MarkerTypeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[MarkerTypeCommand] ArgLength: {Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                MarkerType = argReader.MakeIndex();
                Debug.WriteLine($"[MarkerTypeCommand]:  MarkerType: {MarkerType} ({GetMarkerTypeName(MarkerType)})");
                ValidateArgumentsRead("MarkerTypeCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MarkerTypeCommand Error] {ex.Message}");
                MarkerType = DEFAULT_MARKER_TYPE;
                HasReadErrors = true;
            }
        }

        private string GetMarkerTypeName(int type)
        {
            return type switch
            {
                1 => "Dot",
                2 => "Plus",
                3 => "Asterisk",
                4 => "Circle",
                5 => "Cross",
                _ => $"Custom ({type})"
            };
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"MARKER_TYPE : {MarkerType} ({GetMarkerTypeName(MarkerType)})";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
