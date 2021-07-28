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

        /// <summary>Initializes a new instance of the <see cref="InstallMiddleware" /> class.</summary>
        /// <param name="settingsBouchonDAO">Settings database service.</param>
        public InstallMiddleware(SettingsBouchonDAO settingsBouchonDAO)
            => this.settingsBouchonDAO = settingsBouchonDAO;

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

            if (this.settingsBouchonDAO.IsSettingsMissing() && !context.Request.Path.StartsWithSegments("/api") && !context.Request.Path.StartsWithSegments("/install"))
            {
                context.Response.Redirect("/install/");
                return;
            }

            await next(context).ConfigureAwait(false);
        }
    }
}
