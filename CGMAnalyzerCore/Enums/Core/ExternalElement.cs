using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Enums.Core
{
    // <summary>
    /// Element class 7: External Elements
    /// </summary>
    public enum ExternalElement
    {
        Unused0 = 0,
        Message = 1,
        ApplicationData = 2
    }
    public static class ExternalElementExtensions
    {
        public static ExternalElement GetElement(int ec)
        {
            return ec switch
            {
                0 => ExternalElement.Unused0,
                1 => ExternalElement.Message,
                2 => ExternalElement.ApplicationData,
                _ => throw new ArgumentOutOfRangeException(nameof(ec), ec, "Invalid external element code")
            };
        }
    }
}
