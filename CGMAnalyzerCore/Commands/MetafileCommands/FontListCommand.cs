using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class FontListCommand : CgmCommand
    {
        public string[] FontNames { get; private set; }

        public FontListCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            // Comptage du nombre de polices
            int count = 0, i = 0;
            while (i < Args.Length)
            {
                count++;
                i += Args[i] + 1;
            }

            FontNames = new string[count];
            count = 0;
            i = 0;

            while (i < Args.Length)
            {
                char[] chars = new char[Args[i]];
                for (int j = 0; j < Args[i]; j++)
                {
                    chars[j] = (char)Args[i + j + 1];
                }
                FontNames[count] = new string(chars);
                count++;
                i += Args[i] + 1;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("FontList ");
            for (int i = 0; i < FontNames.Length - 1; i++)
            {
                sb.Append(FontNames[i]).Append(", ");
            }
            if (FontNames.Length > 0)
            {
                sb.Append(FontNames[FontNames.Length - 1]);
            }
            return sb.ToString();
        }
    }
}
