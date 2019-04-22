using System;

namespace MarcusW.SharpUtils.Redis
{
    public class CannotAcquireLockException : Exception
    {
        public CannotAcquireLockException() { }

        public CannotAcquireLockException(string message) : base(message) { }

        public CannotAcquireLockException(string message, Exception innerException) : base(message, innerException) { }
    }
}
