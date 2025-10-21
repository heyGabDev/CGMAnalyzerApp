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
    /// <summary>
    /// RESTRICTED_TEXT_TYPE (case 42) - Définit le type de texte restreint
    /// </summary>

    public enum RestrictedTextTypeEnum
    {
        Basic = 1,
        Boxed = 2,
        Isotropic = 3,
        Justified = 4
    }

    public class RestrictedTextTypeCommand : BaseCgmCommand
    {

        public RestrictedTextTypeEnum RestrictedTextType { get; private set; } = RestrictedTextTypeEnum.Basic;
        private const RestrictedTextTypeEnum DEFAULT_RESTRICTED_TEXT_TYPE = RestrictedTextTypeEnum.Basic;

        public RestrictedTextTypeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[RestrictedTextTypeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int restrictedTextTypeValue = argReader.MakeIndex();
                RestrictedTextType = restrictedTextTypeValue switch
                {
                    1 => RestrictedTextTypeEnum.Basic,
                    2 => RestrictedTextTypeEnum.Boxed,
                    3 => RestrictedTextTypeEnum.Isotropic,
                    4 => RestrictedTextTypeEnum.Justified,
                    _ => RestrictedTextTypeEnum.Basic
                };
                Debug.WriteLine($"[RestrictedTextTypeCommand] RestrictedTextType={restrictedTextTypeValue} : {RestrictedTextType}");

                ValidateArgumentsRead("RestrictedTextTypeCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RestrictedTextTypeCommand Error] {ex.Message}");
                RestrictedTextType = DEFAULT_RESTRICTED_TEXT_TYPE;   
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"RESTRICTED_TEXT_TYPE : {RestrictedTextType}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
