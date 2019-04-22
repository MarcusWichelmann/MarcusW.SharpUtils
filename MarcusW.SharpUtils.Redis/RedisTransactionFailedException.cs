using System;

namespace MarcusW.SharpUtils.Redis
{
    public class RedisTransactionFailedException : Exception
    {
        public RedisTransactionFailedException() { }

        public RedisTransactionFailedException(string message) : base(message) { }

        public RedisTransactionFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
