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
    public class ServicesDAO
    {
        #region Champs

        /// <summary>The context.</summary>
        private readonly DataContext context;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonsDAO"/> class. Initializes a new instance of the<see cref="T:System.Object"></see> class.</summary>
        /// <param name="context">The context.</param>
        public ServicesDAO(DataContext context) => this.context = context;

        #endregion

        #region Méthodes publiques

        /// <summary>The create.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<int> Create([NotNull] Service model)
        {
            this.context.Services.Add(model);
            return await this.context.SaveChangesAsync();
        }

        /// <summary>The exists.</summary>
        /// <param name="baseUrl">The base Url.</param>
        /// <param name="serviceUrl">The service Url.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Exists(string cle) => this.context.Services.Any(service => service.Cle == cle);

        /// <summary>The get all.</summary>
        /// <param name="includes">The includes.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<Service>> GetAll([NotNull] params Expression<Func<Service, object>>[] includes)
        {
            var set = this.context.Set<Service>();
            var includeSet = includes.Aggregate<Expression<Func<Service, object>>, IIncludableQueryable<Service, object>>(
                null, (current, include) => current == null ? set.Include(include) : current.Include(include));
            return includeSet == null ? await set.ToListAsync() : await includeSet.ToListAsync();
        }

        /// <summary>The get all.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<List<Service>> GetAll() => await this.context.Set<Service>().ToListAsync();

        /*public async Task<Service> GetDetails(TIdentity id)
        {
            using (this.context = new Tcontext())
            {
                return this.context.Set<Service>().Any(model => model.Id.CompareTo(id) == 0);
            }
        }*/

        /// <summary>The get details.</summary>
        /// <param name="baseUrl">The base Url.</param>
        /// <param name="serviceUrl">The service Url.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Service> GetDetails(string cle, [NotNull] params Expression<Func<Service, object>>[] includes)
        {
            var set = this.context.Set<Service>();
            var includeSet = includes.Aggregate<Expression<Func<Service, object>>, IIncludableQueryable<Service, object>>(
                null, (current, include) => current == null ? set.Include(include) : current.Include(include));
            return includeSet == null
                ? await set.SingleOrDefaultAsync(service => service.Cle == cle)
                : await includeSet.SingleOrDefaultAsync(service => service.Cle == cle);
        }

        /// <summary>The get details.</summary>
        /// <param name="baseUrl">The base Url.</param>
        /// <param name="serviceUrl">The service Url.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Service> GetDetails(string cle) => await this.context.Set<Service>().SingleOrDefaultAsync(service => service.Cle == cle);

        /// <summary>The remove.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> Remove([NotNull] Service model)
        {
            this.context.Set<Service>().Remove(model);
            return await this.context.SaveChangesAsync();
        }

        /// <summary>The update.</summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> Update([NotNull] Service model)
        {
            this.context.Set<Service>().Update(model);
            return await this.context.SaveChangesAsync();
        }

        #endregion
    }
}