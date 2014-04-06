using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace IFC
{
    class EncryptFolder : FolderOperation
    {
        private Hashtable sourceFileListHT = new Hashtable();
        private Hashtable destinationFileListHT = new Hashtable();

        private ArrayList commands = new ArrayList();

        MD5 hashAlgo = MD5.Create();

        public EncryptFolder()
        {
        }

        /// <summary>
        /// Find and hash all files from source folder
        /// </summary>
        private void buildSourceFileTable()
        {
            /* build file map */
            Logger.log(logFile, "Build Source file table");
            String fileHash;
            foreach (String filePath in System.IO.Directory.EnumerateFiles(SourceDir, "*", System.IO.SearchOption.AllDirectories))
            {
                try
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        fileHash = BitConverter.ToString(hashAlgo.ComputeHash(stream)).Replace("-", string.Empty);
                    }

                    sourceFileListHT.Add(filePath.Remove(filePath.IndexOf(SourceDir), SourceDir.Length), fileHash);
                }
                catch (Exception e)
                {
                    Logger.logError(logFile, "Cannot add " + filePath.Remove(filePath.IndexOf(SourceDir), SourceDir.Length)  
                                + " to table. File will  be skipped. Error details:" + e.Message);
                }
            }
        }

        /// <summary>
        /// Find and extract the has from the filename hash all files from destination folder
        /// </summary>
        private void buildDestinationFileTable()
        {
            /* build file map */
            Logger.log(logFile, "Build Destination file table");
            String noHashFilePath;
            String fileHash;
            foreach (String filePath in System.IO.Directory.EnumerateFiles(DestDir, "*", System.IO.SearchOption.AllDirectories))
            {
                /* extract the hash from filename (everything after the last "." ) */
                fileHash = filePath.Substring(filePath.LastIndexOf(".") + 1);
                noHashFilePath = filePath.Remove(filePath.LastIndexOf("."));
                noHashFilePath = noHashFilePath.Remove(noHashFilePath.IndexOf(DestDir), DestDir.Length);

                try
                {
                    destinationFileListHT.Add(noHashFilePath, fileHash);
                }
                catch (Exception ex)
                {
                    Logger.logError(logFile, "Destination table already contains this file. Error details :" + ex.Message);
                }
            }

        }

        /// <summary>
        /// Compare the source and destination file table and find and encrypt all new or changed files (different hash)
        /// </summary>
        private void findAndEncrypt()
        {
            /* if true create encrypted version of the source file */
            bool addCommandReq;

            /* if true a file waas updated. 
             * indicates that the old version of the encrypted file should be removed from destination dir */
            bool delCommandReq; 

            /* file counters for logging */
            long newFileCountInfo=0;
            long changedFileCountInfo=0;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = EncExePath;

            /* diff the tables */
            foreach (String ks in sourceFileListHT.Keys)
            {
                addCommandReq = false;
                delCommandReq = false;
                if (destinationFileListHT.ContainsKey(ks))
                {
                    if (!sourceFileListHT[ks].Equals(destinationFileListHT[ks].ToString()))
                    {
                        /* hashes are different */
                        addCommandReq = true;
                        delCommandReq = true;
                        changedFileCountInfo++;
                    }
                    else
                    {
                        Logger.log(logFile, "No change:" + ks);
                    }
                }
                else
                {
                    /* file is not there */
                    addCommandReq = true;
                    newFileCountInfo++;
                }

                if (addCommandReq)
                {
                    if (delCommandReq)
                    { Logger.log(logFile, "Change detected:" + ks); }
                    else
                    { Logger.log(logFile, "New File detected:" + ks); }

                    startInfo.Arguments = " -e -p " + Password + " -o \"" + DestDir + ks + "." + sourceFileListHT[ks] + "\" \"" + SourceDir + ks + "\"";
                  
                    try
                    {
                        process.StartInfo = startInfo;
                        process.Start();
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }
                    finally
                    {
                        process.WaitForExit();
                        if (delCommandReq)
                        {
                            /* remove previous version of the file when encryption for the new version will exit succesfully */
                            if (process.ExitCode == 0)
                            {
                                String oldFileName = DestDir + ks + "." + destinationFileListHT[ks];
                                Logger.log(logFile, "Remove old file: " + oldFileName);
                                try
                                {
                                    File.Delete(oldFileName);
                                }
                                catch (Exception e)
                                {
                                    Logger.logError(logFile, "ERROR cannot remove old file: " + oldFileName + " err details:" + e.Message);
                                }
                            }
                            else
                            {
                                Logger.logError(logFile, "Exit code is not 0 for command args: " + startInfo.Arguments);
                            }
                        }
                    }
                }

            }

            Logger.log(logFile, "");
            Logger.log(logFile, "----------------------------");
            Logger.log(logFile, "Total files encrypted: " + (newFileCountInfo+changedFileCountInfo) );
            Logger.log(logFile, "New Files: " + newFileCountInfo);
            Logger.log(logFile, "Changed Files: " + changedFileCountInfo);
        }


        private void printTable(Hashtable table)
        {
            foreach (String k in table.Keys)
            {
                Logger.log(logFile, "{1,32} {2,-300}",table[k].ToString(), k);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void runOperation()
        {
            Logger.log(logFile, "Running Encrypt Operation");
            Logger.log(logFile, "source: " + SourceDir);
            Logger.log(logFile, "destination: " + DestDir);

            checkDirStructure();

            buildSourceFileTable();
            buildDestinationFileTable();

            Logger.log(logFile, "");
            Logger.log(logFile, "Source file map");
            Logger.log(logFile, "----------------------------");
            printTable(sourceFileListHT);

            Logger.log(logFile, "");
            Logger.log(logFile, "Destination file map");
            Logger.log(logFile, "----------------------------");
            printTable(destinationFileListHT);

            Logger.log(logFile, "");
            Logger.log(logFile, "Copy and Encrypt");
            Logger.log(logFile, "----------------------------");
            findAndEncrypt();

            Logger.log(logFile, "");
            Logger.log(logFile, "----------------------------");
            Logger.log(logFile, "Encrypt Folder complete.");
        }
    }
}
