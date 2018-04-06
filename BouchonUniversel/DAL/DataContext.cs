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


        /// <summary>
        ///     Override this method to further configure the model that was discovered by convention from the entity types
        ///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting
        ///     model may be cached
        ///     and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <remarks>
        ///     If a model is explicitly set on the options for this context (via
        ///     <see
        ///         cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />
        ///     )
        ///     then this method will not be run.
        /// </remarks>
        /// <param name="modelBuilder">
        ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
        ///     define extension methods on this object that allow you to configure aspects of the model that are specific
        ///     to a given database.
        /// </param>
        public DbSet<Service> Services { get; set; }

        /// <summary>
        ///     Override this method to further configure the model that was discovered by convention from the entity types
        ///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting
        ///     model may be cached
        ///     and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <remarks>
        ///     If a model is explicitly set on the options for this context (via
        ///     <see
        ///         cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />
        ///     )
        ///     then this method will not be run.
        /// </remarks>
        /// <param name="modelBuilder">
        ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
        ///     define extension methods on this object that allow you to configure aspects of the model that are specific
        ///     to a given database.
        /// </param>
        public DbSet<Environnement> Environnement { get; set; }

        #endregion

        #region Méthodes protected

        /// <summary>Override this method to further configure the model that was discovered by convention from the entity types
        ///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1"/> properties on your derived context. The resulting
        ///     model may be cached
        ///     and re-used for subsequent instances of your derived context.</summary>
        /// <remarks>If a model is explicitly set on the options for this context (via<see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)"/>
        ///     )
        ///     then this method will not be run.</remarks>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        ///     define extension methods on this object that allow you to configure aspects of the model that are specific
        ///     to a given database.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bouchon>().HasKey(bouchon => new { bouchon.BaseUrl, bouchon.ServiceUrl });
            modelBuilder.Entity<Service>().HasOne(service => service.Environnement).WithMany(environnement => environnement.Services);
            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}