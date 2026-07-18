namespace BouchonUniversel.Tests
{
    using System;
    using System.Net;

    using BouchonUniversel.Exceptions;
    using BouchonUniversel.Models.Bouchons;

    using Xunit;

    /// <summary>Tests du mapping exception -> code HTTP sémantique (<see cref="ResponseErreur.FromException" />).</summary>
    public class ResponseErreurTests
    {
        [Fact]
        public void FromException_KeyNotFound_MapsTo404AndCode1001()
        {
            var erreur = ResponseErreur.FromException(new KeyNotFoundException("clé absente"));

            Assert.Equal(HttpStatusCode.NotFound, erreur.StatusCode);
            Assert.StartsWith("1001", erreur.CodeMessage.ToString());
        }

        [Fact]
        public void FromException_EnvironmentNotFound_MapsTo404AndCode1002()
        {
            var erreur = ResponseErreur.FromException(new EnvironmentNotFoundException("env absent"));

            Assert.Equal(HttpStatusCode.NotFound, erreur.StatusCode);
            Assert.StartsWith("1002", erreur.CodeMessage.ToString());
        }

        [Fact]
        public void FromException_UnknownException_MapsTo500AndCode1999()
        {
            var erreur = ResponseErreur.FromException(new InvalidOperationException("boom"));

            Assert.Equal(HttpStatusCode.InternalServerError, erreur.StatusCode);
            Assert.StartsWith("1999", erreur.CodeMessage.ToString());
        }
    }
}
