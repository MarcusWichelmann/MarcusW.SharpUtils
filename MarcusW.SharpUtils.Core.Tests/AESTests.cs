using MarcusW.SharpUtils.Core.Cryptography;
using Xunit;

namespace MarcusW.SharpUtils.Core.Tests
{
    public class AESTests
    {
        [Theory]
        [InlineData(new byte[0])]
        [InlineData(new[] { (byte)0 })]
        [InlineData(new[] { (byte)42, (byte)128, (byte)85, (byte)244, (byte)5, (byte)0, (byte)59 })]
        [InlineData(new[] { (byte)73, (byte)82, (byte)98, (byte)185, (byte)7, (byte)11, (byte)255 })]
        public void EncryptsAndDecryptsBytesCorrectlyUsingSameInstances(byte[] data)
        {
            AES aes = AES.GenerateKey(4096);
            byte[] encrypted = aes.Encrypt(data);
            byte[] decrypted = aes.Decrypt(encrypted);

            Assert.Equal(data, decrypted);
        }

        [Theory]
        [InlineData(new byte[0])]
        [InlineData(new[] { (byte)0 })]
        [InlineData(new[] { (byte)42, (byte)128, (byte)85, (byte)244, (byte)5, (byte)0, (byte)59 })]
        [InlineData(new[] { (byte)73, (byte)82, (byte)98, (byte)185, (byte)7, (byte)11, (byte)255 })]
        public void EncryptsAndDecryptsBytesCorrectlyUsingDifferentInstances(byte[] data)
        {
            AES aes = AES.GenerateKey(4096);
            string key = aes.HexKey;
            byte[] encrypted = aes.Encrypt(data);

            AES newAes = AES.FromHexKey(key);
            byte[] decrypted = newAes.Decrypt(encrypted);

            Assert.Equal(data, decrypted);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("This is a test")]
        public void EncryptsAndDecryptsStringCorrectly(string data)
        {
            AES aes = AES.GenerateKey(4096);
            byte[] encrypted = aes.EncryptString(data);
            string decrypted = aes.DecryptString(encrypted);

            Assert.Equal(data, decrypted);
        }
    }
}
