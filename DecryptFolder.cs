using IFC.Utils;
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

        public override void runOperation()
        {

            Logger.log(logFile, "Running Decrypt Operation");
            Logger.log(logFile, "source: " + SourceDir);
            Logger.log(logFile, "destination: " + DestDir);

            checkDirStructure();
            findAndDecrypt();
        }



        private void findAndDecrypt()
        {
            Logger.log(logFile, "Decrypting files");
            String filePath = null;
            bool fileStatus = true;

            long filesDecriptedInfo = 0;
            long decriptionProcessErrorsInfo = 0;
            long filesSkippedInfo = 0;

            try
            {
                foreach (String f in System.IO.Directory.EnumerateFiles(SourceDir, "*", System.IO.SearchOption.AllDirectories))
                {
                    filePath = f;
                    String hashFromFileName = filePath.Substring(filePath.LastIndexOf(".") + 1);

                    if ( hashFromFileName.Length == HashAlgo.LENGTH )
                    {
                        String fileNameNoHash = filePath.Substring(0, filePath.Length - hashFromFileName.Length - 1);
                        String relativePath = fileNameNoHash.Remove(fileNameNoHash.IndexOf(SourceDir), SourceDir.Length);

                        /* decrypt */
                        Logger.log(logFile, "Decrypting: '" + filePath + "' to '" + DestDir + relativePath + "'");
                        fileStatus = FileEncryptor.decrypt(filePath, DestDir + relativePath);

                        if (fileStatus)
                        { filesDecriptedInfo++;  }
                        else
                        { decriptionProcessErrorsInfo++;  }
                    }
                    else
                    {
                        Logger.log(logFile, "Skipping file due to invalid hash length:" + filePath);
                        filesSkippedInfo++;
                    }
                }
            }
            catch (Exception e)
            {
                if (filePath != null)
                {
                    Logger.logError(logFile, "Cannot access \"" + filePath.Remove(filePath.IndexOf(SourceDir), SourceDir.Length)
                                + "\". File will be skipped. Error details:" + e.Message);
                }
                else
                {
                    Logger.logError(logFile, "File will be skipped. Error details:" + e.Message);
                }
                filesSkippedInfo++;
            }

            Logger.log(logFile, "");
            Logger.log(logFile, "----------------------------");
            Logger.log(logFile, "Total files processed: " + (filesDecriptedInfo + decriptionProcessErrorsInfo + filesSkippedInfo));
            Logger.log(logFile, "Files decrypted: " + filesDecriptedInfo);
            Logger.log(logFile, "Decryption process errors: " + decriptionProcessErrorsInfo);
            Logger.log(logFile, "Files skipped: " + filesSkippedInfo);

        }

        
    }
}
