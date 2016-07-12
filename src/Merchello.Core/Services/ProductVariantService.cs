namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Merchello.Core.Configuration;
    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;
    using Umbraco.Web.Models.TemplateQuery;

    /// <summary>
    /// Represents the ProductVariantService
    /// </summary>
    public class ProductVariantService : MerchelloRepositoryService, IProductVariantService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantService"/> class.
        /// </summary>
        public ProductVariantService()
            : this(LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProductVariantService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public ProductVariantService(ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : this(new RepositoryFactory(logger, sqlSyntax), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProductVariantService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProductVariantService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
        /// </param>
        public ProductVariantService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #region Events

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IProductVariantService, Events.NewEventArgs<IProductVariant>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IProductVariantService, Events.NewEventArgs<IProductVariant>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IProductVariantService, SaveEventArgs<IProductVariant>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IProductVariantService, SaveEventArgs<IProductVariant>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IProductVariantService, DeleteEventArgs<IProductVariant>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IProductVariantService, DeleteEventArgs<IProductVariant>> Deleted;

        #endregion

        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/></param>
        /// <param name="attributes">The <see cref="IProductVariant"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        public IProductVariant CreateProductVariantWithKey(IProduct product, ProductAttributeCollection attributes, bool raiseEvents = true)
        {
            var skuSeparator = MerchelloConfiguration.Current.DefaultSkuSeparator;

            // verify the order of the attributes so that a sku can be constructed in the same order as the UI
            var optionIds = product.ProductOptionsForAttributes(attributes).OrderBy(x => x.SortOrder).Select(x => x.Key).Distinct();

            // the base sku
            var sku = product.Sku;
            var name = string.Format("{0} - ", product.Name);

            foreach (var att in optionIds.Select(key => attributes.FirstOrDefault(x => x.OptionKey == key)).Where(att => att != null))
            {
                name += att.Name + " ";

                sku += skuSeparator + (string.IsNullOrEmpty(att.Sku) ? Regex.Replace(att.Name, "[^0-9a-zA-Z]+", string.Empty) : att.Sku);
            }

            return CreateProductVariantWithKey(product, name.Trim(), sku, product.Price, attributes, raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/></param>
        /// <param name="name">The name of the product variant</param>
        /// <param name="sku">The unique SKU of the product variant</param>
        /// <param name="price">The price of the product variant</param>
        /// <param name="attributes">The <see cref="IProductVariant"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        public IProductVariant CreateProductVariantWithKey(IProduct product, string name, string sku, decimal price, ProductAttributeCollection attributes, bool raiseEvents = true)
        {
            var productVariant = CreateProductVariant(product, name, sku, price, attributes);

            if(raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IProductVariant>(productVariant), this))
            {
                ((ProductVariant)productVariant).WasCancelled = true;
                return productVariant;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductVariantRepository(uow))
                {
                    repository.AddOrUpdate(productVariant);
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Created.RaiseEvent(new Events.NewEventArgs<IProductVariant>(productVariant), this);

            product.ProductVariants.Add(productVariant);

            return productVariant;
        }


        /// <summary>
        /// Saves a single instance of a <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariant">The <see cref="IProductVariant"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IProductVariant productVariant, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IProductVariant>(productVariant), this))
                {
                    ((ProductVariant)productVariant).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductVariantRepository(uow))
                {
                    repository.AddOrUpdate(productVariant);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Saved.RaiseEvent(new SaveEventArgs<IProductVariant>(productVariant), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariantList">The collection of <see cref="IProductVariant"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IProductVariant> productVariantList, bool raiseEvents = true)
        {
            var productVariants = productVariantList as IProductVariant[] ?? productVariantList.ToArray();

            if (raiseEvents)
            Saving.RaiseEvent(new SaveEventArgs<IProductVariant>(productVariants), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductVariantRepository(uow))
                {
                    foreach (var variant in productVariants)
                    {
                        repository.AddOrUpdate(variant);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents)

            Saved.RaiseEvent(new SaveEventArgs<IProductVariant>(productVariants), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariant">The <see cref="IProductVariant"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IProductVariant productVariant, bool raiseEvents = true)
        {
            if (raiseEvents)
            {
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IProductVariant>(productVariant), this))
                {
                    ((ProductVariant)productVariant).WasCancelled = true;
                    return;
                }
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductVariantRepository(uow))
                {
                    repository.Delete(productVariant);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IProductVariant>(productVariant), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariantList">The collection of <see cref="IProductVariant"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IProductVariant> productVariantList, bool raiseEvents = true)
        {
            var productVariants = productVariantList as IProductVariant[] ?? productVariantList.ToArray();

            if (raiseEvents) 
            Deleting.RaiseEvent(new DeleteEventArgs<IProductVariant>(productVariants), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductVariantRepository(uow))
                {
                    foreach (var product in productVariants)
                    {
                        repository.Delete(product);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) 
            Deleted.RaiseEvent(new DeleteEventArgs<IProductVariant>(productVariants), this);
        }

        /// <summary>
        /// Gets an <see cref="IProductVariant"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="key">key of the Product to retrieve</param>
        /// <returns><see cref="IProductVariant"/></returns>
        public IProductVariant GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets an <see cref="IProductVariant"/> object by it's unique SKU.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        public IProductVariant GetBySku(string sku)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IProductVariant>.Builder.Where(x => x.Sku == sku);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets list of <see cref="IProductVariant"/> objects given a list of Unique ids
        /// </summary>
        /// <param name="keys">List of keys for ProductVariant objects to retrieve</param>
        /// <returns>List of <see cref="IProduct"/></returns>
        public IEnumerable<IProductVariant> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }


        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> object for a given Product Key
        /// </summary>
        /// <param name="productKey">The product key of the <see cref="IProductVariant"/> collection to retrieve</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        public IEnumerable<IProductVariant> GetByProductKey(Guid productKey)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetByProductKey(productKey);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> objects associated with a given warehouse 
        /// </summary>
        /// <param name="warehouseKey">The 'unique' key of the warehouse</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        public IEnumerable<IProductVariant> GetByWarehouseKey(Guid warehouseKey)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetByWarehouseKey(warehouseKey);
            }
        }

        /// <summary>
        /// True/false indicating whether or not a SKU is already exists in the database
        /// </summary>
        /// <param name="sku">The SKU to be tested</param>
        /// <returns>A value indicating whether or not the SKU exists</returns>
        public bool SkuExists(string sku)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.SkuExists(sku);
            }
        }


        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// without saving it to the database
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/></param>
        /// <param name="attributes">The <see cref="IProductVariant"/></param>
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        internal IProductVariant CreateProductVariant(IProduct product, ProductAttributeCollection attributes)
        {
            var skuSeparator = MerchelloConfiguration.Current.DefaultSkuSeparator;

            // verify the order of the attributes so that a sku can be constructed in the same order as the UI
            var optionIds = product.ProductOptionsForAttributes(attributes).OrderBy(x => x.SortOrder).Select(x => x.Key).Distinct();

            // the base sku
            var sku = product.Sku;
            var name = string.Format("{0} - ", product.Name);

            foreach (var att in optionIds.Select(key => attributes.FirstOrDefault(x => x.OptionKey == key)).Where(att => att != null))
            {
                name += att.Name + " ";

                sku += skuSeparator + (string.IsNullOrEmpty(att.Sku) ? Regex.Replace(att.Name, "[^0-9a-zA-Z]+", string.Empty) : att.Sku);
            }

            return CreateProductVariant(product, name, sku, product.Price, attributes);
        }

        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// without saving it to the database
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/></param>
        /// <param name="name">The name of the product variant</param>
        /// <param name="sku">The unique SKU of the product variant</param>
        /// <param name="price">The price of the product variant</param>
        /// <param name="attributes">The <see cref="ProductAttributeCollection"/></param>        
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        internal IProductVariant CreateProductVariant(IProduct product, string name, string sku, decimal price, ProductAttributeCollection attributes)
        {
            Mandate.ParameterNotNull(product, "product");
            Mandate.ParameterNotNull(attributes, "attributes");
            Mandate.ParameterCondition(attributes.Count >= product.ProductOptions.Count(x => x.Required), "An attribute must be assigned for every required option");

            //// http://issues.merchello.com/youtrack/issue/M-740
            // verify there is not already a variant with these attributes
            ////Mandate.ParameterCondition(false == ProductVariantWithAttributesExists(product, attributes), "A ProductVariant already exists for the ProductAttributeCollection");
            if (this.ProductVariantWithAttributesExists(product, attributes))
            {
                LogHelper.Debug<ProductVariantService>("Attempt to create a new variant that already exists.  Returning existing variant.");
                return this.GetProductVariantWithAttributes(product, attributes.Select(x => x.Key).ToArray());
            }


            return new ProductVariant(product.Key, attributes, name, sku, price)
            {
                CostOfGoods = product.CostOfGoods,
                SalePrice = product.SalePrice,
                OnSale = product.OnSale,
                Weight = product.Weight,
                Length = product.Length,
                Width = product.Width,
                Height = product.Height,
                Barcode = product.Barcode,
                Available = product.Available,
                Manufacturer = product.Manufacturer,
                ManufacturerModelNumber = product.ManufacturerModelNumber,
                TrackInventory = product.TrackInventory,
                OutOfStockPurchase = product.OutOfStockPurchase,
                Taxable = product.Taxable,
                Shippable = product.Shippable,
                Download = product.Download
            };
        }

        /// <summary>
        /// Gets the count of all product variants.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariant}"/>.
        /// </returns>
        /// <remarks>
        /// Used in tests
        /// </remarks>
        internal IEnumerable<IProductVariant> GetAll(params Guid[] keys)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys);
            }
        }

        /// <summary>
        /// Gets the count of <see cref="IProductVariant"/> by a query
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal int Count(IQuery<IProductVariant> query)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Count(query);
            }
        }


        /// <summary>
        /// Creates a collection of <see cref="IProductVariant"/> that can be created based on unmapped product options.
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/></param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        /// <remarks>
        /// 
        /// This is an expensive method due to the potential number of database calls and should probably 
        /// be refactored
        /// 
        /// </remarks>
        [Obsolete("Unused method")]
        private IEnumerable<IProductVariant> GetProductVariantsThatCanBeCreated(IProduct product)
        {
            var variants = new List<IProductVariant>();

            if (!product.ProductOptions.Any()) return variants;

            foreach (var combo in product.GetPossibleProductAttributeCombinations())
            {
                var attributes = new ProductAttributeCollection();

                foreach (var att in combo) attributes.Add(att);

                if (!ProductVariantWithAttributesExists(product, attributes)) variants.Add(CreateProductVariant(product, attributes));
            }

            return variants;
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
        private IProductVariant GetProductVariantWithAttributes(IProduct product, Guid[] attributeKeys)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductVariantWithAttributes(product, attributeKeys);
            }
        }

        /// <summary>
        /// Compares the <see cref="ProductAttributeCollection"/> with other <see cref="IProductVariant"/>s of the <see cref="IProduct"/> pass
        /// to determine if the a variant already exists with the attributes passed
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to reference</param>
        /// <param name="attributes"><see cref="ProductAttributeCollection"/> to compare</param>
        /// <returns>True/false indicating whether or not a <see cref="IProductVariant"/> already exists with the <see cref="ProductAttributeCollection"/> passed</returns>
        private bool ProductVariantWithAttributesExists(IProduct product, ProductAttributeCollection attributes)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.ProductVariantWithAttributesExists(product, attributes);
            }
        }
    }
}