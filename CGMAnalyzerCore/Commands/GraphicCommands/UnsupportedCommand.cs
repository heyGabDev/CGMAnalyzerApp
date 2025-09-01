using CGMAnalyzerCore.Messages;
using System.Security.Cryptography;
using static CGMAnalyzerCore.Messages.Message;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class UnsupportedCommand
    {
        //TO DO : CONTROLE TEST
        public static BaseCgmCommand CreateUnsupported(int ec, int eid, int length, BinaryReader reader)
        {
            // Au lieu de lever une exception, créer une commande "neutre"
            return new NullCommand(ec, eid, length, reader);
        }

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

    //TO DO : CONTROLE TEST
    public class NullCommand : BaseCgmCommand
    {
        public NullCommand(int ec, int eid, int length, BinaryReader reader)
            : base(ec, eid, length)
        {
            // Lire et ignorer les arguments sans traitement de facon securisée
            for (int i = 0; i < length; i++)
            {
                try
                {
                    reader.ReadByte();
                }
                catch (EndOfStreamException)
                {
                    break; // Arrêter si on atteint la fin
                }
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Ne rien faire - commande ignorée
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Déjà lu dans le constructeur
        }

        public override string ToString()
        {
            return $"[Unsupported] Class={ElementClass}, ID={ElementId}";
        }
    }

}