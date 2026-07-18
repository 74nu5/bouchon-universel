namespace BouchonUniversel.Security
{
    #region Usings

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>
    ///     Journalise (log structuré) les actions de modification de l'administration : identifiant, méthode, chemin, action.
    ///     Les requêtes en lecture et l'API de bouchonnage (<c>/api/*</c>) sont ignorées.
    /// </summary>
    public sealed class AuditActionFilter : IActionFilter
    {
        private readonly ILogger<AuditActionFilter> logger;

        /// <summary>Initializes a new instance of the <see cref="AuditActionFilter" /> class.</summary>
        /// <param name="logger">The logger.</param>
        public AuditActionFilter(ILogger<AuditActionFilter> logger)
            => this.logger = logger;

        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        /// <inheritdoc />
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var request = context.HttpContext.Request;

            if (HttpMethods.IsGet(request.Method)
                || HttpMethods.IsHead(request.Method)
                || HttpMethods.IsOptions(request.Method)
                || request.Path.StartsWithSegments("/api"))
            {
                return;
            }

            var user = context.HttpContext.User?.Identity?.Name ?? "anonyme";
            this.logger.LogInformation(
                "AUDIT utilisateur={User} méthode={Method} chemin={Path} action={Action}",
                user,
                request.Method,
                request.Path.Value,
                context.ActionDescriptor.DisplayName);
        }
    }
}
