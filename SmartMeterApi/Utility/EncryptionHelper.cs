using System.Security.Cryptography;

namespace SmartMeterApi.Utility
{
    /// <summary>
    /// Helper class to handle encryption and decryption of sensitive data using AES-256.
    /// </summary>
    public static class EncryptionHelper
    {
        internal class AESSecrets
        {
            public byte[] Key { get; set; }
            public byte[] IV { get; set; }
        }

        /*Sicherheitsprinzip:
          - Vermeidung der festen Codierung von Schlüsseln und Anmeldedaten
         */

        /// <summary>
        /// Retrieves AES secrets (key and IV) from environment variables.
        /// </summary>
        /// <returns>AESSecrets object containing the key and IV as byte arrays.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the environment variables are not set.</exception>
        private static AESSecrets GetSecrets()
        {
            // Get Key from environment variable
            string key = Environment.GetEnvironmentVariable("SMG_AES_KEY") ?? throw new InvalidOperationException("SMG_AES_KEY environment variable is not set.");
            // Get IV from environment variable
            string iv = Environment.GetEnvironmentVariable("SMG_AES_IV") ?? throw new InvalidOperationException("SMG_AES_IV environment variable is not set.");

            // Convert hex strings to byte arrays and return as AESSecrets object
            return new AESSecrets
            {
                Key = Convert.FromHexString(key),
                IV = Convert.FromHexString(iv)
            };
        }

        /*Sicherheitsprinzip:
         - Verschlüsselung der Benutzerdaten
         */

        /// <summary>
        /// Encrypts the given plaintext using AES-256 encryption.
        /// </summary>
        /// <param name="plainText">The plaintext to encrypt.</param>
        /// <returns>The encrypted text as a base64 encoded string.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the plaintext is null or empty.</exception>
        public static string Encrypt(string plainText)
        {
            // 1. Validate the input to ensure it is not null or empty
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException(nameof(plainText), "The plaintext cannot be null or empty.");
            }

            // 2. Retrieve the AES secrets (key and IV)
            AESSecrets secrets = GetSecrets();

            // 3. Create an AES algorithm instance
            using (Aes aesAlg = Aes.Create())
            {
                // 4. Set the AES key and AES initialization vector (IV)
                aesAlg.Key = secrets.Key;
                aesAlg.IV = secrets.IV;

                // 5. Create an encryptor to perform the stream transform
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // 6. Create a memory stream to hold the encrypted data
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // 7. Create a crypto stream using the memory stream and the encryptor
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // 8. Create a stream writer to write the plain text to the crypto stream
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write the plaintext to the crypto stream
                            swEncrypt.Write(plainText); 
                        }
                    }

                    // 9. Convert the encrypted data to a base64 encoded string and return it
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypts the given ciphertext using AES-256 decryption.
        /// </summary>
        /// <param name="cipherText">The base64 encoded ciphertext to decrypt.</param>
        /// <returns>The decrypted text.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the ciphertext is null or empty.</exception>
        public static string Decrypt(string cipherText)
        {
            // 1. Validate the input to ensure it is not null or empty
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException(nameof(cipherText), "The ciphertext cannot be null or empty.");
            }

            // 2. Retrieve the AES secrets (key and IV)
            AESSecrets secrets = GetSecrets();

            // 3. Create an AES algorithm instance
            using (Aes aesAlg = Aes.Create())
            {
                // 4. Set the AES key and AES initialization vector (IV)
                aesAlg.Key = secrets.Key;
                aesAlg.IV = secrets.IV;

                // 5. Create a decryptor to perform the stream transform
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // 6. Create a memory stream using the cipher text data
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    // 7. Create a crypto stream using the memory stream and the decryptor
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // 8. Create a stream reader to read the decrypted data from the crypto stream
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // 9. Read the decrypted data and return it as a string
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
