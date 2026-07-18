namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using System;
    using System.Net;

    using BouchonUniversel.Exceptions;

    #endregion

    /// <summary>The response erreur.</summary>
    public class ResponseErreur
    {
        /// <summary>Gets or sets the code.</summary>
        public int Code { private get; set; }

        /// <summary>Gets or sets the HTTP status code associated with the error.</summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;

        /// <summary>Gets the code message.</summary>
        public object CodeMessage
            => $"{this.Code} - {this.Message}";

        /// <summary>Gets or sets the message.</summary>
        public string Message { private get; set; }

        /// <summary>Construit une <see cref="ResponseErreur" /> avec un code interne et un statut HTTP sémantique selon le type d'exception.</summary>
        /// <param name="exception">L'exception à traduire.</param>
        /// <returns>The <see cref="ResponseErreur" />.</returns>
        public static ResponseErreur FromException(Exception exception)
            => exception switch
            {
                KeyNotFoundException => new () { Message = exception.Message, Code = 1001, StatusCode = HttpStatusCode.NotFound },
                EnvironmentNotFoundException => new () { Message = exception.Message, Code = 1002, StatusCode = HttpStatusCode.NotFound },
                _ => new () { Message = exception?.Message, Code = 1999, StatusCode = HttpStatusCode.InternalServerError },
            };
    }
}
