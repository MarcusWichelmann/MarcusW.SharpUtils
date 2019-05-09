using System;

namespace MarcusW.SharpUtils.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] bytes) => HexConvert.ToHexString(bytes);

        public static string ToBase64String(this byte[] bytes) => Convert.ToBase64String(bytes);
    }
}
