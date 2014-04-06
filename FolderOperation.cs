using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    abstract class FolderOperation
    {
        public String logFile       { get; set; }

        public String EncExePath    { get; set; }   //  to be moved in fileEncryptor 
        public String Password { get; set; }   //  to be moved in fileEncryptor 

        public String SourceDir     { get; set; }
        public String DestDir       { get; set; }

        public FileEncryptor FileEncryptor { get; set; }

        /// <summary>
        /// Main operation method. Deriving classes will override and implement.
        /// </summary>
        public abstract void runOperation();


        /// <summary>
        /// Check/create all source subdirs in destination dir
        /// </summary>
        protected void checkDirStructure()
        {
            Logger.log(logFile, "Checking  directories ...");
            /* check if all subfolders exist in destination dir*/
            String destSubdir;
            foreach (String dirpath in System.IO.Directory.EnumerateDirectories(SourceDir, "*", System.IO.SearchOption.AllDirectories))
            {
                Logger.log(logFile, "Checking [" + dirpath + "]");
                if (dirpath.IndexOf(SourceDir) != -1)
                {
                    destSubdir = dirpath.Remove(dirpath.IndexOf(SourceDir), SourceDir.Length);
                    if (!Directory.Exists(DestDir + destSubdir))
                    {
                        Directory.CreateDirectory(DestDir + destSubdir);
                        Logger.log(logFile, "Creating destination dir [" + DestDir + destSubdir + "]");
                    }
                }
            }
        }
    }
}
