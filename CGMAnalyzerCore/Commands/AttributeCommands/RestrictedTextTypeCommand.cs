using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.AttributeCommands
{
    public partial class RestrictedTextTypeCommand : CgmCommand
    {

        public RestrictedTextTypeEnum TextType { get; private set; }

        public RestrictedTextTypeCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int type = argReader.MakeIndex();
            TextType = type switch
            {
                1 => RestrictedTextTypeEnum.Basic,
                2 => RestrictedTextTypeEnum.Boxed,
                3 => RestrictedTextTypeEnum.Isotropic,
                4 => RestrictedTextTypeEnum.Justified,
                _ => RestrictedTextTypeEnum.Basic
            };

            ValidateArgumentsRead("RestrictedTextType");

            //System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
            //    "Not all arguments were read in RestrictedTextType");
        }

        public override string ToString()
        {
            return $"RestrictedTextType {TextType}";
        }
    }
}
