using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.MetafileCommands
{
    public class MetafileElementListCommand : CgmCommand
    {
        public string[] MetaFileElements { get; private set; }

        public MetafileElementListCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int nElements = argReader.MakeInt();
            MetaFileElements = new string[nElements];

            for (int i = 0; i < nElements; i++)
            {
                int code1 = argReader.MakeIndex();
                int code2 = argReader.MakeIndex();

                if (code1 == -1)
                {
                    switch (code2)
                    {
                        case 0:
                            MetaFileElements[i] = "DRAWING SET";
                            break;
                        case 1:
                            MetaFileElements[i] = "DRAWING PLUS CONTROL SET";
                            break;
                        case 2:
                            MetaFileElements[i] = "VERSION 2 SET";
                            break;
                        case 3:
                            MetaFileElements[i] = "EXTENDED PRIMITIVES SET";
                            break;
                        case 4:
                            MetaFileElements[i] = "VERSION 2 GKSM SET";
                            break;
                        case 5:
                            MetaFileElements[i] = "VERSION 3 SET";
                            break;
                        case 6:
                            MetaFileElements[i] = "VERSION 4 SET";
                            break;
                        default:
                            MetaFileElements[i] = $"UNSUPPORTED SET {code2}";
                            break;
                    }
                }
                else
                {
                    MetaFileElements[i] = $" ({code1},{code2})";
                }
            }

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in MetafileElementList");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("MetafileElementList ");
            foreach (string element in MetaFileElements)
            {
                sb.Append(element).Append(" ");
            }
            return sb.ToString();
        }
    }
}
