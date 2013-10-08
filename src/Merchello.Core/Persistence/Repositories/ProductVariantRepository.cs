using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class ProductVariantRepository : MerchelloPetaPocoRepositoryBase<Guid, IProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(IDatabaseUnitOfWork work) 
            : base(work)
        { }

        public ProductVariantRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache) 
            : base(work, cache)
        { }

        #region Overrides MerchelloPetaPocoRepositoryBase

        protected override IProductVariant PerformGet(Guid id)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<ProductDto, ProductVariantDto>(sql).FirstOrDefault();

            if (dto == null || dto.ProductVariantDto == null)
                return null;

            var factory = new ProductVariantFactory();
            var variant = factory.BuildEntity(dto.ProductVariantDto);

            // set the attributes collection
            ((ProductVariant)variant).ProductAttributes = GetProductAttributeCollection(variant.ProductKey);
            
            variant.ResetDirtyProperties();

            return variant;
        }

        protected override IEnumerable<IProductVariant> PerformGetAll(params Guid[] ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    yield return Get(id);
                }
            }
            else
            {                
                var dtos = Database.Fetch<ProductDto, ProductVariantDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.ProductVariantDto.Key);
                }
            }
        }

        protected override IEnumerable<IProductVariant> PerformGetByQuery(IQuery<IProductVariant> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProductVariant>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductDto, ProductVariantDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.ProductVariantDto.Key));

        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<ProductDto>()
                .InnerJoin<ProductVariantDto>()
                .On<ProductDto, ProductVariantDto>(left => left.Key, right => right.ProductKey)
                .Where<ProductVariantDto>(x => x.Master == false);

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchProductVariant.pk = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchInventory WHERE productVariantKey = @Id",
                "DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey = @Id",
                "DELETE FROM merchProductVariant WHERE pk = @Id"
            };

            return list;
        }
      
        protected override void PersistNewItem(IProductVariant entity)
        {
            if (!MandateProductVariantRules(entity)) return;

            Mandate.ParameterCondition(!SkuExists(entity.Sku), "The sku must be unique");

            ((Entity)entity).AddingEntity();

            var factory = new ProductVariantFactory();
            var dto = factory.BuildDto(entity);

            // insert the variant
            Database.Insert(dto);
            entity.Key = dto.Key;

            // insert associations for every attribute
            foreach (var association in entity.Attributes.Select(att => new ProductVariant2ProductAttributeDto()
            {
                ProductVariantKey = entity.Key,
                OptionId = att.OptionId,
                ProductAttributeId = att.Id,
                UpdateDate = DateTime.Now,
                CreateDate = DateTime.Now
            }))
            {
                Database.Insert(association);
            }

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IProductVariant entity)
        {
            if (!MandateProductVariantRules(entity)) return;

            Mandate.ParameterCondition(!SkuExists(entity.Sku, entity.Key), "Entity cannot be updated.  The sku already exists.");

            ((Entity)entity).UpdatingEntity();

            var factory = new ProductVariantFactory();
            var dto = factory.BuildDto(entity);

            // update the variant
            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IProductVariant entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Key });
            }
        }

        #endregion

        private static bool MandateProductVariantRules(IProductVariant entity)
        {
            // TODO these checks can probably be moved somewhere else but are here at the moment to enforce the rules as the API develops
            Mandate.ParameterCondition(entity.ProductKey != Guid.Empty, "productKey must be set");

            if (!((ProductVariant)entity).Master)
                Mandate.ParameterCondition(entity.Attributes.Any(), "Product variant must have attributes");            

            return true;
        }

        private ProductAttributeCollection GetProductAttributeCollection(Guid productVariantKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductVariant2ProductAttributeDto>()
                .InnerJoin<ProductAttributeDto>()
                .On<ProductVariant2ProductAttributeDto, ProductAttributeDto>(left => left.ProductAttributeId, right => right.Id)
                .Where<ProductVariant2ProductAttributeDto>(x => x.ProductVariantKey == productVariantKey);

            var dtos = Database.Fetch<ProductVariant2ProductAttributeDto, ProductAttributeDto>(sql);

            var factory = new ProductAttributeFactory();
            var collection = new ProductAttributeCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto.ProductAttributeDto));
            }
            return collection;
        }

        /// <summary>
        /// Compares the <see cref="ProductAttributeCollection"/> with other <see cref="IProductVariant"/>s of the <see cref="IProduct"/> pass
        /// to determine if the a variant already exists with the attributes passed
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to reference</param>
        /// <param name="attributes"><see cref="ProductAttributeCollection"/> to compare</param>
        /// <returns>True/false indicating whether or not a <see cref="IProductVariant"/> already exists with the <see cref="ProductAttributeCollection"/> passed</returns>
        public bool ProductVariantWithAttributesExists(IProduct product, ProductAttributeCollection attributes)
        {
            var variants = GetByProductKey(product.Key);
            return variants.Any(x => x.Attributes.Equals(attributes));
        }

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> object for a given Product Key
        /// </summary>
        /// <param name="productKey">Guid product key of the <see cref="IProductVariant"/> collection to retrieve</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        public IEnumerable<IProductVariant> GetByProductKey(Guid productKey)
        {
            var query = Querying.Query<IProductVariant>.Builder.Where(x => x.ProductKey == productKey && ((ProductVariant)x).Master == false);            
            return GetByQuery(query);
        }

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        public bool SkuExists(string sku)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductVariantDto>()
               .Where<ProductVariantDto>(x => x.Sku == sku);

            return Database.Fetch<ProductVariantDto>(sql).Any();

        }

        /// <summary>
        /// True/false indicating whether or not a sku exists on a record other than the record with the id passed
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <param name="productVariantKey">The key of the <see cref="IProductVariant"/> to be excluded</param>
        /// <returns></returns>
        private bool SkuExists(string sku, Guid productVariantKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductVariantDto>()
                .Where<ProductVariantDto>(x => x.Sku == sku && x.Key != productVariantKey);

            return Database.Fetch<ProductAttributeDto>(sql).Any();
        }

    }
}