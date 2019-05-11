using System;
using Xunit;

namespace MarcusW.SharpUtils.Core.Tests
{
    public class HexConvertTests
    {
        [Fact]
        public void ThrowsWhenBytesNull()
        {
            Assert.Throws<ArgumentNullException>(() => HexConvert.ToHexString(null));
        }

        [Fact]
        public void ThrowsWhenHexStringNull()
        {
            Assert.Throws<ArgumentNullException>(() => HexConvert.FromHexString(null));
        }

        [Theory]
        [InlineData(new byte[0], "")]
        [InlineData(new byte[] { 222 }, "de")]
        [InlineData(new byte[] { 0, 0, 0 }, "000000")]
        [InlineData(new byte[] { 255, 255, 255 }, "ffffff")]
        [InlineData(new byte[] { 5, 172, 99, 201 }, "05ac63c9")]
        [InlineData(new byte[] { 83, 231, 185, 21, 132 }, "53e7b91584")]
        public void ConvertsCorrectly(byte[] bytes, string expectedHex)
        {
            string hex = HexConvert.ToHexString(bytes);
            Assert.Equal(expectedHex, hex);

            byte[] resultBytes = HexConvert.FromHexString(hex);
            Assert.Equal(bytes, resultBytes);
        }
    }
}
