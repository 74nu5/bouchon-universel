namespace BouchonUniversel.Middlewares.Extensions
{
    #region Usings

    using Microsoft.AspNetCore.Builder;

    #endregion

    /// <summary>
    /// Extension class of install middleware.
    /// </summary>
    public static class InstallMiddlewareExtension
    {
        /// <summary>
        /// Method that add install middleware.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>Returns the application builder.</returns>
        public static IApplicationBuilder UseInstall(this IApplicationBuilder builder)
            => builder.UseMiddleware<InstallMiddleware>();
    }
}
