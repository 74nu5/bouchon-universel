namespace BouchonUniversel.Middlewares
{
    #region Usings

    using System;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL.DAO;

    using Microsoft.AspNetCore.Http;

    #endregion

    /// <summary>Classe of install middleware which redirect to install page when settings database is empty.</summary>
    public class InstallMiddleware : IMiddleware
    {
        private readonly SettingsBouchonDAO settingsBouchonDAO;

        private readonly InstallationState installationState;

        /// <summary>Initializes a new instance of the <see cref="InstallMiddleware" /> class.</summary>
        /// <param name="settingsBouchonDAO">Settings database service.</param>
        /// <param name="installationState">État d'installation mis en cache.</param>
        public InstallMiddleware(SettingsBouchonDAO settingsBouchonDAO, InstallationState installationState)
        {
            this.settingsBouchonDAO = settingsBouchonDAO;
            this.installationState = installationState;
        }

        /// <summary>Request handling method.</summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> for the current request.</param>
        /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the execution of this middleware.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            // Tant que l'installation n'est pas confirmée, on vérifie l'état ; une fois installée, plus aucune requête base.
            if (!this.installationState.IsInstalled)
            {
                if (this.settingsBouchonDAO.IsSettingsMissing())
                {
                    if (!context.Request.Path.StartsWithSegments("/api") && !context.Request.Path.StartsWithSegments("/install"))
                    {
                        context.Response.Redirect("/install/");
                        return;
                    }
                }
                else
                {
                    this.installationState.MarkInstalled();
                }
            }

            await next(context).ConfigureAwait(false);
        }
    }
}
