using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MarcusW.SharpUtils.Core.Extensions;

namespace MarcusW.SharpUtils.Core.Cryptography
{
    public class AES : IDisposable
    {
        /// <summary>
        /// The key that is used by this <see cref="AES"/> instance.
        /// </summary>
        public byte[] Key { get; }

        /// <summary>
        /// The key that is used by this <see cref="AES"/> instance. Encoded as hex string.
        /// </summary>
        public string HexKey => Key.ToHexString();

        private readonly Aes _aesAlgorithm;

        private bool _disposed;

        internal AES(Aes aesAlgorithm)
        {
            _aesAlgorithm = aesAlgorithm ?? throw new ArgumentNullException(nameof(aesAlgorithm));
            Key = _aesAlgorithm.Key;
        }

        /// <summary>
        /// Encrypts an array of bytes
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <returns>Encryption result</returns>
        public byte[] Encrypt(byte[] data)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AES));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Lock the algorithm implementation because we change the IV
            lock (_aesAlgorithm)
            {
                // Create a new IV for every encryption
                _aesAlgorithm.GenerateIV();

                using (ICryptoTransform cryptoTransform = _aesAlgorithm.CreateEncryptor())
                using (var outputStream = new MemoryStream())
                {
                    // IV doesn't need to be kept secret, but must be known at decryption time, so write it to the result
                    Debug.Assert(_aesAlgorithm.IV.Length == 16);
                    outputStream.Write(_aesAlgorithm.IV, 0, 16);

                    // Encrypt data and write to result
                    using (var cryptoStream = new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write))
                        cryptoStream.Write(data, 0, data.Length);

                    // return encrypted data
                    return outputStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Decrypts encrypted data
        /// </summary>
        /// <param name="encrypted">Encrypted data</param>
        /// <returns>Decryption result</returns>
        public byte[] Decrypt(byte[] encrypted)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AES));
            if (encrypted == null)
                throw new ArgumentNullException(nameof(encrypted));

            // Lock the algorithm implementation because we change the IV
            lock (_aesAlgorithm)
            {
                using (var inputStream = new MemoryStream(encrypted))
                {
                    // Read IV from data
                    var iv = new byte[16];
                    inputStream.Read(iv, 0, 16);
                    _aesAlgorithm.IV = iv;

                    using (ICryptoTransform cryptoTransform = _aesAlgorithm.CreateDecryptor())
                    using (var cryptoStream = new CryptoStream(inputStream, cryptoTransform, CryptoStreamMode.Read))
                    using (var outputStream = new MemoryStream())
                    {
                        // Decrypt data
                        cryptoStream.CopyTo(outputStream);
                        cryptoStream.Flush();

                        // Return decryption result
                        return outputStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="data">String to encrypt</param>
        /// <returns>Encrypted data</returns>
        public byte[] EncryptString(string data)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AES));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return Encrypt(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Decrypts encrypted data as string
        /// </summary>
        /// <param name="encrypted">Encrypted data</param>
        /// <returns>Decrypted string</returns>
        public string DecryptString(byte[] encrypted)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AES));
            if (encrypted == null)
                throw new ArgumentNullException(nameof(encrypted));

            return Encoding.UTF8.GetString(Decrypt(encrypted));
        }

        public void Dispose()
        {
            _aesAlgorithm?.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Returns an <see cref="AES"/> instance from a newly generated key
        /// </summary>
        public static AES GenerateKey()
        {
            // Key is generated on initialization
            var aesAlgorithm = Aes.Create();
            return new AES(aesAlgorithm);
        }

        /// <summary>
        /// Returns an <see cref="AES"/> instance for the given key
        /// </summary>
        public static AES FromKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var aesAlgorithm = Aes.Create();
            if (aesAlgorithm == null)
                throw new PlatformNotSupportedException("Could not initialize AES implementation");
            aesAlgorithm.Key = key;

            return new AES(aesAlgorithm);
        }

        /// <summary>
        /// Returns an <see cref="AES"/> instance for the given key
        /// </summary>
        public static AES FromBase64Key(string base64Key)
        {
            if (base64Key == null)
                throw new ArgumentNullException(nameof(base64Key));

            return FromKey(base64Key.ParseAsBase64());
        }

        /// <summary>
        /// Returns an <see cref="AES"/> instance for the given key
        /// </summary>
        public static AES FromHexKey(string hexKey)
        {
            if (hexKey == null)
                throw new ArgumentNullException(nameof(hexKey));

            return FromKey(hexKey.ParseAsHex());
        }
    }
}
