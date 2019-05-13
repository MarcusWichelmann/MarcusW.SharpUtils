using MarcusW.SharpUtils.Core.Cryptography;
using MarcusW.SharpUtils.Core.Extensions;
using Xunit;

namespace MarcusW.SharpUtils.Core.Tests
{
    public class RSAKeyLoaderTests
    {
        private const string TestCertFileName = "Files/test-cert.pfx";
        private const string TestKeyFileName = "Files/test-key.pfx";
        private const string TestKeyPassword = "123456";

        [Fact]
        public void LoadsPublicKeyFromCertFile()
        {
            var rsa = RSAKeyLoader.LoadFromCertificate(TestCertFileName);
            Assert.True(rsa.GetHasPublicKey());
            Assert.False(rsa.GetHasPrivateKey());
        }

        [Fact]
        public void LoadsPrivateKeyFromKeyFile()
        {
            var rsa = RSAKeyLoader.LoadFromCertificate(TestKeyFileName, TestKeyPassword);
            Assert.True(rsa.GetHasPublicKey());
            Assert.True(rsa.GetHasPrivateKey());
        }
    }
}
