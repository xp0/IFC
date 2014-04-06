using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    class DecryptFolder : FolderOperation
    {

        MD5 hashAlgo = MD5.Create();

        public override void runOperation()
        {
            // check dirs
            //      - find all source subdirs
            //      - verify/create each subdir in destination dir

            // find all files in source folder 
            // run command for file, output to destination dir

            Logger.log(logFile, "Running Decrypt Operation");
            Logger.log(logFile, "source: " + SourceDir);
            Logger.log(logFile, "destination: " + DestDir);

            checkDirStructure();

            findAndDecrypt();

        }

        private void findAndDecrypt()
        {
            /* build file map */
            Logger.log(logFile, "Decrypting files");
            String filePath=null;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = EncExePath;

            try
            {
                foreach (String f in System.IO.Directory.EnumerateFiles(SourceDir, "*", System.IO.SearchOption.AllDirectories))
                {
                    filePath = f;
                    /* extract name and hash */
                    String hashFromFileName = filePath.Substring(filePath.LastIndexOf(".") + 1);
                    if (hashFromFileName.Length == 32)
                    {
                        String fileNameNoHash = filePath.Substring(0, filePath.Length - hashFromFileName.Length - 1);
                        String relativePath = fileNameNoHash.Remove(fileNameNoHash.IndexOf(SourceDir), SourceDir.Length);

                        /* decrypt */
                        startInfo.Arguments = " -d -p " + Password + " -o \"" + DestDir + relativePath + "\" \"" + filePath + "\"";
                        Logger.log(logFile, "Decrypting: '" + filePath + "' to '" + DestDir + relativePath +"'");
                        try
                        {
                            process.StartInfo = startInfo;
                            process.Start();
                        }
                        catch (Exception err)
                        {
                            Logger.logError(logFile,err.Message);
                        }
                    }
                    else
                    {
                        Logger.log(logFile, "Skipping file due to invalid hash length:" + filePath);
                    }
                }
            }
            catch (Exception e)
            {
                if (filePath!=null)
                {
                    Logger.logError(logFile, "Cannot access \"" + filePath.Remove(filePath.IndexOf(SourceDir), SourceDir.Length)
                                + "\". File will be skipped. Error details:" + e.Message);
                }
                else
                {
                    Logger.logError(logFile, e.Message);
                }
            }

        }
    }
}
