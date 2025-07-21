using CGMAnalyzerCore.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Parser
{
    public interface ICommandListener
    {
        void CommandProcessed(BaseCgmCommand command);
    }
}
