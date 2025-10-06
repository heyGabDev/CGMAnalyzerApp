using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.ExternalCommands
{
    public class MessageCommand : CgmCommand
    {
        public enum MessageActionFlag
        {
            NoAction = 0,
            Action = 1
        }

        public MessageActionFlag ActionFlag { get; private set; }
        public string Message { get; private set; }

        public MessageCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            int actionFlag = argReader.MakeEnum();
            ActionFlag = actionFlag switch
            {
                0 => MessageActionFlag.NoAction,
                1 => MessageActionFlag.Action,
                _ => MessageActionFlag.NoAction
            };

            Message = argReader.MakeString();

            ValidateArgumentsRead("MessageCommand");
        }

        public int GetActionFlag()
        {
            return (int)ActionFlag;
        }

        public string GetMessage()
        {
            return Message;
        }

        public override string ToString()
        {
            return $"MessageCommand: {(int)ActionFlag} {Message}";
        }
    }

}
