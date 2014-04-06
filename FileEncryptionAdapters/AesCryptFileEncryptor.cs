using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    /// <summary>
    /// File encryption adapter for AES Crypt - Console binary (aescrypt.exe).
    /// URL https://www.aescrypt.com/download/ 
    /// 
    /// </summary>
    class AesCryptFileEncryptor : FileEncryptor
    {
        System.Diagnostics.Process process;
        System.Diagnostics.ProcessStartInfo startInfo;


        /// <summary>
        /// New File encryption adapter for AES Crypt.
        /// </summary>
        public AesCryptFileEncryptor()
        {
            process = new System.Diagnostics.Process();
            startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\aescrypt.exe";
        }

        /// <summary>
        /// New File encryption adapter for AES Crypt.
        /// </summary>
        /// <param name="encExePath">AES Crypt binary location</param>
        public AesCryptFileEncryptor(String encExePath)
            : this()
        {
            startInfo.FileName = encExePath;
        }




        /// <summary>
        /// Encrypt source file and store the output to destination file.
        /// </summary>
        /// <param name="sourceFile">Fully quallified file name of the input file</param>
        /// <param name="destinationFile">Fully qualified file name of the output file</param>
        /// <returns></returns>
        public override bool encrypt(string sourceFile, string destinationFile)
        {
            bool status = true;
            startInfo.Arguments = " -e -p " + Password + " -o \"" + destinationFile + "\" \"" + sourceFile + "\"";
            try
            {
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                status = false;
            }
            finally
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    Logger.logError(logFile, "Command did not finished succesfully, command args: " + startInfo.Arguments);
                    status = false;

                    if (!File.Exists(destinationFile))
                    {
                        Logger.logError(logFile, "Encrypted file cannot be found: " + destinationFile);
                    }
                }
            }
            return status;
        }




        /// <summary>
        /// Decrypt provided source file to destination file
        /// </summary>
        /// <param name="sourceFile">fully qualified file path to encrypt</param>
        /// <param name="destinationFile">fully qualified output (encrypted) file</param>
        /// <returns></returns>
        public override bool decrypt(string sourceFile, string destinationFile)
        {
            bool status = true;
            startInfo.Arguments = " -d -p " + Password + " -o \"" + destinationFile + "\" \"" + sourceFile + "\"";
            try
            {
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception err)
            {
                Logger.logError(logFile, err.Message);
                status = false;
            }
            finally {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    Logger.logError(logFile, "Command did not finished succesfully, command args: " + startInfo.Arguments);
                    status = false;

                    if (!File.Exists(destinationFile))
                    {
                        Logger.logError(logFile, "Decrypted file cannot be found: " + destinationFile);
                    }
                }
            }
            return status;
        }
    }
}
