using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using IFC.Utils;


namespace IFC
{
    class EncryptFolder : FolderOperation
    {

        /*
         * File tables for source and destination fodlers.
         *  key: filename
         *  val: file hash
        */
        private Hashtable sourceFileTable = new Hashtable();
        private Hashtable destinationFileTable = new Hashtable();

        HashAlgo hashAlgo = new HashAlgo();


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
            printTable(sourceFileTable);

            Logger.log(logFile, "");
            Logger.log(logFile, "Destination file map");
            Logger.log(logFile, "----------------------------");
            printTable(destinationFileTable);

            Logger.log(logFile, "");
            Logger.log(logFile, "Copy and Encrypt");
            Logger.log(logFile, "----------------------------");
            findAndEncrypt();

            Logger.log(logFile, "");
            Logger.log(logFile, "----------------------------");
            Logger.log(logFile, "Encrypt Folder complete.");
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
                    fileHash = hashAlgo.hashFile(filePath);
                    sourceFileTable.Add(filePath.Remove(filePath.IndexOf(SourceDir), SourceDir.Length), fileHash);
                }
                catch (Exception e)
                {
                    Logger.logError(logFile, "Cannot add " + filePath.Remove(filePath.IndexOf(SourceDir), SourceDir.Length)  
                                + " to table. File will  be skipped. Error details:" + e.Message);
                }
            }
        }




        /// <summary>
        /// Find files from destination folder and extract the hash from the filename
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
                    destinationFileTable.Add(noHashFilePath, fileHash);
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
            /* true: create encrypted version of the source file */
            bool addCommandReq;

            /* true: a file was updated,the old version of the file should be removed from destination dir */
            bool delCommandReq;

            bool encryptStatus = true;

            /* file counters for logging */
            long newFileCountInfo=0;
            long changedFileCountInfo=0;

            /* diff the tables */
            foreach (String ks in sourceFileTable.Keys)
            {
                addCommandReq = false;
                delCommandReq = false;

                if (destinationFileTable.ContainsKey(ks))
                {
                    if (!sourceFileTable[ks].Equals(destinationFileTable[ks].ToString()))
                    {
                        /* existing file with changes (hashes are different) */
                        addCommandReq = true;
                        delCommandReq = true;
                        changedFileCountInfo++;
                    }
                    else
                    {
                        /* source file not changed */
                        Logger.log(logFile, "No change:" + ks);
                    }
                }
                else
                {
                    /* it's a new file */
                    addCommandReq = true;
                    newFileCountInfo++;
                }

                if (addCommandReq)
                {
                    if (delCommandReq)
                    { Logger.log(logFile, "Change detected:" + ks); }
                    else
                    { Logger.log(logFile, "New File detected:" + ks); }

                    encryptStatus = FileEncryptor.encrypt(SourceDir + ks, DestDir + ks + "." + sourceFileTable[ks]);

                    if(encryptStatus) {
                        if (delCommandReq)
                        {
                                String oldFileName = DestDir + ks + "." + destinationFileTable[ks];
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
                    }
                    else
                    {
                        Logger.logError(logFile, "Encryption status is not succesful for source file:'" + SourceDir + ks +"'");
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



    }
}
