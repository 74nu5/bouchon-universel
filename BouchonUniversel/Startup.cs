namespace BouchonUniversel
{
    #region Usings

    using System;
    using System.IO;
using System.Linq;

    using BouchonUniversel.DAL;
    using BouchonUniversel.DAL.DAO;
    using BouchonUniversel.Metier;
    using BouchonUniversel.Middlewares;
    using BouchonUniversel.Middlewares.Extensions;
    using BouchonUniversel.Models;
    using BouchonUniversel.Services;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
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
            }

            app.UseStaticFiles();

            app.UseInstall();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/{VersionSwagger}/swagger.json", TitreSwagger); });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(
                             endpoints => endpoints.MapControllerRoute(
                                                                       "default",
                                                                       "{controller=Home}/{action=Index}/{id?}"));
        }

        /// <summary>The configure services.</summary>
        /// <param name="services">The services.</param>
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddOptions();
            services.Configure<ApplicationSettings>(this.Configuration.GetSection("Bouchon"));

            services.AddEntityFrameworkSqlite();
            services.AddDbContext<DataContext>(builder => builder.UseSqlite(this.GetSqliteConnection()));

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

                                   });

            services.AddSingleton<HttpService>();

            // Configuration des patterns de dates chargée une seule fois (auparavant relue à chaque requête).
            services.AddSingleton<PatternDateFormatProvider>();

            services.AddTransient<SettingsBouchonMetier>();
            services.AddTransient<BouchonInitializer>();
            services.AddTransient<SettingsBouchonDAO>();
            services.AddTransient<ServicesDAO>();
            services.AddTransient<EnvironnementDAO>();
            services.AddTransient<BouchonsMetier>();
            services.AddTransient<InstallMiddleware>();
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
