namespace BouchonUniversel.Exceptions
{
    #region Usings

    using System;
    using System.Runtime.Serialization;

    #endregion

    /// <summary>The environment not found exception.</summary>
    public class EnvironmentNotFoundException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="EnvironmentNotFoundException" /> class.</summary>
        public EnvironmentNotFoundException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="EnvironmentNotFoundException" /> class.</summary>
        /// <param name="message">The message.</param>
        public EnvironmentNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="EnvironmentNotFoundException" /> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EnvironmentNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="EnvironmentNotFoundException" /> class.</summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected EnvironmentNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
