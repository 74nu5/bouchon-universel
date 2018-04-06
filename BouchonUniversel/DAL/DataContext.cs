namespace BouchonUniversel.DAL
{
    #region Usings

    using System;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;

    using Models;
    using Models.Bouchons;

    #endregion

    /// <summary>The memory context.</summary>
    [UsedImplicitly]
    public class DataContext : DbContext
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="DataContext" /> class.</summary>
        [UsedImplicitly]
        public DataContext()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DataContext"/> class.</summary>
        /// <param name="services">The services.</param>
        /// <param name="option">The option.</param>
        [UsedImplicitly]
        public DataContext(IServiceProvider services, DbContextOptions option) : base(option)
        {
        }

        #endregion

        #region Propriétés et indexeurs

        /// <summary>Gets or sets the options bouchons.</summary>
        public DbSet<SettingsBouchon> SettingsBouchon { get; set; }

        /// <summary>Gets or sets the bouchons.</summary>
        public DbSet<Bouchon> Bouchons { get; set; }

        /// <summary>Gets or sets the services.</summary>
        public DbSet<Models.Bouchons.Service> Services { get; set; }

        /// <summary>Gets or sets the environnement.</summary>
        public DbSet<Environnement> Environnement { get; set; }

        #endregion

        #region Méthodes protected

        /// <summary>The on model creating.</summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bouchon>().HasKey(bouchon => new { bouchon.BaseUrl, bouchon.ServiceUrl });
            modelBuilder.Entity<Service>().HasOne(service => service.Environnement).WithMany(environnement => environnement.Services);

            modelBuilder.Entity<Service>().HasIndex(serv => new { serv.Cle, serv.EnvironnementId }).IsUnique();
            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}