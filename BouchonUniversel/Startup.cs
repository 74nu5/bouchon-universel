namespace BouchonUniversel
{
    #region Usings

    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.RateLimiting;

    using BouchonUniversel.DAL;
    using BouchonUniversel.DAL.DAO;
    using BouchonUniversel.Metier;
    using BouchonUniversel.Middlewares;
    using BouchonUniversel.Middlewares.Extensions;
    using BouchonUniversel.Models;
    using BouchonUniversel.Services;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.RateLimiting;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Ustilz.Http;

    #endregion

    /// <summary>The startup.</summary>
    [UsedImplicitly]
    public sealed class Startup
    {
        /// <summary>The titre swagger.</summary>
        private const string TitreSwagger = "API du bouchon";

        /// <summary>The version swagger.</summary>
        private const string VersionSwagger = "v1";

        /// <summary>Nom de la politique CORS permissive appliquée (serveur de mocks appelé depuis n'importe quelle origine).</summary>
        private const string CorsPolicy = "bouchon";

        /// <summary>Nom de la politique de limitation de débit du login (anti-bruteforce).</summary>
        private const string LoginRateLimitPolicy = "login";

        /// <summary>Initializes a new instance of the <see cref="Startup" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        /// <summary>Gets the configuration.</summary>
        private IConfiguration Configuration { get; }

        /// <summary>The configure.</summary>
        /// <param name="app">The app.</param>
        /// <param name="env">The env.</param>
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // En-têtes de sécurité appliqués à toutes les réponses.
            app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["Referrer-Policy"] = "no-referrer";
                headers["Content-Security-Policy"] = "default-src 'self' 'unsafe-inline' 'unsafe-eval' data: blob: https:;";
                await next().ConfigureAwait(false);
            });

            app.UseStaticFiles();

            app.UseInstall();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/{VersionSwagger}/swagger.json", TitreSwagger); });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(CorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>The configure services.</summary>
        /// <param name="services">The services.</param>
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            // Authentification admin : activée uniquement si un identifiant et un mot de passe sont configurés (section « Admin »).
            services.Configure<AdminSettings>(this.Configuration.GetSection("Admin"));
            var adminSettings = this.Configuration.GetSection("Admin").Get<AdminSettings>() ?? new AdminSettings();
            var adminAuthEnabled = adminSettings.IsEnabled;

            services.AddMvc(options =>
            {
                if (adminAuthEnabled)
                {
                    // Toutes les pages d'administration exigent une authentification ; l'API et le compte sont [AllowAnonymous].
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/account/login";
                        options.LogoutPath = "/account/logout";
                        options.AccessDeniedPath = "/account/login";
                    });

            services.AddOptions();
            services.Configure<ApplicationSettings>(this.Configuration.GetSection("Bouchon"));

            services.AddEntityFrameworkSqlite();
            services.AddDbContext<DataContext>(builder => builder.UseSqlite(this.GetSqliteConnection()));

            // CORS permissif : un serveur de mocks est typiquement appelé depuis d'autres origines (gère aussi le préflight OPTIONS).
            services.AddCors(options => options.AddPolicy(
                CorsPolicy,
                policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            // Sondes de santé : liveness + vérification d'accès à la base (readiness).
            services.AddHealthChecks().AddDbContextCheck<DataContext>();

            // Limitation de débit du login (anti-bruteforce) : 5 tentatives par minute et par IP.
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy(LoginRateLimitPolicy, context => RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
            });

            services.AddSwaggerGen(
                                   c =>
                                   {
                                       c.SwaggerDoc(
                                                    VersionSwagger,
                                                    new ()
                                                    {
                                                        Title = TitreSwagger,
                                                        Description = "Une api permettant de créer des bouchons.",
                                                        Version = VersionSwagger,
                                                        Contact = new () { Name = "Romain Avonde", Email = "romain@avonde.email" }
                                                    });

                                       var basePath = AppContext.BaseDirectory;
                                       var xmlPath = Path.Combine(basePath, "BouchonUniversel.xml");
                                       c.IncludeXmlComments(xmlPath);

                                       // La documentation ne couvre que l'API de bouchonnage ; les contrôleurs MVC
                                       // d'administration (routage conventionnel, sans verbe explicite) en sont exclus.
                                       c.DocInclusionPredicate((_, apiDescription) => apiDescription.RelativePath?.StartsWith("api/", StringComparison.OrdinalIgnoreCase) ?? false);
                                   });

            services.AddSingleton<HttpService>();

            // Clients HTTP pour le passthrough des verbes PUT/DELETE/PATCH (non gérés par Ustilz.Http).
            services.AddHttpClient();

            // Accès au contexte HTTP (coupure de connexion simulée par le chaos).
            services.AddHttpContextAccessor();

            // Configuration des patterns de dates chargée une seule fois (auparavant relue à chaque requête).
            services.AddSingleton<PatternDateFormatProvider>();

            services.AddTransient<SettingsBouchonMetier>();
            services.AddTransient<BouchonInitializer>();
            services.AddTransient<SettingsBouchonDAO>();
            services.AddTransient<ServicesDAO>();
            services.AddTransient<EnvironnementDAO>();
            services.AddTransient<BouchonsMetier>();
            services.AddTransient<InstallMiddleware>();
            services.AddSingleton<InstallationState>();
            services.AddTransient<FileService>();
        }

        /// <summary>The get sqlite connection.</summary>
        /// <returns>The <see cref="SqliteConnection" />.</returns>
        private SqliteConnection GetSqliteConnection()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = this.Configuration.GetConnectionString("BDDConnection") };
            return new (connectionStringBuilder.ToString());
        }
    }
}
