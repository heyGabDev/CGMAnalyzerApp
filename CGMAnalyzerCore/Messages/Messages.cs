using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Messages
{
    public class Messages : List<Message>
    {
        private static readonly Messages _instance = new Messages();

        // Singleton public access
        public static Messages Instance => _instance;

        // Private constructor
        private Messages()
        {
            // Prevent instantiation outside
        }

        public void Reset()
        {
            this.Clear();
        }
    }
}

   

