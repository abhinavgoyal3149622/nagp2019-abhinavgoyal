using CodeRedCryptoAPI;
using CodeRedCryptoAPI.Models;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeRedCryptoAPI
{

    public class CodeRedCrypto : ICodeRedCrypto
    {
        private readonly IConfiguration _configuration;
        private const string keySalt = "keySalt";
        private const string passPhrase = "passPhrase";
        int passwordIterations = 2;
        // can be 192 or 128
        private int keySize = 256;
        private ILogger _logger;

        public CodeRedCrypto(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private byte[] CreateSaltOrInitVector()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Hash Password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private byte[] HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 1;
            argon2.Iterations = 3;
            argon2.MemorySize = 8192; // 8192kB

            return argon2.GetBytes(32);
        }

        public bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }

        /// <summary>
        /// Encryt Password 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public PasswordResponseModel Encrypt(PasswordRequestModel passwordData)
        {
            var passwordResponse = new PasswordResponseModel() { InitVectorBytes = passwordData.InitVectorBytes, Salt = passwordData.Salt };
            if (passwordResponse.Salt == null || passwordResponse.InitVectorBytes == null)
            {
                passwordResponse.Salt = CreateSaltOrInitVector();
                passwordResponse.InitVectorBytes = CreateSaltOrInitVector();
            }
            // Convert strings into byte arrays.

            byte[] saltKeyBytes = null;
            saltKeyBytes = Encoding.ASCII.GetBytes(_configuration[keySalt].ToString());

            // Convert our plaintext into a byte array.
            byte[] hashedPasswordBytes = null;

            // Convert our encrypted data from a memory stream into a byte array.
            hashedPasswordBytes = HashPassword(passwordData.PlainText, passwordResponse.Salt);

            // First, we must create a password, from which the key will be derived.
            Rfc2898DeriveBytes key = default(Rfc2898DeriveBytes);
            key = new Rfc2898DeriveBytes(_configuration[passPhrase].ToString(), saltKeyBytes, passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = null;
            keyBytes = key.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = default(RijndaelManaged);
            symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = default(ICryptoTransform);
            encryptor = symmetricKey.CreateEncryptor(keyBytes, passwordResponse.InitVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = default(MemoryStream);
            memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = default(CryptoStream);
            cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(hashedPasswordBytes, 0, hashedPasswordBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            passwordResponse.CipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Return encrypted bytes.
            return passwordResponse;
        }
    }
}
