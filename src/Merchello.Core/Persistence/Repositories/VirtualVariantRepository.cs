﻿namespace Merchello.Core.Persistence.Repositories
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
    internal class VirtualVariantRepository : MerchelloPetaPocoRepositoryBase<IVirtualVariant>, IVirtualVariantRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualVariantRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public VirtualVariantRepository(IDatabaseUnitOfWork work, CacheHelper cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
        }

        /// <summary>
        /// Gets a <see cref="IVirtualVariant"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IVirtualVariant"/>.
        /// </returns>
        protected override IVirtualVariant PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
           .Where(GetBaseWhereClause(), new { Key = key });


            var dto = Database.Fetch<VirtualVariantsDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new VirtualVariantFactory();

            var virtualVariant = factory.BuildEntity(dto);

            return virtualVariant;
        }

        public IVirtualVariant GetBySku(string sku)
        {
            var sql = GetBaseQuery(false).Where("merchVirtualVariants.sku = @Sku", new { Sku = sku });

            var dto = Database.Fetch<VirtualVariantsDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new VirtualVariantFactory();

            var virtualVariant = factory.BuildEntity(dto);

            return virtualVariant;
        }

        /// <summary>
        /// Returns a collection of all virtual variant records.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IVirtualVariant}"/>.
        /// </returns>
        protected override IEnumerable<IVirtualVariant> PerformGetAll(params Guid[] keys)
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
                var factory = new VirtualVariantFactory();
                var dtos = Database.Fetch<VirtualVariantsDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IVirtualVariant"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IVirtualVariant}"/>.
        /// </returns>
        protected override IEnumerable<IVirtualVariant> PerformGetByQuery(IQuery<IVirtualVariant> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IVirtualVariant>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<VirtualVariantsDto>(sql);

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
               .From<VirtualVariantsDto>(SqlSyntax);

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
            return "merchVirtualVariants.pk = @Key";
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
                    "DELETE FROM merchVirtualVariants WHERE pk = @Key",
                };

            return list;
        }

        /// <summary>
        /// Saves a new <see cref="IVirtualVariant"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IVirtualVariant entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new VirtualVariantFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates a <see cref="IVirtualVariant"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IVirtualVariant entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new VirtualVariantFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IVirtualVariant>(entity.Key));
        }
    }
}