namespace BouchonUniversel
{
    #region Usings

    using System;
    using System.IO;

    using DAL;
    using DAL.DAO;

    using JetBrains.Annotations;

    using Metier;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Models;

    using Swashbuckle.AspNetCore.Swagger;

    using Utils.Http;

    #endregion

    /// <summary>The startup.</summary>
    [UsedImplicitly]
    public sealed class Startup
    {
        #region Champs et constantes statiques

        /// <summary>The titre swagger.</summary>
        private const string TitreSwagger = "API du bouchon";

        /// <summary>The version swagger.</summary>
        private const string VersionSwagger = "v1";

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="Startup" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        #endregion

        #region Propriétés et indexeurs

        /// <summary>Gets the configuration.</summary>
        private IConfiguration Configuration { get; }

        #endregion

        #region Méthodes publiques

        /// <summary>The configure.</summary>
        /// <param name="app">The app.</param>
        /// <param name="env">The env.</param>
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/{VersionSwagger}/swagger.json", TitreSwagger); });

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); });
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
                        new Info
                        {
                            Title = TitreSwagger,
                            Description = "Une api permettant de créer des bouchons.",
                            Version = VersionSwagger,
                            Contact = new Contact { Name = "Romain Avonde", Email = "romain@avonde.eu" }
                        });

                    var basePath = AppContext.BaseDirectory;
                    var xmlPath = Path.Combine(basePath, "BouchonUniversel.xml");
                    c.IncludeXmlComments(xmlPath);
                });

            services.AddSingleton<HttpService>();

            services.AddTransient<SettingsBouchonMetier>();
            services.AddTransient<BouchonInitializer>();
            services.AddTransient<SettingsBouchonDAO>();
            services.AddTransient<ServicesDAO>();
            services.AddTransient<EnvironnementDAO>();
            services.AddTransient<BouchonsMetier>();
        }

        #endregion

        #region Méthodes privées

        /// <summary>The get sqlite connection.</summary>
        /// <returns>The <see cref="SqliteConnection" />.</returns>
        private SqliteConnection GetSqliteConnection()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = this.Configuration.GetConnectionString("BDDConnection") };
            return new SqliteConnection(connectionStringBuilder.ToString());
        }

        #endregion
    }
}