namespace BouchonUniversel.DAL
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

    using Models.Bouchons;

    #endregion

    /// <summary>The bouchons dao.</summary>
    public class BouchonsDAO
    {
        #region Champs

        /// <summary>The context.</summary>
        private readonly DataContext context;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonsDAO"/> class. Initializes a new instance of the<see cref="T:System.Object"></see> class.</summary>
        /// <param name="context">The context.</param>
        public BouchonsDAO(DataContext context) => this.context = context;

        #endregion

        #region Méthodes publiques

        /// <summary>The create.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<int> Create([NotNull] Bouchon model)
        {
            this.context.Set<Bouchon>().Add(model);
            return await this.context.SaveChangesAsync();
        }

        /// <summary>The exists.</summary>
        /// <param name="baseUrl">The base Url.</param>
        /// <param name="serviceUrl">The service Url.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Exists(string baseUrl, string serviceUrl) => this.context.Bouchons.Any(PredicateIdentity(baseUrl, serviceUrl));

        /// <summary>The get all.</summary>
        /// <param name="includes">The includes.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<Bouchon>> GetAll([NotNull] params Expression<Func<Bouchon, object>>[] includes)
        {
            var set = this.context.Set<Bouchon>();
            var includeSet = includes.Aggregate<Expression<Func<Bouchon, object>>, IIncludableQueryable<Bouchon, object>>(
                null, (current, include) => current == null ? set.Include(include) : current.Include(include));
            return includeSet == null ? await set.ToListAsync() : await includeSet.ToListAsync();
        }

        /// <summary>The get all.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<List<Bouchon>> GetAll() => await this.context.Set<Bouchon>().ToListAsync();

        /*public async Task<Bouchon> GetDetails(TIdentity id)
        {
            using (this.context = new Tcontext())
            {
                return this.context.Set<Bouchon>().Any(model => model.Id.CompareTo(id) == 0);
            }
        }*/

        /// <summary>The get details.</summary>
        /// <param name="baseUrl">The base Url.</param>
        /// <param name="serviceUrl">The service Url.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Bouchon> GetDetails(string baseUrl, string serviceUrl, [NotNull] params Expression<Func<Bouchon, object>>[] includes)
        {
            var set = this.context.Set<Bouchon>();
            var includeSet = includes.Aggregate<Expression<Func<Bouchon, object>>, IIncludableQueryable<Bouchon, object>>(
                null, (current, include) => current == null ? set.Include(include) : current.Include(include));
            return includeSet == null
                ? await set.SingleOrDefaultAsync(PredicateIdentity(baseUrl, serviceUrl))
                : await includeSet.SingleOrDefaultAsync(PredicateIdentity(baseUrl, serviceUrl));
        }

        /// <summary>The get details.</summary>
        /// <param name="baseUrl">The base Url.</param>
        /// <param name="serviceUrl">The service Url.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Bouchon> GetDetails(string baseUrl, string serviceUrl) => await this.context.Set<Bouchon>().SingleOrDefaultAsync(PredicateIdentity(baseUrl, serviceUrl));

        /// <summary>The remove.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> Remove([NotNull] Bouchon model)
        {
            this.context.Set<Bouchon>().Remove(model);
            return await this.context.SaveChangesAsync();
        }

        /// <summary>The update.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> Update([NotNull] Bouchon model)
        {
            this.context.Set<Bouchon>().Update(model);
            return await this.context.SaveChangesAsync();
        }

        #endregion

        #region Méthodes privées

        /// <summary>The predicate identity.</summary>
        /// <param name="baseUrl">The base url.</param>
        /// <param name="serviceUrl">The service url.</param>
        /// <returns>The <see cref="Expression"/>.</returns>
        private static Expression<Func<Bouchon, bool>> PredicateIdentity(string baseUrl, string serviceUrl)
        {
            return bouchon => bouchon.BaseUrl == baseUrl && bouchon.ServiceUrl == serviceUrl;
        }

        #endregion
    }
}