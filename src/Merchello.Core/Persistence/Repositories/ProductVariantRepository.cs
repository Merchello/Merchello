namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

    /// <summary>
    /// The product variant repository.
    /// </summary>
    internal class ProductVariantRepository : MerchelloPetaPocoRepositoryBase<IProductVariant>, IProductVariantRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public ProductVariantRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {            
        }


        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> objects associated with a given warehouse 
        /// </summary>
        /// <param name="warehouseKey">The 'unique' key of the warehouse</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        public IEnumerable<IProductVariant> GetByWarehouseKey(Guid warehouseKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<CatalogInventoryDto>()
                .InnerJoin<WarehouseCatalogDto>()
                .On<CatalogInventoryDto, WarehouseCatalogDto>(left => left.CatalogKey, right => right.Key)
                .Where<WarehouseCatalogDto>(x => x.WarehouseKey == warehouseKey);

            var dtos = Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(sql);

            return dtos.DistinctBy(dto => dto.ProductVariantKey).Select(dto => Get(dto.ProductVariantKey));

        }

        /// <summary>
        /// Returns <see cref="IProductVariant"/> given the product and the collection of attribute ids that defines the<see cref="IProductVariant"/>
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="attributeKeys">
        /// The attribute Keys.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        public IProductVariant GetProductVariantWithAttributes(IProduct product, Guid[] attributeKeys)
        {
            var variants = GetByProductKey(product.Key);
            return variants.FirstOrDefault(x => x.Attributes.Count() == attributeKeys.Count() && attributeKeys.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));
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
        /// <param name="productKey">GUID product key of the <see cref="IProductVariant"/> collection to retrieve</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        public IEnumerable<IProductVariant> GetByProductKey(Guid productKey)
        {
            var query = Querying.Query<IProductVariant>.Builder.Where(x => x.ProductKey == productKey && ((ProductVariant)x).Master == false);
            return GetByQuery(query);
        }

        /// <summary>
        /// True/false indicating whether or not a SKU is already exists in the database
        /// </summary>
        /// <param name="sku">The SKU to be tested</param>
        /// <returns>
        /// A value indicating whether or not a SKU is already exists in the database
        /// </returns>
        public bool SkuExists(string sku)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductVariantDto>()
               .Where<ProductVariantDto>(x => x.Sku == sku);

            return Database.Fetch<ProductVariantDto>(sql).Any();
        }

        /// <summary>
        /// The get product attribute collection.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeCollection"/>.
        /// </returns>
        internal ProductAttributeCollection GetProductAttributeCollection(Guid productVariantKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductVariant2ProductAttributeDto>()
                .InnerJoin<ProductAttributeDto>()
                .On<ProductVariant2ProductAttributeDto, ProductAttributeDto>(left => left.ProductAttributeKey, right => right.Key)
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
        /// Gets the <see cref="DetachedContentCollection{IProductVariantDetachedContent}"/> for the collection.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="DetachedContentCollection{IProductVariantDetachedContent}"/>.
        /// </returns>
        internal DetachedContentCollection<IProductVariantDetachedContent> GetDetachedContentCollection(Guid productVariantKey)
        {
            var contents = this.GetProductVariantDetachedContents(productVariantKey);
            return new DetachedContentCollection<IProductVariantDetachedContent> { contents };
        }

        /// <summary>
        /// Safely saves the detached content selection.
        /// </summary>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
        internal void SaveDetachedContents(IProductVariant productVariant)
        {
            var existing = this.GetDetachedContentCollection(productVariant.Key).ToArray();

            if (!productVariant.DetachedContents.Any() && !existing.Any()) return;

            if (!productVariant.DetachedContents.Any())
            {
                foreach (var dc in existing)
                {
                    this.DeleteDetachedContent(dc);
                }

                return;
            }

            foreach (var exist in existing.Where(x => !productVariant.DetachedContents.Contains(x.CultureName)))
            {
                this.DeleteDetachedContent(exist);
            }

            foreach (var dc in productVariant.DetachedContents)
            {
                var slug = PathHelper.ConvertToSlug(productVariant.Name);
                this.SaveDetachedContent(dc, slug);
            }
        }

        /// <summary>
        /// The delete detached content.
        /// </summary>
        /// <param name="detachedContent">
        /// The detached content.
        /// </param>
        internal void DeleteDetachedContent(IProductVariantDetachedContent detachedContent)
        {
            Database.Execute(
                "DELETE [merchProductVariantDetachedContent] WHERE pk = @Key",
                new { @Key = detachedContent.Key });
        }

        /// <summary>
        /// The save detached content.
        /// </summary>
        /// <param name="detachedContent">
        /// The detached content.
        /// </param>
        /// <param name="slug">
        /// The generated slug
        /// </param>
        internal void SaveDetachedContent(IProductVariantDetachedContent detachedContent, string slug)
        {
            var factory = new ProductVariantDetachedContentFactory();

            if (!detachedContent.HasIdentity)
            {
                ((Entity)detachedContent).AddingEntity();
                detachedContent.Slug = this.EnsureSlug(detachedContent, slug);
                var dto = factory.BuildDto(detachedContent);
                Database.Insert(dto);
                detachedContent.Key = dto.Key;
            }
            else
            {
                ((Entity)detachedContent).UpdatingEntity();
                detachedContent.Slug = this.EnsureSlug(detachedContent, detachedContent.Slug);
                var dto = factory.BuildDto(detachedContent);

                const string Update =
                    "UPDATE [merchProductVariantDetachedContent] SET [merchProductVariantDetachedContent].[detachedContentTypeKey] = @Dctk, [merchProductVariantDetachedContent].[templateId] = @Tid, [merchProductVariantDetachedContent].[slug] = @Slug, [merchProductVariantDetachedContent].[values] = @Vals, [merchProductVariantDetachedContent].[canBeRendered] = @Cbr, [merchProductVariantDetachedContent].[updateDate] = @Ud WHERE [merchProductVariantDetachedContent].[cultureName] = @Cn AND [merchProductVariantDetachedContent].[productVariantKey] = @Pvk";


                Database.Execute(
                    Update,
                    new
                        {
                            @Dctk = dto.DetachedContentTypeKey,
                            @Tid = dto.TemplateId,
                            @Slug = dto.Slug,
                            @Vals = dto.Values,
                            @Cbr = dto.CanBeRendered,
                            @Ud = dto.UpdateDate,
                            @Cn = dto.CultureName,
                            @Pvk = dto.ProductVariantKey
                        });
            }
        }

        /// <summary>
        /// Saves the catalog inventory.
        /// </summary>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
        /// <remarks>
        /// This merely asserts that an association between the warehouse and the variant has been made
        /// </remarks>
        internal void SaveCatalogInventory(IProductVariant productVariant)
        {
            var existing = GetCategoryInventoryCollection(productVariant.Key);

            foreach (var inv in existing.Where(inv => !((ProductVariant)productVariant).CatalogInventoryCollection.Contains(inv.CatalogKey)))
            {
                DeleteCatalogInventory(productVariant.Key, inv.CatalogKey);
            }

            foreach (var inv in productVariant.CatalogInventories.Where(inv => !existing.Contains(inv.CatalogKey)))
            {
                AddCatalogInventory(productVariant, inv);
            }

            foreach (var inv in productVariant.CatalogInventories.Where(x => existing.Contains(x.CatalogKey)))
            {
                UpdateCatalogInventory(inv);
            }
        }

        /// <summary>
        /// Gets the category inventory collection.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogInventoryCollection"/>.
        /// </returns>
        internal CatalogInventoryCollection GetCategoryInventoryCollection(Guid productVariantKey)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<CatalogInventoryDto>()
               .InnerJoin<WarehouseCatalogDto>()
               .On<CatalogInventoryDto, WarehouseCatalogDto>(left => left.CatalogKey, right => right.Key)
               .Where<CatalogInventoryDto>(x => x.ProductVariantKey == productVariantKey);

            var dtos = Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(sql);

            var collection = new CatalogInventoryCollection();
            var factory = new CatalogInventoryFactory();

            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }

        /// <summary>
        /// Gets detached content associated with the product variant.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariantDetachedContent}"/>.
        /// </returns>
        internal IEnumerable<IProductVariantDetachedContent> GetProductVariantDetachedContents(Guid productVariantKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductVariantDetachedContentDto>()
                .InnerJoin<DetachedContentTypeDto>()
                .On<ProductVariantDetachedContentDto, DetachedContentTypeDto>(
                    left => left.DetachedContentTypeKey,
                    right => right.Key)
                .Where(
                    "[merchProductVariantDetachedContent].[productVariantKey] = @Key",
                    new { @Key = productVariantKey });

            var dtos = Database.Fetch<ProductVariantDetachedContentDto, DetachedContentTypeDto>(sql);

            var factory = new ProductVariantDetachedContentFactory();

            return dtos.Select(factory.BuildEntity);
        }

        /// <summary>
        /// Gets the product variant.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        protected override IProductVariant PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(sql).FirstOrDefault();

            if (dto == null || dto.ProductVariantDto == null)
                return null;

            var factory = new ProductVariantFactory(GetProductAttributeCollection, GetCategoryInventoryCollection, GetDetachedContentCollection);
            var variant = factory.BuildEntity(dto.ProductVariantDto);

            variant.ResetDirtyProperties();

            return variant;
        }

        /// <summary>
        /// The perform get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariant}"/>.
        /// </returns>
        protected override IEnumerable<IProductVariant> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    yield return Get(key);
                }
            }
            else
            {                
                var dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.ProductVariantDto.Key);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariant}"/>.
        /// </returns>
        protected override IEnumerable<IProductVariant> PerformGetByQuery(IQuery<IProductVariant> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProductVariant>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(sql);

            return dtos.DistinctBy(x => x.ProductVariantDto.Key).Select(dto => Get(dto.ProductVariantDto.Key));
        }

        /// <summary>
        /// Gets the base SQL clause.
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
                .From<ProductDto>()
                .InnerJoin<ProductVariantDto>()
                .On<ProductDto, ProductVariantDto>(left => left.Key, right => right.ProductKey)
                .InnerJoin<ProductVariantIndexDto>()
                .On<ProductVariantDto, ProductVariantIndexDto>(left => left.Key, right => right.ProductVariantKey);

            return sql;
        }

        /// <summary>
        /// Gets the base SQL where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchProductVariant.pk = @Key";
        }

        /// <summary>
        /// Gets the delete clauses that need to be executed when a variant is to be deleted
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchCatalogInventory WHERE productVariantKey = @Key",
                "DELETE FROM merchProductVariantDetachedContent WHERE productVariantKey = @Key",
                "DELETE FROM merchProductVariantIndex WHERE productVariantKey = @Key",
                "DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey = @Key",
                "DELETE FROM merchProductVariant WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IProductVariant entity)
        {
            if (!MandateProductVariantRules(entity)) return;

            Mandate.ParameterCondition(!SkuExists(entity.Sku), "The sku must be unique");

            ((Entity)entity).AddingEntity();

            ((ProductVariant)entity).VersionKey = Guid.NewGuid();

            var factory = new ProductVariantFactory(
                pa => ((ProductVariant)entity).ProductAttributes,
                ci => ((ProductVariant)entity).CatalogInventoryCollection,
                dc => ((ProductVariant)entity).DetachedContents);

            var dto = factory.BuildDto(entity);

            // insert the variant
            Database.Insert(dto);
            entity.Key = dto.Key; // to set HasIdentity

            Database.Insert(dto.ProductVariantIndexDto);
            ((ProductVariant) entity).ExamineId = dto.ProductVariantIndexDto.Id;

            // insert associations for every attribute
            foreach (var association in entity.Attributes.Select(att => new ProductVariant2ProductAttributeDto()
            {
                ProductVariantKey = entity.Key,
                OptionKey = att.OptionKey,
                ProductAttributeKey = att.Key,
                UpdateDate = DateTime.Now,
                CreateDate = DateTime.Now
            }))
            {
                Database.Insert(association);
            }

            SaveCatalogInventory(entity);

            SaveDetachedContents(entity);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProduct>(entity.ProductKey));
        }

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IProductVariant entity)
        {
            if (!MandateProductVariantRules(entity)) return;

            Mandate.ParameterCondition(!SkuExists(entity.Sku, entity.Key), "Entity cannot be updated.  The sku already exists.");

            ((Entity)entity).UpdatingEntity();
            ((ProductVariant)entity).VersionKey = Guid.NewGuid();

            var factory = new ProductVariantFactory(
                pa => ((ProductVariant)entity).ProductAttributes,
                ci => ((ProductVariant)entity).CatalogInventoryCollection,
                dc => ((ProductVariant)entity).DetachedContents);

            var dto = factory.BuildDto(entity);

            // update the variant
            Database.Update(dto);

            SaveCatalogInventory(entity);

            SaveDetachedContents(entity);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProduct>(entity.ProductKey));
        }

        /// <summary>
        /// The persist deleted item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IProductVariant entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { entity.Key });
            }

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProduct>(entity.ProductKey));
        }

        /// <summary>
        /// The mandate product variant rules.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool MandateProductVariantRules(IProductVariant entity)
        {
            // TODO these checks can probably be moved somewhere else but are here at the moment to enforce the rules as the API develops
            Mandate.ParameterCondition(entity.ProductKey != Guid.Empty, "productKey must be set");

            if (!((ProductVariant)entity).Master)
                Mandate.ParameterCondition(entity.Attributes.Any(), "Product variant must have attributes");            

            return true;
        }

        /// <summary>
        /// Associates a <see cref="IProductVariant"/> with a catalog inventory.
        /// </summary>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
        /// <param name="inv">
        /// The <see cref="ICatalogInventory"/>.
        /// </param>
        private void AddCatalogInventory(IProductVariant productVariant, ICatalogInventory inv)
        {
            inv.CreateDate = DateTime.Now;
            inv.UpdateDate = DateTime.Now;

            var dto = new CatalogInventoryDto()
            {
                CatalogKey = inv.CatalogKey,
                ProductVariantKey = productVariant.Key,
                Count = inv.Count,
                LowCount = inv.LowCount,
                Location = inv.Location,
                CreateDate = inv.CreateDate,
                UpdateDate = inv.UpdateDate
            };

            Database.Insert(dto);
        }

        /// <summary>
        /// Updates catalog inventory.
        /// </summary>
        /// <param name="inv">
        /// The <see cref="ICatalogInventory"/>.
        /// </param>
        private void UpdateCatalogInventory(ICatalogInventory inv)
        {
            inv.UpdateDate = DateTime.Now;


            Database.Execute(
                "UPDATE merchCatalogInventory SET Count = @invCount, LowCount = @invLowCount, Location = @invLocation, UpdateDate = @invUpdateDate WHERE catalogKey = @catalogKey AND productVariantKey = @productVariantKey",
                new
                {
                    invCount = inv.Count,
                    invLowCount = inv.LowCount,
                    invLocation = inv.Location,
                    invUpdateDate = inv.UpdateDate,
                    catalogKey = inv.CatalogKey,
                    productVariantKey = inv.ProductVariantKey                    
                });
        }

        /// <summary>
        /// Deletes catalog inventory.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <param name="catalogKey">
        /// The catalog key.
        /// </param>
        private void DeleteCatalogInventory(Guid productVariantKey, Guid catalogKey)
        {
            const string Sql = "DELETE FROM merchCatalogInventory WHERE productVariantKey = @pvKey AND catalogKey = @cKey";

            Database.Execute(Sql, new { @pvKey = productVariantKey, @cKey = catalogKey });
        }

        /// <summary>
        /// True/false indicating whether or not a SKU exists on a record other than the record with the id passed
        /// </summary>
        /// <param name="sku">The SKU to be tested</param>
        /// <param name="productVariantKey">The key of the <see cref="IProductVariant"/> to be excluded</param>
        /// <returns>A value indicating whether or not a SKU exists on a record other than the record with the id passed</returns>
        private bool SkuExists(string sku, Guid productVariantKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductVariantDto>()
                .Where<ProductVariantDto>(x => x.Sku == sku && x.Key != productVariantKey);

            return Database.Fetch<ProductAttributeDto>(sql).Any();
        }

        /// <summary>
        /// Ensures the slug is valid.
        /// </summary>
        /// <param name="detachedContent">
        /// The detached content.
        /// </param>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// A slug incremented with a count if necessary.
        /// </returns>
        private string EnsureSlug(IProductVariantDetachedContent detachedContent, string slug)
        {
            var count = Database.ExecuteScalar<int>("SELECT COUNT(slug) FROM [merchProductVariantDetachedContent] WHERE [merchProductVariantDetachedContent].[slug] = @Slug AND [merchProductVariantDetachedContent].[productVariantKey] != @Pvk", new { @Slug = slug, @Pvk = detachedContent.ProductVariantKey });
            if (count > 0) slug = string.Format("{0}-{1}", slug, count + 1);
            return slug;
        }

    }
}