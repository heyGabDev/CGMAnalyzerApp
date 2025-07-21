using CGMAnalyzerCore.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Messages
{
    public class Message
    {
        public enum SeverityLevel
        {
            Info,
            Unsupported,
            Unimplemented,
            Fatal
        }

        public SeverityLevel Level { get; }
        public string Description { get; }
        public int ElementClass { get; }
        public int ElementId { get; }
        public string? CommandDescription { get; }


        public Message(SeverityLevel level, int ec, int eid, string description, string commandDescription)
        {
            Level = level;
            ElementClass = ec;
            ElementId = eid;
            Description = description;
            CommandDescription = commandDescription;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(ElementClassMapper.GetElementClassName(ElementClass)).Append(" ");
            sb.Append(ElementClassMapper.GetElementName(ElementClass, ElementId)).Append(" ");
            sb.Append(Level).Append(" ").Append(Description);

            if (!string.IsNullOrEmpty(CommandDescription))
            {
                sb.Append(" {").Append(CommandDescription).Append("}");
            }

            return sb.ToString();
        }

    }
}
