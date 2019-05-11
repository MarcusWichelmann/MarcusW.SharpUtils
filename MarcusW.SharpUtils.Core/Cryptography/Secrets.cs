using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MarcusW.SharpUtils.Core.Cryptography
{
    public static class Secrets
    {
        /// <summary>
        /// Generates a random token with a specified length
        /// </summary>
        /// <param name="length">Length of the token that will be generated</param>
        /// <returns>Bytes of the token</returns>
        public static byte[] GenerateToken(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            // Generate random bytes
            var bytes = new byte[length];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
                randomNumberGenerator.GetBytes(bytes);

            return bytes;
        }

        /// <summary>
        /// Generates a password string with a specified length
        /// </summary>
        /// <param name="length">Length of the password that will be generated</param>
        /// <param name="generationFlags">Configuration for the password generation</param>
        /// <returns>The generated password</returns>
        public static string GeneratePassword(int length, PasswordGenerationFlags generationFlags = PasswordGenerationFlags.Default)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            // Build character set
            string[] charSets = BuildCharacterSet(generationFlags).ToArray();
            if (charSets.Length == 0)
                throw new ArgumentException("No char sets selected.", nameof(generationFlags));
            if (length < charSets.Length)
                throw new ArgumentException("Password length is too small to contain characters form all selected sets.", nameof(generationFlags));

            // Use cryptographically safe random number generator
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                // Generate random numbers
                var randomBytes = new byte[length];
                randomNumberGenerator.GetBytes(randomBytes);

                // Select generation method
                if (generationFlags.HasFlag(PasswordGenerationFlags.EquallyDistributed))
                {
                    int fullCharSetLength = charSets.Sum(charSet => charSet.Length);

                    // collect characters from the different char sets
                    var charCollector = new StringBuilder(length);
                    for (int i = 0; i < charSets.Length; i++)
                    {
                        string charSet = charSets[i];
                        int charsFromThisSet = i == charSets.Length - 1 ? length - charCollector.Length : (int)Math.Round((double)charSet.Length / fullCharSetLength * length);
                        for (int j = 0; j < charsFromThisSet; j++)
                            charCollector.Append(GetRandomCharFromCharSet(charSet, randomBytes[charCollector.Length]));
                    }

                    // shuffle characters
                    return ShuffleCharacters(charCollector.ToString(), randomNumberGenerator);
                }
                else
                {
                    // Combine available characters to one single character set
                    string fullCharSet = ShuffleCharacters(string.Join(string.Empty, charSets), randomNumberGenerator);

                    // Build random password
                    return new string(randomBytes.Select(number => GetRandomCharFromCharSet(fullCharSet, number)).ToArray());
                }
            }
        }

        /// <summary>
        /// Checks if two secrets are equal. As long as both secrets have the same length, this comparison will execute in constant time.
        /// </summary>
        /// <param name="secret1">First secret</param>
        /// <param name="secret2">Other secret</param>
        /// <returns>True, if both secrets were the same</returns>
        public static bool SecureCompare(string secret1, string secret2)
        {
            if (secret1 == null)
                throw new ArgumentNullException(nameof(secret1));
            if (secret2 == null)
                throw new ArgumentNullException(nameof(secret2));

            // Of course this is not constant-time, but I think this case is uncritical.
            if (secret1.Length != secret2.Length)
                return false;

            // Get bytes
            byte[] bytes1 = Encoding.UTF8.GetBytes(secret1);
            byte[] bytes2 = Encoding.UTF8.GetBytes(secret2);

            // Calculate difference
            var diff = 0;
            for (var i = 0; i < bytes1.Length; i++)
                diff |= bytes1[i] ^ bytes2[i];

            // Return if secrets were different
            return diff == 0;
        }

        private static IEnumerable<string> BuildCharacterSet(PasswordGenerationFlags generationFlags)
        {
            bool readable = generationFlags.HasFlag(PasswordGenerationFlags.ReadableCharSet);

            if (generationFlags.HasFlag(PasswordGenerationFlags.WithNumbers))
                yield return readable ? "23456789" : "1234567890";

            if (generationFlags.HasFlag(PasswordGenerationFlags.WithUpperCaseChars))
                yield return readable ? "ABCDEFGHKLMNPQRSTUVWXYZ" : "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (generationFlags.HasFlag(PasswordGenerationFlags.WithLowerCaseChars))
                yield return readable ? "abcdefghkmnpqrstuvwxyz" : "abcdefghijklmnopqrstuvwxyz";
        }

        private static string ShuffleCharacters(string value, RandomNumberGenerator randomNumberGenerator)
        {
            // Generate random numbers
            var randomBytes = new byte[value.Length];
            randomNumberGenerator.GetBytes(randomBytes);

            // Select random characters from the string, add it to the new string and remove it from the collection
            var valueChars = new StringBuilder(value);
            var stringBuilder = new StringBuilder(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                int charIndex = randomBytes[i] % valueChars.Length;
                stringBuilder.Append(valueChars[charIndex]);
                valueChars.Remove(charIndex, 1);
            }

            // Get the shuffled string
            return stringBuilder.ToString();
        }

        private static char GetRandomCharFromCharSet(string charSet, byte randomNumber)
        {
            // scale random number down to available range
            int index = (int)Math.Round((double)randomNumber / 0xff * (charSet.Length - 1));
            return charSet[index];
        }
    }
}
