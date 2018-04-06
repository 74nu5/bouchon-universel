namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;

    using Models;

    #endregion

    /// <summary>The totem dao.</summary>
    /// <typeparam name="TContext">Type du contexte</typeparam>
    /// <typeparam name="TModel">Type du model</typeparam>
    /// <typeparam name="TIdentity">Type de la clé primaire du model</typeparam>
    public abstract class BaseDAO<TContext, TModel, TIdentity>
        where TModel : class, IDto<TIdentity> where TIdentity : IComparable<TIdentity> where TContext : DbContext
    {
        #region Champs

        /// <summary>Gets or sets the context.</summary>
        private readonly TContext Context;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BaseDAO{TContext,TModel,TIdentity}"/> class.</summary>
        /// <param name="context">The context.</param>
        protected BaseDAO(TContext context)
        {
            this.Context = context;
            this.Entities = this.Context.Set<TModel>();
        }

        #endregion

        #region Propriétés et indexeurs

        /// <summary>Gets or sets the entities.</summary>
        public DbSet<TModel> Entities { get; set; }

        #endregion

        #region Méthodes publiques

        /// <summary>The create.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> Create([NotNull] TModel model)
        {
            this.Context.Set<TModel>().Add(model);
            return await this.Context.SaveChangesAsync();
        }

        /// <summary>The exists.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Exists(TIdentity id) => this.Context.Set<TModel>().Any(e => e.Id.CompareTo(id) == 0);

        /// <summary>The get all.</summary>
        /// <param name="includes">The includes.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<TModel>> GetAll([NotNull] params Expression<Func<TModel, object>>[] includes)
        {
            var set = this.Context.Set<TModel>();
            var includeSet = includes.Aggregate<Expression<Func<TModel, object>>, IIncludableQueryable<TModel, object>>(
                null, (current, include) => current == null ? set.Include(include) : current.Include(include));
            return includeSet == null ? await set.ToListAsync() : await includeSet.ToListAsync();
        }

        /// <summary>The get all.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<List<TModel>> GetAll() => await this.Context.Set<TModel>().ToListAsync();

        /// <summary>The get details.</summary>
        /// <param name="id">The id.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<TModel> GetDetails(TIdentity id, [NotNull] params Expression<Func<TModel, object>>[] includes)
        {
            var set = this.Context.Set<TModel>();
            var includeSet = includes.Aggregate<Expression<Func<TModel, object>>, IIncludableQueryable<TModel, object>>(
                null, (current, include) => current == null ? set.Include(include) : current.Include(include));
            return includeSet == null
                ? await set.SingleOrDefaultAsync(model => model.Id.CompareTo(id) == 0)
                : await includeSet.SingleOrDefaultAsync(model => model.Id.CompareTo(id) == 0);
        }

        /// <summary>The get details.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<TModel> GetDetails(TIdentity id) => await this.Context.Set<TModel>().SingleOrDefaultAsync(model => model.Id.CompareTo(id) == 0);

        /// <summary>The remove.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> Remove([NotNull] TModel model)
        {
            this.Context.Set<TModel>().Remove(model);
            return await this.Context.SaveChangesAsync();
        }

        /// <summary>The update.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> Update([NotNull] TModel model)
        {
            this.Context.Set<TModel>().Update(model);
            return await this.Context.SaveChangesAsync();
        }

        /// <summary>The save changes.</summary>
        /// <returns>The <see cref="int" />.</returns>
        public int SaveChanges() => this.Context.SaveChanges();

        #endregion
    }
}