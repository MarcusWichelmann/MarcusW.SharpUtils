using System;

namespace MarcusW.SharpUtils.Core.Cryptography
{
    [Flags]
    public enum PasswordGenerationFlags
    {
        /// <summary>
        /// Password should contain numbers
        /// </summary>
        WithNumbers = 1 << 0,

        /// <summary>
        /// Password should contain upper case letters
        /// </summary>
        WithUpperCaseChars = 1 << 1,

        /// <summary>
        /// Password should contain lower case letters
        /// </summary>
        WithLowerCaseChars = 1 << 2,

        /// <summary>
        /// Password should be made of distinctive characters only
        /// </summary>
        ReadableCharSet = 1 << 3,

        /// <summary>
        /// The occurence of each type of character (number, upper case, lower case) should be statically based on the length of the password instead of being random.
        /// </summary>
        EquallyDistributed = 1 << 4,

        /// <summary>
        /// Default
        /// </summary>
        Default = WithNumbers | WithUpperCaseChars | WithLowerCaseChars
    }
}
