namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The digital media repository.
    /// </summary>
    internal class DigitalMediaRepository : MerchelloPetaPocoRepositoryBase<IDigitalMedia>, IDigitalMediaRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public DigitalMediaRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
        }

        /// <summary>
        /// Gets a <see cref="IDigitalMedia"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        protected override IDigitalMedia PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
           .Where(GetBaseWhereClause(), new { Key = key });


            var dto = Database.Fetch<DigitalMediaDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new DigitalMediaFactory();

            var digitalMedia = factory.BuildEntity(dto);

            return digitalMedia;
        }

        /// <summary>
        /// Returns a collection of all digital media records.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDigitalMedia}"/>.
        /// </returns>
        protected override IEnumerable<IDigitalMedia> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var id in keys)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new DigitalMediaFactory();
                var dtos = Database.Fetch<DigitalMediaDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IDigitalMedia"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDigitalMedia}"/>.
        /// </returns>
        protected override IEnumerable<IDigitalMedia> PerformGetByQuery(IQuery<IDigitalMedia> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IDigitalMedia>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<DigitalMediaDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base SQL query.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<DigitalMediaDto>(SqlSyntax);

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchDigitalMedia.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses to be executed on a delete operation.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchDigitalMedia WHERE pk = @Key",
                };

            return list;
        }

        /// <summary>
        /// Saves a new <see cref="IDigitalMedia"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IDigitalMedia entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new DigitalMediaFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates a <see cref="IDigitalMedia"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IDigitalMedia entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new DigitalMediaFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}