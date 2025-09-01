using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class AppendTextCommand : BaseCgmCommand
    {
        public string Text { get; private set; } = string.Empty;

        public AppendTextCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            Text = argReader.MakeString();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Cette commande ajoute du texte à la position courante
            // Implémentation basique - peut être améliorée selon le contexte
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => $"APPEND_TEXT: \"{Text}\"";
    }
}
