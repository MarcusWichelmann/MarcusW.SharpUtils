using System;

namespace MarcusW.SharpUtils.Core.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ParseAsHex(this string value) => HexConvert.FromHexString(value);

        public static byte[] ParseAsBase64(this string value) => Convert.FromBase64String(value);
    }
}
