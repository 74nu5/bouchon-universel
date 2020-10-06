namespace BouchonUniversel.ApiTest
{
    #region Usings

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    #endregion

    /// <summary>The startup.</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class Startup
    {
        /// <summary>The titre swagger.</summary>
        private const string TitreSwagger = "API test du bouchon";

        /// <summary>The version swagger.</summary>
        private const string VersionSwagger = "v1";

        /// <summary>Initializes a new instance of the <see cref="Startup" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        /// <summary>Gets the configuration.</summary>
        private IConfiguration Configuration { get; }

        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>The configure.</summary>
        /// <param name="app">The app.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/{VersionSwagger}/swagger.json", TitreSwagger); });

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endppoints => endppoints.MapDefaultControllerRoute());
        }

        /// This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>The configure services.</summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        VersionSwagger,
                        new OpenApiInfo
                        {
                            Title = TitreSwagger,
                            Description = "Une api de test permettant de tester des bouchons.",
                            Version = VersionSwagger,
                            Contact = new OpenApiContact { Name = "Romain Avonde", Email = "romain@avonde.eu" }
                        });
                });

            services.AddMvc();
        }
    }
}
