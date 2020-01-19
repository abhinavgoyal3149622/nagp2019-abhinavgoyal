using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeRedCryptoAPI.Models
{
    public class PasswordResponseModel
    {
        public byte[] CipherTextBytes { get; set; }

        public byte[] Salt { get; set; }

        public byte[] InitVectorBytes { get; set; }

    }
}
