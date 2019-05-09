using System;
using System.Security.Cryptography;
using System.Text;

namespace MarcusW.SharpUtils.Core.Cryptography
{
    public static class Hashing
    {
        public static byte[] Sha256(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            using (var sha256 = new SHA256Managed())
                return sha256.ComputeHash(bytes);
        }

        public static byte[] Sha256(string token) => Sha256(Encoding.UTF8.GetBytes(token));

        public static byte[] Sha384(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            using (var sha384 = new SHA384Managed())
                return sha384.ComputeHash(bytes);
        }

        public static byte[] Sha384(string token) => Sha384(Encoding.UTF8.GetBytes(token));

        public static byte[] Sha512(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            using (var sha512 = new SHA512Managed())
                return sha512.ComputeHash(bytes);
        }

        public static byte[] Sha512(string token) => Sha512(Encoding.UTF8.GetBytes(token));
    }
}
