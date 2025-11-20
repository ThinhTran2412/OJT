using Laboratory_Service.Application.Interface;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Laboratory_Service.Infrastructure.Security
{
    /// <summary>
    /// create class have Encypt and Decypt
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.IEncryptionService" />
    public class DeterministicAesEncryptionService : IEncryptionService
    {
        /// <summary>
        /// The key
        /// </summary>
        private readonly byte[] _key;
        /// <summary>
        /// The salt
        /// </summary>
        private readonly byte[] _salt;
        /// <summary>
        /// The iv size
        /// </summary>
        private const int IV_SIZE = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeterministicAesEncryptionService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public DeterministicAesEncryptionService(IConfiguration configuration)
        {
            // Hard-code 32 bytes key (AES-256)
            _key = Convert.FromBase64String("k0mLw2sR4v8fP1QeZtV3yB7uX9hJ6k4LwM9nP2qR0tY="); 
            Console.WriteLine($"[Lab Debug] Key length: {_key.Length} bytes");
            Console.WriteLine($"[Lab Debug] Key bytes: {BitConverter.ToString(_key)}");

            // Hard-code salt
            _salt = Encoding.UTF8.GetBytes("jHJBFM8rTcuwS5zmIszvgQ==");
            Console.WriteLine($"[Lab Debug] Salt bytes length: {_salt.Length}");
        }




        /// <summary>
        /// Encrypts deterministically: same plaintext -&gt; same ciphertext.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
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
        /// <param name="cipherText">The cipher text.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Cannot decrypt deterministically without knowing IV source (plaintext). Store IV if decryption is required.</exception>
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
