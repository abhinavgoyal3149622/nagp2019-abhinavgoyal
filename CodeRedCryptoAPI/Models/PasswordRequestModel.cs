using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeRedCryptoAPI
{
    public class PasswordRequestModel
    {
        public string PlainText { get; set; }

        public byte[] Salt { get; set; }   
            
        public byte[] InitVectorBytes { get; set; }

    }
}
