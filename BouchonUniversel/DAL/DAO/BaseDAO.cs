namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using BouchonUniversel.Models;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;

    #endregion

    /// <summary>The totem dao.</summary>
    /// <typeparam name="TContext">Type du contexte.</typeparam>
    /// <typeparam name="TModel">Type du model.</typeparam>
    /// <typeparam name="TIdentity">Type de la clé primaire du model.</typeparam>
    [PublicAPI]
    public abstract class BaseDAO<TContext, TModel, TIdentity>
        where TModel : class, IDto<TIdentity>
        where TIdentity : IComparable<TIdentity>
        where TContext : DbContext
    {
        /// <summary>Gets or sets the context.</summary>
        private readonly TContext context;

        /// <summary>Initializes a new instance of the <see cref="BaseDAO{TContext,TModel,TIdentity}" /> class.</summary>
        /// <param name="context">The context.</param>
        protected BaseDAO([NotNull] TContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.Querable = this.context.Set<TModel>().AsQueryable();
        }

        /// <summary>Gets the entities.</summary>
        protected IQueryable<TModel> Querable { get; }

        /// <summary>The create.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<int> CreateAsync([NotNull] TModel model)
        {
            this.context.Set<TModel>().Add(model);
            return this.context.SaveChangesAsync();
        }

        /// <summary>The exists.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool Exists(TIdentity id)
            => this.context.Set<TModel>().Any(e => e.Id.CompareTo(id) == 0);

        /// <summary>The get all.</summary>
        /// <param name="includes">The includes.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<List<TModel>> GetAllAsync([NotNull] params Expression<Func<TModel, object>>[] includes)
        {
            var set = this.context.Set<TModel>();
            var includeSet = includes.Aggregate<Expression<Func<TModel, object>>, IIncludableQueryable<TModel, object>>(
                null,
                (current, include) => current == null ? set.Include(include) : current.Include(include));
            return includeSet == null ? await set.ToListAsync().ConfigureAwait(false) : await includeSet.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>The get all.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<TModel>> GetAllAsync()
            => this.context.Set<TModel>().ToListAsync();

        /// <summary>The get details.</summary>
        /// <param name="id">The id.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<TModel> GetDetailsAsync(TIdentity id, [NotNull] params Expression<Func<TModel, object>>[] includes)
        {
            var set = this.context.Set<TModel>();
            var includeSet = includes.Aggregate<Expression<Func<TModel, object>>, IIncludableQueryable<TModel, object>>(
                                                                                                                        null,
                                                                                                                        (current, include) => current == null ? set.Include(include) : current.Include(include));
            return includeSet == null
                       ? await set.SingleOrDefaultAsync(model => model.Id.CompareTo(id) == 0).ConfigureAwait(false)
                       : await includeSet.SingleOrDefaultAsync(model => model.Id.CompareTo(id) == 0).ConfigureAwait(false);
        }

        /// <summary>The get details.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<TModel> GetDetailsAsync(TIdentity id)
            => this.context.Set<TModel>().SingleOrDefaultAsync(model => model.Id.CompareTo(id) == 0);

        /// <summary>The remove.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<int> RemoveAsync([NotNull] TModel model)
        {
            this.context.Set<TModel>().Remove(model);
            return this.context.SaveChangesAsync();
        }

        /// <summary>The update.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<int> UpdateAsync([NotNull] TModel model)
        {
            this.context.Set<TModel>().Update(model);
            return this.context.SaveChangesAsync();
        }

        /// <summary>The save changes.</summary>
        /// <returns>The <see cref="int" />.</returns>
        protected int SaveChanges()
            => this.context.SaveChanges();
    }
}
