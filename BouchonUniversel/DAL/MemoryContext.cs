namespace BouchonUniversel.DAL
{
    #region Usings

    using System;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;

    using Models;

    #endregion

    /// <summary>The memory context.</summary>
    [UsedImplicitly]
    public class MemoryContext : DbContext
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="MemoryContext" /> class.</summary>
        [UsedImplicitly]
        public MemoryContext()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MemoryContext"/> class.</summary>
        /// <param name="services">The services.</param>
        /// <param name="option">The option.</param>
        [UsedImplicitly]
        public MemoryContext(IServiceProvider services, DbContextOptions option) : base(option)
        {
        }

        #endregion

        #region Propriétés et indexeurs

        /// <summary>Gets or sets the options bouchons.</summary>
        public DbSet<SettingsBouchon> SettingsBouchon { get; set; }

        #endregion
    }
}