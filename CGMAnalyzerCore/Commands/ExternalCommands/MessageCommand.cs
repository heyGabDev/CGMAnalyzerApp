using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.ExternalCommands
{
    /// <summary>
    /// MESSAGE (case 7, 1) - Commande de message externe
    /// </summary>

    public enum MessageActionFlag
    {
        NoAction = 0,
        Action = 1
    }
    
    public class MessageCommand : BaseCgmCommand
    {

        public MessageActionFlag ActionFlag { get; private set; } = MessageActionFlag.NoAction;
        private const MessageActionFlag DEFAULT_ACTION_FLAG = MessageActionFlag.NoAction;
        public string Message { get; private set; } = "";

        public MessageCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[MessageCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int msgActionFlagValue = argReader.MakeEnum();
                ActionFlag = msgActionFlagValue switch
                {
                    0 => MessageActionFlag.NoAction,
                    1 => MessageActionFlag.Action,
                    _ => MessageActionFlag.NoAction
                };

                Message = argReader.MakeString();
                Debug.WriteLine($"[MessageCommand] ActionFlag : {msgActionFlagValue} ({(int)ActionFlag})");

                ValidateArgumentsRead("MessageCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MessageCommand Error] {ex.Message}");
                ActionFlag = DEFAULT_ACTION_FLAG;
                Message = string.Empty;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"MESSAGE_COMMAND: ActionFlag={ActionFlag}, Message='{Message}'";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }

}
