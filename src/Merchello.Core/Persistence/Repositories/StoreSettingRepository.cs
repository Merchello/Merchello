namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Factories;
    using Models;
    using Models.EntityBase;
    using Models.Rdbms;
    using Models.TypeFields;
    using Querying;    
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using UnitOfWork;

    /// <summary>
    /// Represents the Store Settings Repository
    /// </summary>
    internal class StoreSettingRepository :  MerchelloPetaPocoRepositoryBase<IStoreSetting>, IStoreSettingRepository
    {
        public StoreSettingRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {            
        }

        protected override IStoreSetting PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<StoreSettingDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new StoreSettingFactory();

            var setting = factory.BuildEntity(dto);

            return setting;
        }

        protected override IEnumerable<IStoreSetting> PerformGetAll(params Guid[] keys)
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
                var factory = new StoreSettingFactory();
                var dtos = Database.Fetch<StoreSettingDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IStoreSetting> PerformGetByQuery(IQuery<IStoreSetting> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IStoreSetting>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<StoreSettingDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<StoreSettingDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchStoreSetting.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchStoreSetting WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IStoreSetting entity)
        {

            ((Entity)entity).AddingEntity();

            var factory = new StoreSettingFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IStoreSetting entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new StoreSettingFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Gets the next invoice number (int)
        /// </summary>
        /// <param name="storeSettingKey">Constant Guid Key of the NextInvoiceNumber store setting</param>
        /// <param name="invoicesCount">The number of invoices needing invoice numbers.  Useful when saving multiple new invoices.</param>
        /// <returns></returns>
        public int GetNextInvoiceNumber(Guid storeSettingKey, Func<int> validate, int invoicesCount = 1)
        {
            Mandate.ParameterCondition(1 >= invoicesCount, "invoicesCount");

            var setting = Get(storeSettingKey);
            var nextInvoiceNumber = int.Parse(setting.Value);
            var max = validate();
            nextInvoiceNumber = nextInvoiceNumber >= max ? nextInvoiceNumber : max + 5; 
            var invoiceNumber = nextInvoiceNumber + invoicesCount;

            setting.Value = invoiceNumber.ToString(CultureInfo.InvariantCulture);

            AddOrUpdate(setting); // this will deal with the cache as well

            return invoiceNumber;
        }

        /// <summary>
        /// Gets the next order number (int)
        /// </summary>
        /// <param name="storeSettingKey">Constant Guid Key of the NextOrderNumber store setting</param>
        /// <param name="ordersCount">The number of orders needing order numbers.  Useful when saving multiple new orders</param>
        /// <returns></returns>
        public int GetNextOrderNumber(Guid storeSettingKey, Func<int> validate, int ordersCount = 1)
        {
            Mandate.ParameterCondition(1 >= ordersCount, "ordersCount");

            var setting = Get(storeSettingKey);
            var max = validate();
            var nextOrderNumber = int.Parse(setting.Value);
            nextOrderNumber = nextOrderNumber >= max ? nextOrderNumber : max + 5;
            var orderNumber = nextOrderNumber + ordersCount;

            setting.Value = orderNumber.ToString(CultureInfo.InvariantCulture);

            AddOrUpdate(setting);

            return orderNumber;
        }

        /// <summary>
        /// The get type fields.
        /// </summary>
        /// <returns>
        /// The collection of all type fields.
        /// </returns>
        public IEnumerable<ITypeField> GetTypeFields()
        {
            var dtos = Database.Fetch<TypeFieldDto>("SELECT * FROM merchTypeField");
            return dtos.Select(dto => new TypeField(dto.Alias, dto.Name, dto.Key));
        }
    }
}