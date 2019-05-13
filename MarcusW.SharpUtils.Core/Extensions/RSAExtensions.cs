using System;
using System.Security.Cryptography;

namespace MarcusW.SharpUtils.Core.Extensions
{
    public static class RSAExtensions
    {
        /// <summary>
        /// Checks if the <see cref="RSA"/> object contains any information of the private key.
        /// </summary>
        /// <param name="rsa"><see cref="RSA"/> object</param>
        /// <returns>True, if any private information were found. If not, false.</returns>
        public static bool GetHasPrivateKey(this RSA rsa)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            RSAParameters parameters;
            try
            {
                parameters = rsa.ExportParameters(true);
            }
            catch (CryptographicException)
            {
                return false;
            }

            return parameters.D != null || parameters.P != null || parameters.Q != null || parameters.DP != null || parameters.DQ != null || parameters.InverseQ != null;
        }

        /// <summary>
        /// Checks if the <see cref="RSA"/> object contains all public information.
        /// </summary>
        /// <param name="rsa"><see cref="RSA"/> object</param>
        /// <returns>True, if all public information were found. If not, false.</returns>
        public static bool GetHasPublicKey(this RSA rsa)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            RSAParameters parameters;
            try
            {
                parameters = rsa.ExportParameters(false);
            }
            catch (Exception)
            {
                return false;
            }

            return parameters.Modulus != null && parameters.Exponent != null;
        }
    }
}
