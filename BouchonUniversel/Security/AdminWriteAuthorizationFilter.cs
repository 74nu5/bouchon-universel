namespace BouchonUniversel.Security
{
    #region Usings

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    #endregion

    /// <summary>
    ///     Exige le rôle <see cref="Roles.Admin" /> pour toute requête de modification (POST/PUT/PATCH/DELETE)
    ///     des pages d'administration. Les lectures, l'API de bouchonnage, le compte et l'installation sont exclus.
    ///     Un lecteur (<see cref="Roles.Viewer" />) peut ainsi consulter mais pas modifier.
    /// </summary>
    public sealed class AdminWriteAuthorizationFilter : IAuthorizationFilter
    {
        /// <inheritdoc />
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            if (HttpMethods.IsGet(request.Method)
                || HttpMethods.IsHead(request.Method)
                || HttpMethods.IsOptions(request.Method))
            {
                return;
            }

            var path = request.Path;
            if (path.StartsWithSegments("/api")
                || path.StartsWithSegments("/account")
                || path.StartsWithSegments("/install"))
            {
                return;
            }

            if (!context.HttpContext.User.IsInRole(Roles.Admin))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
