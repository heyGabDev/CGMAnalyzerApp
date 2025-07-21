using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Mapper
{
    public static class ElementClassMapper
    {
        public static string GetElementClassName(int ec)
        {
            return $"Class{ec}";
        }

        public static string GetElementName(int ec, int eid)
        {
            return $"Element{ec}-{eid}";
        }
    }
}
