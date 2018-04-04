namespace BouchonUniversel
{
    #region Usings

    using DAL;

    using JetBrains.Annotations;

    using Metier;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Models;

    using Swashbuckle.AspNetCore.Swagger;

    #endregion

    /// <summary>The startup.</summary>
    [UsedImplicitly]
    public class Startup
    {
        #region Champs et constantes statiques

        /// <summary>The titre swagger.</summary>
        private const string TitreSwagger = "API du bouchon";

        /// <summary>The version swagger.</summary>
        private const string VersionSwagger = "v1";

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="Startup"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        #endregion

        #region Propriétés et indexeurs

        /// <summary>Gets the configuration.</summary>
        public IConfiguration Configuration { get; }

        #endregion

        #region Méthodes publiques

        /// <summary>The configure services.</summary>
        /// <param name="services">The services.</param>
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddOptions();
            services.Configure<ApplicationSettings>(this.Configuration.GetSection("Bouchon"));

            services.AddEntityFrameworkInMemoryDatabase();
            services.AddDbContext<MemoryContext>((serviceProvider, options) => options.UseInMemoryDatabase("BouchonDB").UseInternalServiceProvider(serviceProvider));

            services.AddSwaggerGen(c => { c.SwaggerDoc(VersionSwagger, new Info { Title = TitreSwagger, Version = VersionSwagger }); });


            services.AddTransient<BouchonMetier>();
            services.AddTransient<BouchonInitializer>();
            services.AddTransient<MemoryContextDAO>();
        }

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

        #endregion
    }
}