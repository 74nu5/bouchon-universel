namespace BouchonUniversel.Metier
{
    /// <summary>
    ///     Verbes HTTP gérés par le métier de bouchonnage.
    ///     Remplace l'ancienne dépendance à l'énumération interne de Kestrel
    ///     (<c>Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod</c>), qui n'est pas une API publique stable.
    /// </summary>
    public enum HttpVerb
    {
        /// <summary>Aucun verbe.</summary>
        None = 0,

        /// <summary>Verbe GET.</summary>
        Get,

        /// <summary>Verbe PUT.</summary>
        Put,

        /// <summary>Verbe DELETE.</summary>
        Delete,

        /// <summary>Verbe POST.</summary>
        Post,

        /// <summary>Verbe HEAD.</summary>
        Head,

        /// <summary>Verbe TRACE.</summary>
        Trace,

        /// <summary>Verbe PATCH.</summary>
        Patch,

        /// <summary>Verbe CONNECT.</summary>
        Connect,

        /// <summary>Verbe OPTIONS.</summary>
        Options,

        /// <summary>Verbe personnalisé.</summary>
        Custom,
    }
}
