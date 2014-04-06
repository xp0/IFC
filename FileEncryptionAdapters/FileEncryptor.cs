using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    
    abstract class FileEncryptor
    {
        public String Password { get; set; }
        public String logFile { get; set; }

        public abstract Boolean encrypt(String sourceFile, String destinationFile);

        public abstract Boolean decrypt(String sourceFile, String destinationFile);
    }
}
