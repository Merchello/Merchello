﻿namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
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
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The product variant repository.
    /// </summary>
    internal class ProductVariantRepository : MerchelloBulkOperationRepository<IProductVariant, ProductVariantDto>, IProductVariantRepository
    {
        /// <summary>
        /// Chunk size for SQL statements
        /// </summary>
        private const int ChunkSize = 400;

        /// <summary>
        /// The <see cref="IProductOptionRepository"/>.
        /// </summary>
        private readonly IProductOptionRepository _productOptionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        /// <param name="productOptionRepository">
        /// The <see cref="IProductOptionRepository"/>.
        /// </param>
        public ProductVariantRepository(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax, IProductOptionRepository productOptionRepository)
            : base(work, logger, sqlSyntax, () => new ProductVariantFactory())
        {
            Mandate.ParameterNotNull(productOptionRepository, "productOptionRepository");

            _productOptionRepository = productOptionRepository;
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
                .From<CatalogInventoryDto>(SqlSyntax)
                .InnerJoin<WarehouseCatalogDto>(SqlSyntax)
                .On<CatalogInventoryDto, WarehouseCatalogDto>(SqlSyntax, left => left.CatalogKey, right => right.Key)
                .Where<WarehouseCatalogDto>(x => x.WarehouseKey == warehouseKey, SqlSyntax);

            var dtos = Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(sql);

            return dtos.DistinctBy(dto => dto.ProductVariantKey).Select(dto => Get(dto.ProductVariantKey));
        }

        /// <summary>
        /// Returns <see cref="IProductVariant"/> given the product and the collection of attribute ids that defines the<see cref="IProductVariant"/>
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="variants">
        /// Variants to check against
        /// </param>
        /// <param name="attributeKeys">
        /// The attribute Keys.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        public IProductVariant GetProductVariantWithAttributes(IProduct product, List<IProductVariant> variants, Guid[] attributeKeys)
        {
            return variants.FirstOrDefault(x => x.Attributes.Count() == attributeKeys.Count() && attributeKeys.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));
        }

        /// <summary>
        /// Compares the <see cref="ProductAttributeCollection"/> with other <see cref="IProductVariant"/>s of the <see cref="IProduct"/> pass
        /// to determine if the a variant already exists with the attributes passed
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to reference</param>
        /// <param name="variants">
        /// Variants to check against
        /// </param>
        /// <param name="attributes"><see cref="ProductAttributeCollection"/> to compare</param>
        /// <returns>True/false indicating whether or not a <see cref="IProductVariant"/> already exists with the <see cref="ProductAttributeCollection"/> passed</returns>
        public bool ProductVariantWithAttributesExists(IProduct product, List<IProductVariant> variants, ProductAttributeCollection attributes)
        {
            var keys = attributes.Select(x => x.Key);
            return variants.Any(x => x.Attributes.All(z => keys.Contains(z.Key)));
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
        /// Gets the <see cref="ProductVariantCollection"/> for a given product.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantCollection"/>.
        /// </returns>
        public ProductVariantCollection GetProductVariantCollection(Guid productKey)
        {
            var collection = new ProductVariantCollection();
            var variants = GetByProductKey(productKey);

            foreach (var variant in variants.Where(variant => variant != null))
            {
                collection.Add(variant);
            }

            return collection;
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
               .From<ProductVariantDto>(SqlSyntax)
               .Where<ProductVariantDto>(x => x.Sku == sku, SqlSyntax);

            return Database.Fetch<ProductVariantDto>(sql).Any();
        }

        #region Bulk Operations


        /// <summary>
        /// The persist new items.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        public void PersistNewItems(IEnumerable<IProductVariant> entities)
        {
            var productVariants = entities as IProductVariant[] ?? entities.ToArray();

            if (!MandateProductVariantRules(productVariants)) return;

            //// Mandate.ParameterCondition(!SkuExists(entity.Sku), "The sku must be unique");
            var dtos = new List<ProductVariantDto>();
            foreach (var entity in productVariants)
            {
                ((ProductBase)entity).AddingEntity();
                var factory = new ProductVariantFactory();
                dtos.Add(factory.BuildDto(entity));
            }

            // insert the variants
            BulkInsertRecordsWithKey<ProductVariantDto>(dtos);
            BulkInsertRecordsWithKey<ProductVariantIndexDto>(dtos.Select(v => v.ProductVariantIndexDto));

            // Split keys into chunks  to stop SQL 2100 limit
            var keys = dtos.Select(x => x.Key).ToArray();

            // Chunk the keys
            var keysChunked = keys.Split(ChunkSize);

            // List of ProductVariantIndexDto's
            var idDtos = new List<ProductVariantIndexDto>();

            // Loop keys
            foreach (var pvkeys in keysChunked)
            {
                // We have to look up the examine ids and add them to the existing list
                idDtos.AddRange(Database.Fetch<ProductVariantIndexDto>("WHERE productVariantKey IN (@pvkeys)", new { pvkeys }));
            }

            foreach (var entity in productVariants)
            {
                var dto = dtos.FirstOrDefault(d => d.VersionKey == entity.VersionKey);
                // ReSharper disable once PossibleNullReferenceException
                entity.Key = dto.Key; // to set HasIdentity

                var productVariantIndexDto = idDtos.FirstOrDefault(id => id.ProductVariantKey == dto.Key);
                if (productVariantIndexDto != null)
                {
                    ((ProductVariant)entity).ExamineId = productVariantIndexDto.Id;
                }

                foreach (var inv in entity.CatalogInventories)
                {
                    ((CatalogInventory)inv).ProductVariantKey = entity.Key;
                }
            }

            var xrefs = new List<ProductVariant2ProductAttributeDto>();

            foreach (var v in productVariants.ToArray())
            {
                var associations = v.Attributes.Select(x =>
                    new ProductVariant2ProductAttributeDto
                    {
                        ProductVariantKey = v.Key,
                        OptionKey = x.OptionKey,
                        ProductAttributeKey = x.Key,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now
                    });

                xrefs.AddRange(associations);
            }

            BulkInsertRecordsWithKey<ProductVariant2ProductAttributeDto>(xrefs);

            SaveCatalogInventory(productVariants);

            SaveDetachedContents(productVariants);

            foreach (var entity in productVariants)
            {
                entity.ResetDirtyProperties();
            }
        }

        /// <summary>
        /// The persist updated items.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        public void PersistUpdatedItems(IEnumerable<IProductVariant> entities)
        {
            var productVariants = entities as IProductVariant[] ?? entities.ToArray();
            if (!MandateProductVariantRules(productVariants)) return;

            Mandate.ParameterCondition(!SkuExists(productVariants), "Entity cannot be updated.  The sku already exists.");

            ExecuteBatchUpdate(productVariants);

            SaveCatalogInventory(productVariants);

            SaveDetachedContents(productVariants);

            foreach (var entity in productVariants)
            {
                entity.ResetDirtyProperties();
            }
        }

        #endregion

        #region Object Collections

        /// <summary>
        /// Gets the <see cref="DetachedContentCollection{IProductVariantDetachedContent}"/> for the collection.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="DetachedContentCollection{IProductVariantDetachedContent}"/>.
        /// </returns>
        public DetachedContentCollection<IProductVariantDetachedContent> GetDetachedContentCollection(Guid productVariantKey)
        {
            var contents = this.GetProductVariantDetachedContents(productVariantKey).ToArray();
            return new DetachedContentCollection<IProductVariantDetachedContent> { contents };
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
        public CatalogInventoryCollection GetCategoryInventoryCollection(Guid productVariantKey)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<CatalogInventoryDto>(SqlSyntax)
               .InnerJoin<WarehouseCatalogDto>(SqlSyntax)
               .On<CatalogInventoryDto, WarehouseCatalogDto>(SqlSyntax, left => left.CatalogKey, right => right.Key)
               .Where<CatalogInventoryDto>(x => x.ProductVariantKey == productVariantKey, SqlSyntax);

            var dtos = Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(sql);

            var collection = new CatalogInventoryCollection();
            var factory = new CatalogInventoryFactory();

            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }

        #endregion

        /// <summary>
        /// Saves the catalog inventory.
        /// </summary>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
        /// <remarks>
        /// This merely asserts that an association between the warehouse and the variant has been made
        /// </remarks>
        public void SaveCatalogInventory(IProductVariant productVariant)
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
        /// The save catalog inventory.
        /// </summary>
        /// <param name="productVariants">
        /// The product variants.
        /// </param>
        /// <remarks>
        /// This merely asserts that an association between the warehouse and the variant has been made
        /// </remarks>
        internal void SaveCatalogInventory(IProductVariant[] productVariants)
        {
            // Get any variant keys
            var keys = productVariants.Select(v => v.Key).ToArray();

            // List to hold the result of the query
            var inventoryDtos = new List<CatalogInventoryDto>();

            // Check for variant keys
            if (keys.Any())
            {
                // Split keys into chunks to stop SQL 2100 limit
                var keysChunked = keys.Split(ChunkSize);

                // Loop keys and execute query
                foreach (var keyChunk in keysChunked)
                {
                    var sql = new Sql();
                    sql.Select("*")
                        .From<CatalogInventoryDto>(SqlSyntax)
                        .InnerJoin<WarehouseCatalogDto>(SqlSyntax)
                        .On<CatalogInventoryDto, WarehouseCatalogDto>(SqlSyntax, left => left.CatalogKey, right => right.Key)
                        .WhereIn<CatalogInventoryDto>(x => x.ProductVariantKey, keyChunk, SqlSyntax);

                    inventoryDtos.AddRange(Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(sql));
                }
            }
            else
            {
                // Not ideal as duplicate bits of query
                var sql = new Sql();
                sql.Select("*")
                    .From<CatalogInventoryDto>(SqlSyntax)
                    .InnerJoin<WarehouseCatalogDto>(SqlSyntax)
                    .On<CatalogInventoryDto, WarehouseCatalogDto>(SqlSyntax, left => left.CatalogKey, right => right.Key);

                inventoryDtos = Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(sql);
            }

            var isSqlCe = SqlSyntax is SqlCeSyntaxProvider;

            string sqlStatement = string.Empty;
            int paramIndex = 0;
            var parms = new List<object>();
            var inserts = new List<CatalogInventoryDto>();

            var variantIndex = 0;
            foreach (var productVariant in productVariants)
            {
                foreach (var dto in inventoryDtos.Where(i => i.ProductVariantKey == productVariant.Key))
                {
                    if (!((ProductVariant)productVariant).CatalogInventoryCollection.Contains(dto.CatalogKey))
                    {
                        if (isSqlCe)
                        {
                            SqlCeDeleteCatalogInventory(dto.ProductVariantKey, dto.CatalogKey);
                        }
                        else
                        {
                            sqlStatement += string.Format(" DELETE FROM merchCatalogInventory WHERE productVariantKey = @{0} AND catalogKey = @{1}", paramIndex++, paramIndex++);
                            parms.Add(dto.ProductVariantKey);
                            parms.Add(dto.CatalogKey);
                        }

                    }
                }

                foreach (var inv in productVariant.CatalogInventories)
                {
                    inv.UpdateDate = DateTime.Now;
                    if (inventoryDtos.Any(i => i.ProductVariantKey == productVariant.Key && i.CatalogKey == inv.CatalogKey))
                    {
                        if (isSqlCe)
                        {
                            SqlCeUpdateCatalogInventory(inv);
                        }
                        else
                        {
                            sqlStatement += string.Format(" UPDATE merchCatalogInventory SET Count = @{0}, LowCount = @{1}, Location = @{2}, UpdateDate = @{3} WHERE catalogKey = @{4} AND productVariantKey = @{5}", paramIndex++, paramIndex++, paramIndex++, paramIndex++, paramIndex++, paramIndex++);
                            parms.Add(inv.Count);
                            parms.Add(inv.LowCount);
                            parms.Add(inv.Location);
                            parms.Add(inv.UpdateDate);
                            parms.Add(inv.CatalogKey);
                            parms.Add(inv.ProductVariantKey);
                        }
                    }
                    else
                    {
                        inv.CreateDate = DateTime.Now;
                        inv.UpdateDate = DateTime.Now;
                        inserts.Add(new CatalogInventoryDto
                        {
                            CatalogKey = inv.CatalogKey,
                            ProductVariantKey = productVariant.Key,
                            Count = inv.Count,
                            LowCount = inv.LowCount,
                            Location = inv.Location,
                            CreateDate = inv.CreateDate,
                            UpdateDate = inv.UpdateDate
                        });
                    }
                }
                
                //split into batches of 100
                if (++variantIndex >= 100) {
                    if (!string.IsNullOrEmpty(sqlStatement))
                    {
                    		Database.Execute(sqlStatement, parms.ToArray());
                    }
                		variantIndex = 0;
                    sqlStatement = string.Empty;
                }
            }
            
            if (!string.IsNullOrEmpty(sqlStatement))
            {
                Database.Execute(sqlStatement, parms.ToArray());
            }

            if (inserts.Any())
            {

                // Database.BulkInsertRecords won't work here because of the many to many and no pk.
                foreach (var ins in inserts) Database.Insert(ins);
            }
        }

        /// <summary>
        /// Updates SQLCE database where bulk update is not available.
        /// </summary>
        /// <param name="inv">
        /// The inventory.
        /// </param>
        private void SqlCeUpdateCatalogInventory(ICatalogInventory inv)
        {
            var sql =
                new Sql(
                    "UPDATE merchCatalogInventory SET Count = @ct, LowCount = @lct, Location = @loc, UpdateDate = @ud WHERE catalogKey = @ck AND productVariantKey = @pvk",
                    new
                        {
                            @ct = inv.Count,
                            @lct = inv.LowCount,
                            @loc = inv.Location,
                            @ud = DateTime.Now,
                            @ck = inv.CatalogKey,
                            @pvk = inv.ProductVariantKey
                        });

            Database.Execute(sql);
        }

        /// <summary>
        /// Deletes catalog inventory where bulk operations are not available.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <param name="catalogKey">
        /// The catalog key.
        /// </param>
        private void SqlCeDeleteCatalogInventory(Guid productVariantKey, Guid catalogKey)
        {
            var sql = new Sql("DELETE FROM merchCatalogInventory WHERE productVariantKey = @pvk AND catalogKey = @ck", new { @pvk = productVariantKey, @ck = catalogKey });
            Database.Execute(sql);
        }

        #region DetachedContent


        /// <summary>
        /// Safely saves the detached content selection.
        /// </summary>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
        public void SaveDetachedContents(IProductVariant productVariant)
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
        /// Bulk save detached contents.
        /// </summary>
        /// <param name="productVariants">
        /// The product variants.
        /// </param>
        internal void SaveDetachedContents(IEnumerable<IProductVariant> productVariants)
        {
            var variants = productVariants as IProductVariant[] ?? productVariants.ToArray();
            var factory = new ProductVariantDetachedContentFactory();

            // Get the variant keys and batch into Lists of 500
            var variantKeys = variants.Select(x => x.Key).ToArray().Split(ChunkSize).ToList();

            var existing = this.GetProductVariantDetachedContents(variantKeys).ToArray();

            var sqlStatement = string.Empty;

            var parms = new List<object>();
            var paramIndex = 0;
            var inserts = new List<ProductVariantDetachedContentDto>();

            var variantIndex = 0;
            foreach (var variant in variants)
            {
                if (variant.DetachedContents.Any() || existing.Any(x => x.ProductVariantKey == variant.Key))
                {
                    if (existing.Any(x => x.ProductVariantKey == variant.Key) && !variant.DetachedContents.Any())
                    {
                        sqlStatement += string.Format(" DELETE [merchProductVariantDetachedContent] WHERE [productVariantKey] = @{0}", paramIndex++);
                    }

                    var slug = PathHelper.ConvertToSlug(variant.Name);
                    foreach (var dc in variant.DetachedContents)
                    {
                        if (!dc.HasIdentity)
                        {
                            ((Entity)dc).AddingEntity();
                            dc.Slug = this.EnsureSlug(dc, slug);
                            var dto = factory.BuildDto(dc);
                            inserts.Add(dto);
                        }
                        else
                        {
                            ((Entity)dc).UpdatingEntity();
                            var dto = factory.BuildDto(dc);
                            sqlStatement +=
                                string.Format(
                                    " UPDATE [merchProductVariantDetachedContent] SET [merchProductVariantDetachedContent].[detachedContentTypeKey] = @{0}, [merchProductVariantDetachedContent].[templateId] = @{1}, [merchProductVariantDetachedContent].[slug] = @{2}, [merchProductVariantDetachedContent].[values] = @{3}, [merchProductVariantDetachedContent].[canBeRendered] = @{4}, [merchProductVariantDetachedContent].[updateDate] = @{5} WHERE [merchProductVariantDetachedContent].[cultureName] = @{6} AND [merchProductVariantDetachedContent].[productVariantKey] = @{7}",
                                    paramIndex++,
                                    paramIndex++,
                                    paramIndex++,
                                    paramIndex++,
                                    paramIndex++,
                                    paramIndex++,
                                    paramIndex++,
                                    paramIndex++);

                            parms.Add(dto.DetachedContentTypeKey);
                            parms.Add(dto.TemplateId);
                            parms.Add(dto.Slug);
                            parms.Add(dto.Values);
                            parms.Add(dto.CanBeRendered);
                            parms.Add(dto.UpdateDate);
                            parms.Add(dto.CultureName);
                            parms.Add(dto.ProductVariantKey);
                        }
                    }
                }
                
                //split into batches of 100
                if (++variantIndex >= 100) {
                    if (!string.IsNullOrEmpty(sqlStatement))
                    {
                    		Database.Execute(sqlStatement, parms.ToArray());
                    }
                		variantIndex = 0;
                    sqlStatement = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(sqlStatement))
            {
                Database.Execute(sqlStatement, parms.ToArray());
            }

            if (inserts.Any())
            {
                BulkInsertRecordsWithKey<ProductVariantDetachedContentDto>(inserts);
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
                .From<ProductVariantDetachedContentDto>(SqlSyntax)
                .InnerJoin<DetachedContentTypeDto>(SqlSyntax)
                .On<ProductVariantDetachedContentDto, DetachedContentTypeDto>(
                    SqlSyntax,
                    left => left.DetachedContentTypeKey,
                    right => right.Key)
                .Where(
                    "[merchProductVariantDetachedContent].[productVariantKey] = @Key",
                    new { @Key = productVariantKey });

            var dtos = Database.Fetch<ProductVariantDetachedContentDto, DetachedContentTypeDto>(sql);

            var factory = new ProductVariantDetachedContentFactory();

            return dtos.Where(x => x != null).Select(factory.BuildEntity);
        }


        /// <summary>
        /// Gets detached content associated with the product variant.
        /// </summary>
        /// <param name="productVariantKeys">
        /// The product variant keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariantDetachedContent}"/>.
        /// </returns>
        [Obsolete("Don't use this as it has the possibility of failing")]
        internal IEnumerable<IProductVariantDetachedContent> GetProductVariantDetachedContents(IEnumerable<Guid> productVariantKeys)
        {
            var batchedKeys = productVariantKeys.ToArray().Split(ChunkSize).ToList();
            return GetProductVariantDetachedContents(batchedKeys);

            //var sql = new Sql();
            //sql.Select("*")
            //    .From<ProductVariantDetachedContentDto>(SqlSyntax)
            //    .InnerJoin<DetachedContentTypeDto>(SqlSyntax)
            //    .On<ProductVariantDetachedContentDto, DetachedContentTypeDto>(
            //        SqlSyntax,
            //        left => left.DetachedContentTypeKey,
            //        right => right.Key)
            //    .WhereIn<ProductVariantDetachedContentDto>(x => x.ProductVariantKey, productVariantKeys, SqlSyntax);

            //var dtos = Database.Fetch<ProductVariantDetachedContentDto, DetachedContentTypeDto>(sql);

            //var factory = new ProductVariantDetachedContentFactory();

            //return dtos.Select(factory.BuildEntity);
        }

        /// <summary>
        /// Gets detached content associated with the product variant.
        /// </summary>
        /// <param name="productVariantKeys">
        /// The product variant keys in list batches
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariantDetachedContent}"/>.
        /// </returns>
        internal IEnumerable<IProductVariantDetachedContent> GetProductVariantDetachedContents(List<IEnumerable<Guid>> productVariantKeys)
        {
            var dtos = new List<IProductVariantDetachedContent>();
            var factory = new ProductVariantDetachedContentFactory();

            foreach (var pvks in productVariantKeys)
            {
                var sql = new Sql();
                sql.Select("*")
                    .From<ProductVariantDetachedContentDto>(SqlSyntax)
                    .InnerJoin<DetachedContentTypeDto>(SqlSyntax)
                    .On<ProductVariantDetachedContentDto, DetachedContentTypeDto>(
                        SqlSyntax,
                        left => left.DetachedContentTypeKey,
                        right => right.Key)
                    .WhereIn<ProductVariantDetachedContentDto>(x => x.ProductVariantKey, pvks, SqlSyntax);

                dtos.AddRange(Database.Fetch<ProductVariantDetachedContentDto, DetachedContentTypeDto>(sql).Select(factory.BuildEntity));
            }

            return dtos;
        }


        #endregion

        #region RepositoryBase overrides

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

            return PerformGet(dto);
        }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        /// <param name="dto">
        /// ProductDto
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        /// <remarks>
        /// This is a combination of the overridden PerformGet(Guid) and the 
        /// MerchelloRespositoryBase Get(Guid key). Not sure where else to put this method ^LM
        /// </remarks>
        public IProductVariant PerformGet(ProductDto dto)
        {
            if (dto == null || dto.ProductVariantDto == null)
            {
                return null;
            }

            // Get needed collections
            var productAttributeCollection = _productOptionRepository.GetProductAttributeCollectionForVariant(dto.ProductVariantDto.Key);
            var catalogInventoryCollection = GetCategoryInventoryCollection(dto.ProductVariantDto.Key);
            var detachedContentCollection = GetDetachedContentCollection(dto.ProductVariantDto.Key);

            return PerformGet(dto, productAttributeCollection, catalogInventoryCollection, detachedContentCollection);
        }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        /// <param name="dto">
        /// Product Dto
        /// </param>
        /// <param name="productAttributeCollection">
        /// Populated productAttributeCollection
        /// </param>
        /// <param name="catalogInventoryCollection">
        /// Populated catalogInventoryCollection
        /// </param>
        /// <param name="detachedContentCollection">
        /// Populated detachedContentCollection
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        /// <remarks>
        /// This is a combination of the overridden PerformGet(Guid) and the 
        /// MerchelloRespositoryBase Get(Guid key). Not sure where else to put this method ^LM
        /// </remarks>
        public IProductVariant PerformGet(ProductDto dto,
                ProductAttributeCollection productAttributeCollection,
                CatalogInventoryCollection catalogInventoryCollection,
                DetachedContentCollection<IProductVariantDetachedContent> detachedContentCollection)
        {
            if (dto == null || dto.ProductVariantDto == null)
            {
                return null;
            }

            var factory = new ProductVariantFactory(productAttributeCollection, catalogInventoryCollection, detachedContentCollection);
            var variant = factory.BuildEntity(dto.ProductVariantDto);

            if (variant != null)
            {
                variant.ResetDirtyProperties();
            }

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
            var dtos = new List<ProductDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false).WhereIn<ProductVariantDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false));
            }

            return GetVariantsBulk(dtos);


            //if (keys.Any())
            //{
            //    var productVariants = new List<IProductVariant>();
            //    // TODO - This is really innefficient, even though it's adding everything to caching
            //    foreach (var key in keys)
            //    {
            //        productVariants.Add(Get(key));
            //    }
            //    return productVariants;
            //}
            //else
            //{
            //    var variantDtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false));

            //    return GetVariantsBulk(variantDtos);
            //}
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
        /// <remarks>
        /// This is a 'hefty' method, could be split out into seperate methods
        /// </remarks>
        protected override IEnumerable<IProductVariant> PerformGetByQuery(IQuery<IProductVariant> query)
        {
            var baseVariantClause = GetBaseQuery(false);
            var variantTranslator = new SqlTranslator<IProductVariant>(baseVariantClause, query);
            var variantSql = variantTranslator.Translate();
            var variantDtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(variantSql);

            // Get the variants
            return GetVariantsBulk(variantDtos);
        }

        /// <summary>
        /// Currently the most efficient way to get a list of IProductVariant
        /// TODO - Must be a better / more efficient way to do this
        /// </summary>
        /// <param name="variantDtos"></param>
        /// <returns></returns>
        private IEnumerable<IProductVariant> GetVariantsBulk(List<ProductDto> variantDtos)
        {
            var productVariants = new List<IProductVariant>();
            if (variantDtos.Any())
            {
                // Get all productvariantkeys 
                var distinctDtos = variantDtos.DistinctBy(x => x.ProductVariantDto.Key).ToArray();

                // We get all the variant keys, and split into lists
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var variantKeyLists = distinctDtos.Select(x => x.ProductVariantDto.Key).ToArray().Split(ChunkSize).ToList();

                // Get the product attribute collections - This should be a Dictionary<Guid, Collection>
                var productAttributeCollectionDictionary = GetProductAttributeCollectionDictionary(variantKeyLists);

                // Get the catalog inventory collections - This should be a Dictionary<Guid, Collection>
                var catalogInventoryCollectionDictionary = GetCatalogInventoryCollectionDictionary(variantKeyLists);

                // Get the variant detached content dictionary - This should be a Dictionary<Guid, Collection>
                var variantsDetachedContentDictionary = GetvariantsDetachedContentDictionary(variantKeyLists);

                foreach (var dto in distinctDtos)
                {
                    // Variant Key
                    var variantKey = dto.ProductVariantDto.Key;

                    // Attribute Collection
                    var productAttributeCollection = new ProductAttributeCollection();
                    if (productAttributeCollectionDictionary.ContainsKey(variantKey))
                    {
                        productAttributeCollection = productAttributeCollectionDictionary[variantKey];
                    }

                    // CatalogInventoryCollection
                    var catalogInventoryCollection = new CatalogInventoryCollection();
                    if (catalogInventoryCollectionDictionary.ContainsKey(variantKey))
                    {
                        catalogInventoryCollection = catalogInventoryCollectionDictionary[variantKey];
                    }

                    // Detached Content
                    var variantDetachedContents = new DetachedContentCollection<IProductVariantDetachedContent>();
                    if (variantsDetachedContentDictionary.ContainsKey(variantKey))
                    {
                        variantDetachedContents = variantsDetachedContentDictionary[variantKey];
                    }


                    // Add the variant
                    productVariants.Add(PerformGet(dto, productAttributeCollection, catalogInventoryCollection, variantDetachedContents));
                }
            }
            return productVariants;
        }

        /// <summary>
        /// Gets the variant DetachedContentCollection in a dictionary
        /// </summary>
        /// <param name="variantKeyLists"></param>
        /// <returns></returns>
        private Dictionary<Guid, DetachedContentCollection<IProductVariantDetachedContent>> GetvariantsDetachedContentDictionary(List<IEnumerable<Guid>> variantKeyLists)
        {
            //Get the detached content for all variants and then dump in Lookup
            var variantsDetachedContent = GetProductVariantDetachedContents(variantKeyLists).Where(x => x != null).GroupBy(x => x.ProductVariantKey);

            // Dictionary to hold the detached contents collection by key
            var variantsDetachedContentDictionary = new Dictionary<Guid, DetachedContentCollection<IProductVariantDetachedContent>>();

            // Loop lookup and populate dictionary
            foreach (var vdc in variantsDetachedContent)
            {
                variantsDetachedContentDictionary.Add(vdc.Key, new DetachedContentCollection<IProductVariantDetachedContent> { vdc });
            }

            return variantsDetachedContentDictionary;
        }

        /// <summary>
        /// Gets the CatalogInventoryCollection with the id as the key
        /// </summary>
        /// <param name="variantKeyLists"></param>
        /// <returns></returns>
        private Dictionary<Guid, CatalogInventoryCollection> GetCatalogInventoryCollectionDictionary(List<IEnumerable<Guid>> variantKeyLists)
        {
            // Hold a list for the DTOs
            var catalogInventoryCollectionDtos = new List<CatalogInventoryDto>();

            // Loop keys and populate
            foreach (var variantKeys in variantKeyLists)
            {
                var catalogInventoryCollectionSql = new Sql();
                catalogInventoryCollectionSql.Select("*")
                    .From<CatalogInventoryDto>(SqlSyntax)
                    .InnerJoin<WarehouseCatalogDto>(SqlSyntax)
                    .On<CatalogInventoryDto, WarehouseCatalogDto>(SqlSyntax, left => left.CatalogKey, right => right.Key)
                    .WhereIn<CatalogInventoryDto>(x => x.ProductVariantKey, variantKeys, SqlSyntax);

                // Execute the query
                catalogInventoryCollectionDtos.AddRange(Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(catalogInventoryCollectionSql));
            }

            // Dump into lookup
            var catalogInventoryCollectionDtosLookUp = catalogInventoryCollectionDtos.ToLookup(x => x.ProductVariantKey);

            // Get the factory
            var catalogInventoryFactory = new CatalogInventoryFactory();

            var catalogInventoryCollectionDictionary = new Dictionary<Guid, CatalogInventoryCollection>();

            foreach (var dto in catalogInventoryCollectionDtosLookUp)
            {
                var variantKey = dto.Key;
                var collection = new CatalogInventoryCollection();
                foreach (var catalogInventoryDto in dto)
                {
                    collection.Add(catalogInventoryFactory.BuildEntity(catalogInventoryDto));
                }
                catalogInventoryCollectionDictionary.Add(variantKey, collection);

            }

            return catalogInventoryCollectionDictionary;
        }

        /// <summary>
        /// Gets a dictionary of the ProductAttributeCollection key'd by guid
        /// </summary>
        /// <param name="variantKeyLists"></param>
        /// <returns></returns>
        private Dictionary<Guid, ProductAttributeCollection> GetProductAttributeCollectionDictionary(List<IEnumerable<Guid>> variantKeyLists)
        {
            // Hold the dtos
            var productVariant2ProductAttributeDtos = new List<ProductVariant2ProductAttributeDto>();

            // Loop the variant keys
            foreach (var variantKeys in variantKeyLists)
            {
                var prodAttrCollForVariantSql = new Sql();
                prodAttrCollForVariantSql.Select("*")
                    .From<ProductVariant2ProductAttributeDto>(SqlSyntax)
                    .InnerJoin<ProductAttributeDto>(SqlSyntax)
                    .On<ProductVariant2ProductAttributeDto, ProductAttributeDto>(SqlSyntax, left => left.ProductAttributeKey, right => right.Key)
                    .WhereIn<ProductVariant2ProductAttributeDto>(x => x.ProductVariantKey, variantKeys, SqlSyntax); // New Where in returns many

                // Execute the query
                productVariant2ProductAttributeDtos.AddRange(Database.Fetch<ProductVariant2ProductAttributeDto, ProductAttributeDto>(prodAttrCollForVariantSql));
            }

            // Dump into LookUp table
            var productVariant2ProductAttributeDtosLookUp = productVariant2ProductAttributeDtos.ToLookup(x => x.ProductVariantKey);

            // Get a factory
            var productAttributeFactory = new ProductAttributeFactory();

            // This should be a Dictionary<Guid, Collection>
            var productAttributeCollectionDictionary = new Dictionary<Guid, ProductAttributeCollection>();

            foreach (var dto in productVariant2ProductAttributeDtosLookUp)
            {
                var variantKey = dto.Key;
                var productAttributeCollection = new ProductAttributeCollection();

                // Loop through the group to get the attributes
                foreach (var productVariant2ProductAttributeDto in dto)
                {
                    var attribute = productAttributeFactory.BuildEntity(productVariant2ProductAttributeDto.ProductAttributeDto);

                    productAttributeCollection.Add(attribute);
                }
                productAttributeCollectionDictionary.Add(variantKey, productAttributeCollection);
            }

            return productAttributeCollectionDictionary;
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
                .From<ProductDto>(SqlSyntax)
                .InnerJoin<ProductVariantDto>(SqlSyntax)
                .On<ProductDto, ProductVariantDto>(SqlSyntax, left => left.Key, right => right.ProductKey)
                .InnerJoin<ProductVariantIndexDto>(SqlSyntax)
                .On<ProductVariantDto, ProductVariantIndexDto>(SqlSyntax, left => left.Key, right => right.ProductVariantKey);

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

            var factory = new ProductVariantFactory();

            var dto = factory.BuildDto(entity);

            // insert the variant
            Database.Insert(dto);
            entity.Key = dto.Key; // to set HasIdentity

            Database.Insert(dto.ProductVariantIndexDto);
            ((ProductVariant)entity).ExamineId = dto.ProductVariantIndexDto.Id;

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

            var factory = new ProductVariantFactory();

            var dto = factory.BuildDto(entity);

            // update the variant
            Database.Update(dto);

            SaveCatalogInventory(entity);

            SaveDetachedContents(entity);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist deleted item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IProductVariant entity)
        {
            _productOptionRepository.DeleteAllProductVariantAttributes(entity);
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { entity.Key });
            }
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
                "DELETE FROM merchProductVariant WHERE pk = @Key"
            };

            return list;
        }

        #endregion

        protected override void ApplyAddingOrUpdating(TransactionType transactionType, IProductVariant entity)
        {
            if (transactionType == TransactionType.Insert)
            {
                ((ProductBase)entity).AddingEntity();
            }
            else
            {
                ((ProductBase)entity).UpdatingEntity();
            }
        }


        /// <summary>
        /// The mandate product variant rules.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool MandateProductVariantRules(IEnumerable<IProductVariant> entities)
        {
            var success = true;
            foreach (var entity in entities)
            {
                success = MandateProductVariantRules(entity);
                if (!success) break;
            }

            return success;
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

        #region Inventory

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

        #endregion

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
                .From<ProductVariantDto>(SqlSyntax)
                .Where<ProductVariantDto>(x => x.Sku == sku && x.Key != productVariantKey, SqlSyntax);

            return Database.Fetch<ProductAttributeDto>(sql).Any();
        }


        /// <summary>
        /// True/false indicating whether or not a SKU exists on a record other than the record with the id passed
        /// </summary>
        /// <param name="entities">The collection of the <see cref="IProductVariant"/> to be excluded</param>
        /// <returns>A value indicating whether or not a SKU exists on a record other than the record with the id passed</returns>
        private bool SkuExists(IEnumerable<IProductVariant> entities)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductVariantDto>(SqlSyntax);

            // TODO - This is STILL not working as expected.
            foreach (var entity in entities)
            {
                if (entities.IndexOf(entity) == 0)
                {                    
                    // this is the last item
                    sql.Where("Sku = @sku and pk != @pk", new { @sku = entity.Sku, @pk = entity.Key });
                }
                else
                {
                    sql.Append("OR (Sku = @sku and pk != @pk)", new { @sku = entity.Sku, @pk = entity.Key });
                }
            }

            //var whereClauses = entities.Select(entity => string.Format("(Sku = '{0}' and pk != '{1}')", entity.Sku, entity.Key)).ToList();

            //sql = sql.Where(string.Join(" or ", whereClauses), null);

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
        private string EnsureSlug(IProductVariantDetachedContent detachedContent, string slug, int interval = 0)
        {
            var modSlug = interval > 0 ? string.Format("{0}-{1}", slug, interval) : slug;
            var count = Database.ExecuteScalar<int>("SELECT COUNT(slug) FROM [merchProductVariantDetachedContent] WHERE [merchProductVariantDetachedContent].[slug] = @Slug AND [merchProductVariantDetachedContent].[productVariantKey] != @Pvk", new { @Slug = modSlug, @Pvk = detachedContent.ProductVariantKey });
            if (count > 0) modSlug = EnsureSlug(detachedContent, slug, interval + 1);
            return modSlug;
        }

    }
}