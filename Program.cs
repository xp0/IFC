using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    class Program
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationMode"> '-e' to encrypt, '-d' to decrypt </param>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="-p">password indicator</param>
        /// <param name="password">password value</param>
        static void Main(string[] args)
        {


            System.Console.WriteLine("Arguments count :" + args.Length);

            foreach (String arg in args)
            {
                System.Console.WriteLine("arg: " + arg);
            }


            String sourceDir;
            String destDir;

            if (args[1].IndexOf("\\\\") != -1)
            {
                sourceDir = args[1].Replace("\\", "\\\\");
                destDir = args[2].Replace("\\", "\\\\");
            }
            else
            {
                sourceDir = args[1];
                destDir = args[2];
            }

            FolderOperation folderOp;
            String logFile = "";

            if (args[0].Equals("-e"))
            {
                logFile = sourceDir + "\\IFC_" + DateTime.Now.ToString("yyyymmdd_hhmmss") + ".log";
                folderOp = new EncryptFolder();
            }
            else if (args[0].Equals("-d"))
            {
                logFile = destDir + "\\IFC_" + DateTime.Now.ToString("yyyymmdd_hhmmss") + ".log";
                folderOp = new DecryptFolder();
            }
            else
            {
                System.Console.WriteLine("Unsuported folder operation: " + args[0]);
                return;
            }
            folderOp.SourceDir = sourceDir;
            folderOp.DestDir = destDir;

            folderOp.logFile = logFile;

            System.Console.WriteLine("Log File:" + logFile);

            FileEncryptor fEnc = new AesCryptFileEncryptor();
            fEnc.logFile = logFile;
            fEnc.Password       = args[4];
            folderOp.Password   = args[4];
            folderOp.FileEncryptor = fEnc;

            folderOp.EncExePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\aescrypt.exe";
            if (!File.Exists(folderOp.EncExePath))
            {
                System.Console.WriteLine("Can't find " + folderOp.EncExePath);
            }
            else
            {
                System.Console.WriteLine("Exe is " + folderOp.EncExePath);
                folderOp.runOperation();
            }

            System.Console.WriteLine("Finished.");
            System.Console.ReadKey();
        }
    }
}
