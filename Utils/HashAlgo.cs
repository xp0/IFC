using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IFC.Utils
{

    class HashAlgo
    {
        public static int LENGTH = 32;

        MD5 hashAlgo = MD5.Create();

        public String hashFile(String filePath)
        {
            String returnVal;
            using (var stream = File.OpenRead(filePath))
            {
                returnVal = BitConverter.ToString(hashAlgo.ComputeHash(stream)).Replace("-", string.Empty);
            }
            return returnVal;
        }

    }
}
