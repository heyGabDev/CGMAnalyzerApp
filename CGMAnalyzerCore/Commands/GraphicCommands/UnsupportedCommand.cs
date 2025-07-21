using CGMAnalyzerCore.Messages;
using System.Security.Cryptography;
using static CGMAnalyzerCore.Messages.Message;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class UnsupportedCommand  
    {
        public static BaseCgmCommand Unsupported(int ec, int eid, int length, BinaryReader reader)
        {
            if (ec == 0 && eid == 0)
            {
                // 0, 0 is NO-OP
                throw new InvalidOperationException($"[Unsupported] element class - EC={ec}, element ID - EID={eid}");


            }

            if (ec < 10 || ec > 15)
            {
                throw new InvalidOperationException($"[Unsupported] element class - EC={ec}, element ID - EID={eid}");
            }

            new Messages.Message(
                SeverityLevel.Unimplemented,
                ec,
                eid,
                "unsupported",
                commandDescription: null);

            return new CgmCommand(ec, eid, length, reader); // fallback vers commande neutre
        }

        public static Messages.Message Unsupported(int ec, int eid, string msg) { 
            return new Messages.Message(
                SeverityLevel.Unimplemented,
                ec,
                eid,
                "unsupported",
                msg);
        }
    }
}