using CGMAnalyzerCore.Messages;
using System.Diagnostics;
using System.Security.Cryptography;
using static CGMAnalyzerCore.Messages.Message;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class UnsupportedCommand
    {
        /// <summary>
        /// Ne lève pas d'exception, retourne une commande neutre
        /// </summary>
        public static BaseCgmCommand CreateUnsupported(int ec, int eid, int length, CgmCommand command)
        {
            Debug.WriteLine($"[UNSUPPORTED] EC={ec}, EID={eid}, Length={length}");

            // Créer une CgmCommand de base qui LIT les Args
            //var cmd = new CgmCommand(ec, eid, length, reader);

            Debug.WriteLine($"[UNSUPPORTED] Args ALREADY read: {command.Args?.Length ?? 0} bytes");

            // Retourner une NullCommand qui UTILISE les Args déjà lus
            return new NullCommand(ec, eid, length, command);
        }
    }

    public class NullCommand : BaseCgmCommand
    {
        /// <summary>
        /// Utilise les Args déjà lus par CgmCommand
        /// </summary>
        public NullCommand(int ec, int eid, int length, CgmCommand command)
            : base(ec, eid, length)
        {
            // Récupérer les Args déjà lus(pas de double lecture)
            Args = command.Args;
            Debug.WriteLine($"[NullCommand] Created with {Args?.Length ?? 0} args");
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Ne rien faire - commande ignorée
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Les arguments ont déjà été lus
            throw new NotImplementedException("Use constructor with CgmCommand instead");
        }

        public override string ToString()
        {
            return $"[Unsupported] Class={ElementClass}, ID={ElementId}, Args={Args?.Length ?? 0}";
        }
    }

}