using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MarcusW.SharpUtils.Core.Cryptography
{
    public static class RSAKeyLoader
    {
        public static RSA LoadFromCertificate(string pfxFileName, string password = null, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
        {
            if (pfxFileName == null)
                throw new ArgumentNullException(nameof(pfxFileName));

            return LoadFromCertificate(new X509Certificate2(pfxFileName, password ?? string.Empty, keyStorageFlags));
        }

        public static RSA LoadFromCertificate(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));

            return certificate.HasPrivateKey ? certificate.GetRSAPrivateKey() : certificate.GetRSAPublicKey();
        }
    }
}
