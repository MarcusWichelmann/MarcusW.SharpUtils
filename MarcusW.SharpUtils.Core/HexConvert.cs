using System;
using System.Text;

namespace MarcusW.SharpUtils.Core
{
    public static class HexConvert
    {
        /// <summary>
        /// Encode bytes as hexadecimal string
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Hex string</returns>
        public static string ToHexString(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var resultBuilder = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                string hexComponent = Convert.ToString(b, 16);
                if (hexComponent.Length == 1)
                    resultBuilder.Append('0');
                resultBuilder.Append(hexComponent);
            }


            return resultBuilder.ToString();
        }

// Improved version, as soon as this library can be released with targeting .Net Standard 2.1
#if NETSTANDARD2_1
        /// <summary>
        /// Decode hex string to bytes
        /// </summary>
        /// <param name="hex">Hex string</param>
        /// <returns>Bytes</returns>
        public static byte[] FromHexString(ReadOnlySpan<char> hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Odd hex string length.", nameof(hex));

            var result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = Convert.ToByte(hex.Slice(i * 2, 2).ToString(), 16);

            return result;
        }
#endif

        /// <summary>
        /// Decode hex string to bytes
        /// </summary>
        /// <param name="hex">Hex string</param>
        /// <returns>Bytes</returns>
        public static byte[] FromHexString(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex));
#if NETSTANDARD2_1
            return FromHexString(hex.AsSpan());
#else
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Odd hex string length.", nameof(hex));

            var result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return result;
#endif
        }
    }
}
