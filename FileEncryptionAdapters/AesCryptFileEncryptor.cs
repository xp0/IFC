using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    class AesCryptFileEncryptor : FileEncryptor
    {
        System.Diagnostics.Process process;
        System.Diagnostics.ProcessStartInfo startInfo;

        public AesCryptFileEncryptor()
        {
            process = new System.Diagnostics.Process();
            startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\aescrypt.exe";
        }

        public AesCryptFileEncryptor(String _encExePath)
            : this()
        {
            startInfo.FileName = _encExePath;
        }



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
                    Logger.logError(logFile, "Exit code is not 0 for command args: " + startInfo.Arguments);
                    status = false;
                }
                if (!File.Exists(destinationFile))
                {
                    Logger.logError(logFile, "Encrypted file cannot be found: " + destinationFile);
                    status = false;
                }
                else
                {
                    Logger.log(logFile, "File exists " + destinationFile);
                }

            }
            return status;
        }

        /// <summary>
        /// Decrypt files using aescrypt
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
                    Logger.logError(logFile, "Exit code is not 0 for command args: " + startInfo.Arguments);
                    status = false;
                }
                if (!File.Exists(destinationFile))
                {
                    Logger.logError(logFile, "Decrypted file cannot be found: " + destinationFile);
                    status = false;
                }
                else
                {
                    Logger.log(logFile, "File exists " + destinationFile);
                }
            }
            return status;
        }
    }
}
