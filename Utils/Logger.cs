using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    class Logger
    {
        public static void log(String logFile, String logText)
        {
            if (logFile == null) return;
            System.IO.StreamWriter file = new System.IO.StreamWriter(logFile, true);

            file.WriteLine(DateTime.Now.ToString("hh:mm:ss dd-mm-yyyy") + " | INFO  | " + logText);

            file.Close();
        }

        public static void logError(String logFile, String logText)
        {
            if (logFile == null) return;
            System.IO.StreamWriter file = new System.IO.StreamWriter(logFile, true);

            file.WriteLine(DateTime.Now.ToString("hh:mm:ss dd-mm-yyyy") + " | ERROR | " + logText);

            file.Close();
        }

        public static void log(String logFile, String formatting, String logText1, String logText2)
        {
            if (logFile == null) return;
            System.IO.StreamWriter file = new System.IO.StreamWriter(logFile, true);
            file.WriteLine("{0,30} " + formatting, DateTime.Now.ToString("hh:mm:ss dd-mm-yyyy") + " | INFO  | ", logText1, logText2);

            file.Close();
        }

    }
}
