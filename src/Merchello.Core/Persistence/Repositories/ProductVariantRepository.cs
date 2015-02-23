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


        protected override IProductVariant PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ProductVariantDto, ProductVariantIndexDto>(sql).FirstOrDefault();

            if (dto == null || dto == null)
                return null;

            var factory = new ProductVariantFactory(GetProductAttributeCollection(key), GetCategoryInventoryCollection(key));
            var variant = factory.BuildEntity(dto);

            variant.ResetDirtyProperties();

            return variant;
        }

        protected override IEnumerable<IProductVariant> PerformGetAll(params Guid[] keys)
        {
            if (!keys.Any()) return new List<IProductVariant>();

            var sql = GetBaseQuery(false);
            if (keys.Any())
            {
                // Note:  This will not work if keys collection > 2000 count
                sql = sql.WhereIn<ProductVariantDto>(v => v.Key, keys);
            }

            var dtos = Database.Fetch<ProductVariantDto, ProductVariantIndexDto>(sql);

            sql = new Sql();
            sql.Select("*")
                .From<ProductVariant2ProductAttributeDto>()
                .InnerJoin<ProductAttributeDto>()
                .On<ProductVariant2ProductAttributeDto, ProductAttributeDto>(left => left.ProductAttributeKey, right => right.Key);
            if (keys.Any())
            {
                sql = sql.WhereIn<ProductVariant2ProductAttributeDto>(x => x.ProductVariantKey, keys);
            }
            var attributeDtos = Database.Fetch<ProductVariant2ProductAttributeDto, ProductAttributeDto>(sql);

            sql = new Sql();
            sql.Select("*")
                .From<CatalogInventoryDto>()
                .InnerJoin<WarehouseCatalogDto>()
                .On<CatalogInventoryDto, WarehouseCatalogDto>(left => left.CatalogKey, right => right.Key);
            if (keys.Any())
            {
                sql = sql.WhereIn<CatalogInventoryDto>(x => x.ProductVariantKey, keys);
            }
            var inventoryDtos = Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(sql);

            if (dtos == null || !dtos.Any())
                return null;

            var variants = new List<IProductVariant>();
            foreach (var dto in dtos)
            {
                var factory = new ProductVariantFactory();
                var variant = factory.BuildEntity(dto, attributeDtos.Where(a => a.ProductVariantKey == dto.Key).Select(a => a.ProductAttributeDto), inventoryDtos.Where(i => i.ProductVariantKey == dto.Key));
                variant.ResetDirtyProperties();
                variants.Add(variant);
            }

            return variants;
        }

        protected override IEnumerable<IProductVariant> PerformGetByQuery(IQuery<IProductVariant> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProductVariant>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductVariantDto, ProductVariantIndexDto>(sql);

            return GetAll(dtos.DistinctBy(x => x.Key).Select(dto => dto.Key).ToArray());
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<ProductVariantDto>()
                .InnerJoin<ProductVariantIndexDto>()
                .On<ProductVariantDto, ProductVariantIndexDto>(left => left.Key, right => right.ProductVariantKey);
                //.Where<ProductVariantDto>(x => x.Master == false);

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchProductVariant.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchCatalogInventory WHERE productVariantKey = @Key",
                "DELETE FROM merchProductVariantIndex WHERE productVariantKey = @Key",
                "DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey = @Key",
                "DELETE FROM merchProductVariant WHERE pk = @Key"
            };

            return list;
        }
      
        protected override void PersistNewItem(IProductVariant entity)
        {
            if (!MandateProductVariantRules(entity)) return;

            //Mandate.ParameterCondition(!SkuExists(entity.Sku), "The sku must be unique");

            ((Entity)entity).AddingEntity();

            ((ProductVariant)entity).VersionKey = Guid.NewGuid();

            var factory = new ProductVariantFactory(((ProductVariant)entity).ProductAttributes, ((ProductVariant)entity).CatalogInventoryCollection);
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

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProduct>(entity.ProductKey));

        }

        public void PersistNewItems(IEnumerable<IProductVariant> entities)
        {
            if (!MandateProductVariantRules(entities)) return;

            //Mandate.ParameterCondition(!SkuExists(entity.Sku), "The sku must be unique");
            var dtos = new List<ProductVariantDto>();
            foreach (var entity in entities)
            {
                ((Entity)entity).AddingEntity();

                ((ProductVariant)entity).VersionKey = Guid.NewGuid();
                var factory = new ProductVariantFactory(((ProductVariant)entity).ProductAttributes, ((ProductVariant)entity).CatalogInventoryCollection);
                dtos.Add(factory.BuildDto(entity));
            }

            // insert the variants
            Database.BulkInsertRecords<ProductVariantDto>(dtos);
            Database.BulkInsertRecords<ProductVariantIndexDto>(dtos.Select(v => v.ProductVariantIndexDto));

            foreach (var entity in entities)
            {
                var dto = dtos.FirstOrDefault(d => d.VersionKey == entity.VersionKey);
                entity.Key = dto.Key; // to set HasIdentity
                ((ProductVariant)entity).ExamineId = dto.ProductVariantIndexDto.Id;
            }

            // insert associations for every attribute
            Database.BulkInsertRecords<ProductVariant2ProductAttributeDto>(entities.SelectMany(e => e.Attributes.Select(att => new ProductVariant2ProductAttributeDto()
                {
                    ProductVariantKey = e.Key,
                    OptionKey = att.OptionKey,
                    ProductAttributeKey = att.Key,
                    UpdateDate = DateTime.Now,
                    CreateDate = DateTime.Now
                })));

            SaveCatalogInventory(entities);

            foreach (var entity in entities)
            {
                entity.ResetDirtyProperties();
                RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProduct>(entity.ProductKey));
            }
        }

        protected override void PersistUpdatedItem(IProductVariant entity)
        {
            if (!MandateProductVariantRules(entity)) return;

            //Mandate.ParameterCondition(!SkuExists(entity.Sku, entity.Key), "Entity cannot be updated.  The sku already exists.");

            ((Entity)entity).UpdatingEntity();
            ((ProductVariant)entity).VersionKey = Guid.NewGuid();

            var factory = new ProductVariantFactory(((ProductVariant)entity).ProductAttributes, ((ProductVariant)entity).CatalogInventoryCollection);
            var dto = factory.BuildDto(entity);

            // update the variant
            Database.Update(dto);

            SaveCatalogInventory(entity);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProduct>(entity.ProductKey));
        }

        public void PersistUpdatedItems(IEnumerable<IProductVariant> entities)
        {
            if (!MandateProductVariantRules(entities)) return;

            Mandate.ParameterCondition(!SkuExists(entities), "Entity cannot be updated.  The sku already exists.");

            // Each record requires 26 parameters at the time of this writing, so we need to split into batches due to the
            // hard-coded 2100 parameter limit imposed by SQL Server
            int batchSize = 2100 / 26;
            int batchCount = ((entities.Count() - 1) / batchSize) + 1;
            for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
            {
                string sqlStatement = "";
                int paramIndex = 0;
                var parms = new List<object>();
                foreach (var entity in entities.Skip(batchIndex * batchSize).Take(batchSize))
                {
                    ((Entity)entity).UpdatingEntity();
                    ((ProductVariant)entity).VersionKey = Guid.NewGuid();

                    // TODO:  Make generic:  Build this dynamically based on POCO
                    sqlStatement += string.Format(" UPDATE [merchProductVariant] SET");
                    sqlStatement += string.Format(" [productKey] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [sku] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [name] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [price] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [costOfGoods] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [salePrice] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [onSale] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [manufacturer] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [modelNumber] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [weight] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [length] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [width] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [height] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [barcode] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [available] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [trackInventory] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [outOfStockPurchase] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [taxable] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [shippable] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [download] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [downloadMediaId] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [master] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [versionKey] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [updateDate] = @{0}", paramIndex++);
                    sqlStatement += string.Format(", [createDate] = @{0}", paramIndex++);
                    sqlStatement += string.Format(" WHERE [pk] = @{0}", paramIndex++);

                    parms.AddRange(new List<object>() { entity.ProductKey, entity.Sku, entity.Name, entity.Price, entity.CostOfGoods, entity.SalePrice, entity.OnSale, entity.Manufacturer, entity.ManufacturerModelNumber, entity.Weight, entity.Length, entity.Width, entity.Height, entity.Barcode, entity.Available, entity.TrackInventory, entity.OutOfStockPurchase, entity.Taxable, entity.Shippable, entity.Download, entity.DownloadMediaId, ((ProductVariant)entity).Master, entity.VersionKey, entity.UpdateDate, entity.CreateDate, entity.Key });
                }
                // Commit the batch
                Database.Execute(sqlStatement, parms.ToArray());
            }

            SaveCatalogInventory(entities);

            foreach (var entity in entities)
            {
                entity.ResetDirtyProperties();
                RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProduct>(entity.ProductKey));
            }
        }

        protected override void PersistDeletedItem(IProductVariant entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { entity.Key });
            }

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProduct>(entity.ProductKey));
        }

        public void PersistDeletedItems(IEnumerable<IProductVariant> entity)
        {
            throw new NotImplementedException();
        }

        private static bool MandateProductVariantRules(IProductVariant entity)
        {
            // TODO these checks can probably be moved somewhere else but are here at the moment to enforce the rules as the API develops
            Mandate.ParameterCondition(entity.ProductKey != Guid.Empty, "productKey must be set");

            if (!((ProductVariant)entity).Master)
                Mandate.ParameterCondition(entity.Attributes.Any(), "Product variant must have attributes");            

            return true;
        }

        private static bool MandateProductVariantRules(IEnumerable<IProductVariant> entities)
        {
            foreach (var entity in entities)
            {
                // TODO these checks can probably be moved somewhere else but are here at the moment to enforce the rules as the API develops
                Mandate.ParameterCondition(entity.ProductKey != Guid.Empty, "productKey must be set");

                if (!((ProductVariant)entity).Master)
                    Mandate.ParameterCondition(entity.Attributes.Any(), "Product variant must have attributes");
            }
            return true;
        }

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



        // this merely asserts that an assoicate between the warehouse and the variant has been made
        internal void SaveCatalogInventory(IProductVariant productVariant)
        {
            var existing = GetCategoryInventoryCollection(productVariant.Key);

            foreach (var inv in existing.Where(inv => !((ProductVariant)productVariant).CatalogInventoryCollection.Contains(inv.CatalogKey)))
            {
                DeleteCatalogInventory(productVariant.Key, inv.CatalogKey);
            }

            foreach (var inv in productVariant.CatalogInventories.Where((inv => !existing.Contains(inv.CatalogKey))))
            {
                AddCatalogInventory(productVariant, inv);
            }

            foreach (var inv in productVariant.CatalogInventories.Where((x => existing.Contains(x.CatalogKey))))
            {
                UpdateCatalogInventory(inv);
            }
        }

        // this merely asserts that an assoicate between the warehouse and the variant has been made
        internal void SaveCatalogInventory(IEnumerable<IProductVariant> productVariants)
        {
            var keys = productVariants.Select(v => v.Key);
            var sql = new Sql();
            sql.Select("*")
                .From<CatalogInventoryDto>()
                .InnerJoin<WarehouseCatalogDto>()
                .On<CatalogInventoryDto, WarehouseCatalogDto>(left => left.CatalogKey, right => right.Key);
            if (keys.Any())
            {
                sql = sql.WhereIn<CatalogInventoryDto>(x => x.ProductVariantKey, keys);
            }
            var inventoryDtos = Database.Fetch<CatalogInventoryDto, WarehouseCatalogDto>(sql);

            string sqlStatement = "";
            int paramIndex = 0;
            var parms = new List<object>();
            var inserts = new List<CatalogInventoryDto>();
            foreach (var productVariant in productVariants)
            {
                foreach (var dto in inventoryDtos.Where(i => i.ProductVariantKey == productVariant.Key))
                {
                    if (!((ProductVariant)productVariant).CatalogInventoryCollection.Contains(dto.CatalogKey))
                    {
                        sqlStatement += string.Format(" DELETE FROM merchCatalogInventory WHERE productVariantKey = @{0} AND catalogKey = @{1}", paramIndex++, paramIndex++);
                        parms.Add(dto.ProductVariantKey);
                        parms.Add(dto.CatalogKey);
                    }
                }
                foreach (var inv in (productVariant.CatalogInventories))
                {
                    inv.UpdateDate = DateTime.Now;
                    if (inventoryDtos.Any(i => i.ProductVariantKey == productVariant.Key && i.CatalogKey == inv.CatalogKey))
                    {
                        sqlStatement += string.Format(" UPDATE merchCatalogInventory SET Count = @{0}, LowCount = @{1}, Location = @{2}, UpdateDate = @{3} WHERE catalogKey = @{4} AND productVariantKey = @{5}", paramIndex++, paramIndex++, paramIndex++, paramIndex++, paramIndex++, paramIndex++);
                        parms.Add(inv.Count);
                        parms.Add(inv.LowCount);
                        parms.Add(inv.Location);
                        parms.Add(inv.UpdateDate);
                        parms.Add(inv.CatalogKey);
                        parms.Add(inv.ProductVariantKey);
                    }
                    else
                    {
                        inv.CreateDate = DateTime.Now;
                        inserts.Add(new CatalogInventoryDto()
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
            }
            if (!string.IsNullOrEmpty(sqlStatement))
            {
                Database.Execute(sqlStatement, parms.ToArray());
            }
            if (inserts.Any())
            {
                Database.BulkInsertRecords<CatalogInventoryDto>(inserts);
            }
        }

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

        private void DeleteCatalogInventory(Guid productVariantKey, Guid catalogKey)
        {
            const string sql = "DELETE FROM merchCatalogInventory WHERE productVariantKey = @pvKey AND catalogKey = @cKey";
            Database.Execute(sql, new {@pvKey = productVariantKey, @cKey = catalogKey});
        }


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

        private bool SkuExists(IEnumerable<IProductVariant> entities)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductVariantDto>();
            var whereClauses = new List<string>();
            foreach (var entity in entities)
            {
                whereClauses.Add(string.Format("(Sku = '{0}' and pk != '{1}')", entity.Sku, entity.Key));
            }
            sql = sql.Where(string.Join(" or ", whereClauses), null);

            return Database.Fetch<ProductAttributeDto>(sql).Any();
        }

    }

}
