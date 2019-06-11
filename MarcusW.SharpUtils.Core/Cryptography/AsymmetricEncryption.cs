using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using MarcusW.SharpUtils.Core.Extensions;

namespace MarcusW.SharpUtils.Core.Cryptography
{
    /// <summary>
    /// Provides helper methods for encrypting large amounts of data asymmetrically.
    /// </summary>
    public static class AsymmetricEncryption
    {
        public static (byte[] encryptedKey, byte[] encryptedData) Encrypt(X509Certificate2 publicCertificate, byte[] data, int aesKeySize = 128)
        {
            RSA publicKey = RSAKeyLoader.LoadFromCertificate(publicCertificate);
            if (!publicKey.GetHasPublicKey())
                throw new ArgumentException("Public key information not found in certificate.");

            return Encrypt(publicKey, data, aesKeySize);
        }

        public static (byte[] encryptedKey, byte[] encryptedData) Encrypt(RSA publicKey, byte[] data, int aesKeySize = 128)
        {
            // Symmetrically encrypt data
            using (AES aes = AES.GenerateKey(aesKeySize))
            {
                byte[] encryptedData = aes.Encrypt(data);

                // Asymmetricaly encrypt key
                byte[] encryptedKey = publicKey.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA256);

                return (encryptedKey, encryptedData);
            }
        }

        public static byte[] Decrypt(X509Certificate2 keyCertificate, byte[] encryptedKey, byte[] encryptedData)
        {
            RSA privateKey = RSAKeyLoader.LoadFromCertificate(keyCertificate);
            if (!privateKey.GetHasPrivateKey())
                throw new ArgumentException("Private key information not found in certificate.");

            return Decrypt(privateKey, encryptedKey, encryptedData);
        }

        public static byte[] Decrypt(RSA privateKey, byte[] encryptedKey, byte[] encryptedData)
        {
            // Decrypt key
            byte[] aesKey = privateKey.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);

            // Decrypt data
            using (AES aes = AES.FromKey(aesKey))
                return aes.Decrypt(encryptedData);
        }
    }
}
