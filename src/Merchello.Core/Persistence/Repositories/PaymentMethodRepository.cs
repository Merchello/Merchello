namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
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
    /// Represents the PaymentMethodRepository
    /// </summary>
    internal class PaymentMethodRepository : MerchelloPetaPocoRepositoryBase<IPaymentMethod>, IPaymentMethodRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public PaymentMethodRepository(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {
        }

        /// <summary>
        /// Gets a <see cref="IPaymentMethod"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentMethod"/>.
        /// </returns>
        protected override IPaymentMethod PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<PaymentMethodDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new PaymentMethodFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets a collection of all <see cref="IPaymentMethod"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IPaymentMethod}"/>.
        /// </returns>
        protected override IEnumerable<IPaymentMethod> PerformGetAll(params Guid[] keys)
        {
            var dtos = new List<PaymentMethodDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<PaymentMethodDto>(GetBaseQuery(false).WhereIn<PaymentMethodDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<PaymentMethodDto>(GetBaseQuery(false));
            }

            var factory = new PaymentMethodFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IPaymentMethod"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IPaymentMethod}"/>.
        /// </returns>
        protected override IEnumerable<IPaymentMethod> PerformGetByQuery(IQuery<IPaymentMethod> query)
        {

            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IPaymentMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<PaymentMethodDto>(sql).ToList();

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base query.
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
                .From<PaymentMethodDto>(SqlSyntax);

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
            return "merchPaymentMethod.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchPaymentMethod WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IPaymentMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new PaymentMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Saves an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IPaymentMethod entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new PaymentMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Deletes an existing item from the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IPaymentMethod entity)
        {
            // TODO clear cache for Payment
            Database.Execute("UPDATE merchPayment SET paymentMethodKey = NULL WHERE paymentMethodKey = @Key", new {entity.Key});

            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new {entity.Key });
            }
        }
    }
}