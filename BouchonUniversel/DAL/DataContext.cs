namespace BouchonUniversel.DAL
{
    #region Usings

    using BouchonUniversel.Models;
    using BouchonUniversel.Models.Bouchons;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;

    #endregion

    /// <summary>The memory context.</summary>
    [UsedImplicitly]
    public class DataContext : DbContext
    {
        /// <summary>Initializes a new instance of the <see cref="DataContext" /> class.</summary>
        [UsedImplicitly]
        public DataContext()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DataContext" /> class.</summary>
        /// <param name="option">The option.</param>
        [UsedImplicitly]
        public DataContext(DbContextOptions option)
            : base(option)
        {
        }

        /// <summary>Gets or sets the bouchons.</summary>
        public DbSet<Bouchon> Bouchons { get; set; }

        /// <summary>Gets or sets the environnement.</summary>
        public DbSet<Environnement> Environnement { get; set; }

        /// <summary>Gets or sets the services.</summary>
        public DbSet<Service> Services { get; set; }

        /// <summary>Gets or sets the options bouchons.</summary>
        public DbSet<SettingsBouchon> SettingsBouchon { get; set; }

        /// <summary>The on model creating.</summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                return;
            }

            modelBuilder.Entity<Service>().HasOne(service => service.Environnement).WithMany(environnement => environnement.Services);

            modelBuilder.Entity<Service>().HasIndex(serv => new { serv.Cle, serv.EnvironnementId }).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
