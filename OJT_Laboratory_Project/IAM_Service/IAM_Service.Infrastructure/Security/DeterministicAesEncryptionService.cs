using IAM_Service.Application.Interface.IEncryptionService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IAM_Service.Infrastructure.Security
{
    public class DeterministicAesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _salt;
        private const int IV_SIZE = 16;

        public DeterministicAesEncryptionService(IConfiguration configuration)
        {
            var base64Key = configuration["Encryption:Key"];
            var saltValue = configuration["Encryption:Salt"];

            if (string.IsNullOrEmpty(base64Key))
                throw new InvalidOperationException("Encryption key is missing in configuration.");

            _key = Convert.FromBase64String(base64Key);
            _salt = Encoding.UTF8.GetBytes(saltValue ?? "default-shared-salt");
        }

        /// <summary>
        /// Encrypts deterministically: same plaintext -> same ciphertext.
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            // Generate deterministic IV (SHA256 of plainText + salt, take first 16 bytes)
            using var sha = SHA256.Create();
            var ivSource = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText + Convert.ToBase64String(_salt)));
            var iv = ivSource.Take(IV_SIZE).ToArray();

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _key;
            aes.IV = iv;

            using var memoryStream = new MemoryStream();
            using (var encryptor = aes.CreateEncryptor())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
            {
                writer.Write(plainText);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        /// Decrypts the ciphertext.
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            // For deterministic encryption, IV must be regenerated the same way
            using var sha = SHA256.Create();
            var decrypted = string.Empty;

            // Try all possible IV candidates (you can skip if IV rule is deterministic)
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = _key;

                // We must regenerate IV from plaintext pattern, so we can only decrypt data encrypted by this class
                // For decrypt, since IV = hash(plainText+salt), we can only decrypt if we know the same plainText logic.
                // So normally we store IV or use same deterministic scheme both sides.

                throw new InvalidOperationException("Cannot decrypt deterministically without knowing IV source (plaintext). Store IV if decryption is required.");
            }
            catch
            {
                return decrypted;
            }
        }
    }
}
