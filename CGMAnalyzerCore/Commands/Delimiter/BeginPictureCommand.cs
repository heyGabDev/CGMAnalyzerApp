using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.Delimiter
{
    public class BeginPictureCommand : BaseCgmCommand
    {
        public string Name { get; private set; }

        private readonly CgmArgumentReader _argReader;

        public BeginPictureCommand(int ec, int eid, int l, BinaryReader reader, CgmArgumentReader argReader)
            : base(ec, eid, l)
        {
            _argReader = argReader ?? throw new ArgumentNullException(nameof(reader));
            Name = _argReader.MakeString(reader);
        }

        public override string ToString()
        {
            return $"BeginPicture {Name}";
        }

        public override void Draw(Graphics g, Pen pen)
        {
            throw new NotImplementedException();
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
