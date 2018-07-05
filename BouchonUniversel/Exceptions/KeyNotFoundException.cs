namespace BouchonUniversel.Exceptions
{
    #region Usings

    using System;
    using System.Runtime.Serialization;

    #endregion

    /// <summary>The key not found exception.</summary>
    public class KeyNotFoundException : Exception
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="KeyNotFoundException" /> class.</summary>
        public KeyNotFoundException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="KeyNotFoundException"/> class.</summary>
        /// <param name="message">The message.</param>
        public KeyNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="KeyNotFoundException"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public KeyNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="KeyNotFoundException"/> class.</summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected KeyNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}