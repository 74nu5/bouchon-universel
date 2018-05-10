namespace BouchonUniversel.ApiTest
{
    #region Usings

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    /// <summary>The startup.</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class Startup
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="Startup"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        #endregion

        #region Propriétés et indexeurs

        /// <summary>Gets the configuration.</summary>
        private IConfiguration Configuration { get; }

        #endregion

        #region Méthodes publiques

        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>The configure.</summary>
        /// <param name="app">The app.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        /// This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>The configure services.</summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services) => services.AddMvc();

        #endregion
    }
}