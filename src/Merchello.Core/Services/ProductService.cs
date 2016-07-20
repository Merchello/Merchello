namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    using RepositoryFactory = Merchello.Core.Persistence.RepositoryFactory;

    /// <summary>
    /// Represents the Product Service 
    /// </summary>
    public class ProductService : PageCachedServiceBase<IProduct>, IProductService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// The valid sort fields.
        /// </summary>
        private static readonly string[] ValidSortFields = { "sku", "name", "price" };

        /// <summary>
        /// The product variant service.
        /// </summary>
        private readonly IProductVariantService _productVariantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        public ProductService()
            : this(LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProductService(ILogger logger)
            : this(new RepositoryFactory(), logger, new ProductVariantService(logger))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public ProductService(ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : this(new RepositoryFactory(logger, sqlSyntax), logger, new ProductVariantService(logger, sqlSyntax))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="productVariantService">
        /// The product variant service.
        /// </param>
        public ProductService(RepositoryFactory repositoryFactory, ILogger logger, IProductVariantService productVariantService)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger, productVariantService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
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
        /// <param name="productVariantService">
        /// The product variant service.
        /// </param>
        public ProductService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IProductVariantService productVariantService)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory(), productVariantService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
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
        /// <param name="productVariantService">
        /// The product variant service.
        /// </param>
        public ProductService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory, IProductVariantService productVariantService)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
            Mandate.ParameterNotNull(productVariantService, "productVariantService");
            _productVariantService = productVariantService;
        }

        #region Event Handlers



        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IProductService, Events.NewEventArgs<IProduct>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IProductService, Events.NewEventArgs<IProduct>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IProductService, SaveEventArgs<IProduct>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IProductService, SaveEventArgs<IProduct>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IProductService, DeleteEventArgs<IProduct>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IProductService, DeleteEventArgs<IProduct>> Deleted;

        #endregion

        /// <summary>
        /// Creates a Product without saving it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        public IProduct CreateProduct(string name, string sku, decimal price, bool raiseEvents = true)
        {
            var templateVariant = new ProductVariant(name, sku, price);
            var product = new Product(templateVariant);
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IProduct>(product), this))
            {
                product.WasCancelled = true;
                return product;
            }

            if (raiseEvents)
            Created.RaiseEvent(new Events.NewEventArgs<IProduct>(product), this);

            return product;
        }

        /// <summary>
        /// Creates and saves a <see cref="IProduct"/> to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        public IProduct CreateProductWithKey(string name, string sku, decimal price, bool raiseEvents = true)
        {
            var templateVariant = new ProductVariant(name, sku, price);
            var product = new Product(templateVariant);

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IProduct>(product), this))
            {
                product.WasCancelled = true;
                return product;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductRepository(uow))
                {
                    repository.AddOrUpdate(product);
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Created.RaiseEvent(new Events.NewEventArgs<IProduct>(product), this);

            return product;
        }

        /// <summary>
        /// Saves a single <see cref="IProduct"/> object
        /// </summary>
        /// <param name="product">The <see cref="IProductVariant"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IProduct product, bool raiseEvents = true)
        {
            if (raiseEvents)
            {
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IProduct>(product), this))
                {
                    ((Product)product).WasCancelled = true;
                    return;
                }
            }
            
            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductRepository(uow))
                {
                    repository.AddOrUpdate(product);
                    uow.Commit();
                }

                // Synchronize product variants
                this.EnsureVariants(product);
            }

            // verify that all variants of this product still have attributes - or delete them
            EnsureProductVariantsHaveAttributes(product);

            // save any remaining variants changes in the variants collection
            if (product.ProductVariants.Any())
            _productVariantService.Save(product.ProductVariants);

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IProduct>(product), this);

        }

        /// <summary>
        /// Saves a collection of <see cref="IProduct"/> objects.
        /// </summary>
        /// <param name="productList">Collection of <see cref="ProductVariant"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IProduct> productList, bool raiseEvents = true)
        {
            var productArray = productList as IProduct[] ?? productList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IProduct>(productArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductRepository(uow))
                {
                    foreach (var product in productArray)
                    {
                        repository.AddOrUpdate(product);
                    }

                    uow.Commit();
                }

                // Synchronize the products array
                EnsureVariants(productArray);
            }

            // verify that all variants of these products still have attributes - or delete them
            productArray.ForEach(EnsureProductVariantsHaveAttributes);

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IProduct>(productArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IProduct"/> object
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IProduct product, bool raiseEvents = true)
        {
            if (raiseEvents)
            {
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IProduct>(product), this))
                {
                    ((Product)product).WasCancelled = true;
                    return;
                }
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductRepository(uow))
                {
                    repository.Delete(product);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IProduct>(product), this);
        }


        /// <summary>
        /// Deletes a collection <see cref="IProduct"/> objects
        /// </summary>
        /// <param name="productList">Collection of <see cref="IProduct"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IProduct> productList, bool raiseEvents = true)
        {
            var productArray = productList as IProduct[] ?? productList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IProduct>(productArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductRepository(uow))
                {
                    foreach (var product in productArray)
                    {
                        repository.Delete(product);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IProduct>(productArray), this);
        }

        /// <summary>
        /// Gets an <see cref="IProduct"/> by it's unique SKU.
        /// </summary>
        /// <param name="sku">
        /// The product SKU.
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        public IProduct GetBySku(string sku)
        {
            using (var repository = RepositoryFactory.CreateProductVariantRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IProductVariant>.Builder.Where(x => x.Sku == sku && ((ProductVariant)x).Master);
                var variant = repository.GetByQuery(query).FirstOrDefault();
                return variant == null ? null : GetByKey(variant.ProductKey);
            }
        }

        /// <summary>
        /// Gets a Product by its unique id - primary key
        /// </summary>
        /// <param name="key">GUID key for the Product</param>
        /// <returns><see cref="IProductVariant"/></returns>
        public override IProduct GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a page of <see cref="IProduct"/>
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProduct}"/>.
        /// </returns>
        public override Page<IProduct> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetPage(page, itemsPerPage, null, ValidateSortByField(sortBy), sortDirection);
            }
        }       

        /// <summary>
        /// Gets a list of Product give a list of unique keys
        /// </summary>
        /// <param name="keys">
        /// List of unique keys
        /// </param>
        /// <returns>
        /// A collection of <see cref="IProduct"/>.
        /// </returns>
        public IEnumerable<IProduct> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets a <see cref="IProductVariant"/> by it's key.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        public IProductVariant GetProductVariantByKey(Guid productVariantKey)
        {
            return _productVariantService.GetByKey(productVariantKey);
        }

        /// <summary>
        /// Get's a <see cref="IProductVariant"/> by it's unique SKU.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        public IProductVariant GetProductVariantBySku(string sku)
        {
            return _productVariantService.GetBySku(sku);
        }

        /// <summary>
        /// The get product variants by product key.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariant}"/>.
        /// </returns>
        public IEnumerable<IProductVariant> GetProductVariantsByProductKey(Guid productKey)
        {
            return _productVariantService.GetByProductKey(productKey);
        }

        /// <summary>
        /// Returns the count of all products
        /// </summary>
        /// <returns>
        /// The total product count.
        /// </returns>
        [Obsolete("Only used in ProductQuery")]
        public int ProductsCount()
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IProduct>.Builder.Where(x => x.Key != Guid.Empty);

                return repository.Count(query);
            }
        }

        /// <summary>
        /// True/false indicating whether or not a SKU is already exists in the database
        /// </summary>
        /// <param name="sku">
        /// The SKU to be tested
        /// </param>
        /// <returns>
        /// A value indicating whether or not  a SKU exists
        /// </returns>
        public bool SkuExists(string sku)
        {
            return _productVariantService.SkuExists(sku);
        }

        /// <summary>
        /// Removes detached content from the product.
        /// </summary>
        /// <param name="product">
        /// The product variants.
        /// </param>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void RemoveDetachedContent(IProduct product, Guid detachedContentTypeKey, bool raiseEvents = true)
        {                      
            Save(this.RemoveDetachedContentFromProduct(product, detachedContentTypeKey), raiseEvents);
        }

        /// <summary>
        /// Removes detached content from the collection of products
        /// </summary>
        /// <param name="products">
        /// The product variants.
        /// </param>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void RemoveDetachedContent(IEnumerable<IProduct> products, Guid detachedContentTypeKey, bool raiseEvents = true)
        {
            var productsArray = products as IProduct[] ?? products.ToArray();
            if (!productsArray.Any()) return;
            var modified = productsArray.Select(p => this.RemoveDetachedContentFromProduct(p, detachedContentTypeKey)).ToList();
            Save(modified, raiseEvents);
        }

        /// <summary>
        /// Gets a collect of products by detached content type.
        /// </summary>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Product}"/>.
        /// </returns>
        public IEnumerable<IProduct> GetByDetachedContentType(Guid detachedContentTypeKey)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetByDetachedContentType(detachedContentTypeKey);
            }
        }

        /// <summary>
        /// The add product to collection.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void AddToCollection(IProduct product, IEntityCollection collection)
        {
            this.AddToCollection(product, collection.Key);
        }

        /// <summary>
        /// The add product to collection.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(IProduct product, Guid collectionKey)
        {
            this.AddToCollection(product.Key, collectionKey);
        }

        /// <summary>
        /// The add product to collection.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(Guid productKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                repository.AddToCollection(productKey, collectionKey);
            }
        }

        /// <summary>
        /// The exists in collection.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid productKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.ExistsInCollection(productKey, collectionKey);
            }
        }

        /// <summary>
        /// The remove product from collection.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void RemoveFromCollection(IProduct product, IEntityCollection collection)
        {
            this.RemoveFromCollection(product, collection.Key);
        }

        /// <summary>
        /// The remove product from collection.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void RemoveFromCollection(IProduct product, Guid collectionKey)
        {
            this.RemoveFromCollection(product.Key, collectionKey);
        }

        /// <summary>
        /// The remove product from collection.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void RemoveFromCollection(Guid productKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                repository.RemoveFromCollection(productKey, collectionKey);
            }
        }

        /// <summary>
        /// Gets products from a collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProduct}"/>.
        /// </returns>
        public Page<IProduct> GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetFromCollection(collectionKey, page, itemsPerPage, this.ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// Gets products from a collection filtered by a search term.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProduct}"/>.
        /// </returns>
        public Page<IProduct> GetFromCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetFromCollection(collectionKey, searchTerm, page, itemsPerPage, this.ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// Gets all the products
        /// </summary>
        /// <returns>
        /// A collection of all <see cref="IProduct"/>.
        /// </returns>
        public IEnumerable<IProduct> GetAll()
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// The get product keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysFromCollection(collectionKey, page, itemsPerPage, this.ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// The get keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysFromCollection(collectionKey, searchTerm, page, itemsPerPage, this.ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysNotInCollection(collectionKey, page, itemsPerPage, this.ValidateSortByField(sortBy), sortDirection);
            } 
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysNotInCollection(collectionKey, searchTerm, page, itemsPerPage, this.ValidateSortByField(sortBy), sortDirection);
            }
        }

        #region Filtering

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionKey">
        /// The option key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysWithOption(
            Guid optionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysWithOption(
                    optionKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceNames">
        /// The choice names.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysWithOption(
            string optionName,
            IEnumerable<string> choiceNames,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysWithOption(
                    optionName,
                    choiceNames,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysWithOption(
            string optionName,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysWithOption(
                    optionName,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceName">
        /// The choice name.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysWithOption(
            string optionName,
            string choiceName,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysWithOption(
                    optionName,
                    choiceName,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionNames">
        /// The option names.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysWithOption(
            IEnumerable<string> optionNames,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysWithOption(
                    optionNames,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys in price range.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysInPriceRange(
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysInPriceRange(
                    min,
                    max,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys in price range.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="taxModifier">
        /// The tax modifier.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysInPriceRange(
            decimal min,
            decimal max,
            decimal taxModifier,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysInPriceRange(
                    min,
                    max,
                    taxModifier,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products by barcode.
        /// </summary>
        /// <param name="barcode">
        /// The barcode.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsByBarcode(
            string barcode,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysByBarcode(
                    barcode,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products by barcodes.
        /// </summary>
        /// <param name="barcodes">
        /// The barcodes.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsByBarcode(
            IEnumerable<string> barcodes,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysByBarcode(
                    barcodes,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        } 


        /// <summary>
        /// The get products keys by manufacturer.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysByManufacturer(
            string manufacturer,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysByManufacturer(
                    manufacturer,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys by manufacturer.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysByManufacturer(
            IEnumerable<string> manufacturer,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysByManufacturer(
                    manufacturer,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys in stock.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <param name="includeAllowOutOfStockPurchase">
        /// The include allow out of stock purchase.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysInStock(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending,
            bool includeAllowOutOfStockPurchase = false)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysInStock(
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get products keys on sale.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetProductsKeysOnSale(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductsKeysOnSale(
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        #endregion

        /// <summary>
        /// The count.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal override int Count(IQuery<IProduct> query)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Count(query);
            }
        }

        /// <summary>
        /// The count.
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
        /// Gets a page of product keys
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal override Page<Guid> GetPagedKeys(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetPagedKeys(page, itemsPerPage, null, ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// The get paged keys.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetPagedKeys(string searchTerm, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.SearchKeys(searchTerm, page, itemsPerPage, ValidateSortByField(sortBy), sortDirection);
            }
        }


        /// <summary>
        /// Gets the product key associated with a slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The product key.
        /// </returns>
        internal Guid GetKeyForSlug(string slug)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeyForSlug(slug);
            }
        }

        /// <summary>
        /// The validate sort by field.
        /// </summary>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string ValidateSortByField(string sortBy)
        {
            return ValidSortFields.Contains(sortBy.ToLowerInvariant()) ? sortBy : "name";
        }

        /// <summary>
        /// Ensures that variants are created for each option and option choice combination
        /// </summary>
        /// <param name="products">
        /// The collection of products.
        /// </param>
        private void EnsureVariants(IEnumerable<IProduct> products)
        {
            products.ForEach(this.EnsureVariants);
        }

        /// <summary>
        /// Ensures that variants are created for each option and option choice combination
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        private void EnsureVariants(IProduct product)
        {
            var attributeLists = product.GetPossibleProductAttributeCombinations().ToArray();

            // delete any variants that don't have the correct number of attributes
            var attCount = attributeLists.Any() ? attributeLists.First().Count() : 0;

            var removers = product.ProductVariants.Where(x => x.Attributes.Count() != attCount).ToArray();
            if (removers.Any())
            {
                foreach (var remover in removers)
                {
                    product.ProductVariants.Remove(remover.Sku);

                }
                _productVariantService.Delete(removers);
            }

            var newVariants = new List<IProductVariant>();
            foreach (var list in attributeLists)
            {
                // Check to see if the variant exists
                var productAttributes = list as IProductAttribute[] ?? list.ToArray();
                   
                if (product.GetProductVariantForPurchase(productAttributes) != null) continue;
                   
                var variant = this._productVariantService.CreateProductVariantWithKey(product, productAttributes.ToProductAttributeCollection(), false);
                foreach (var inv in product.CatalogInventories)
                {
                    variant.AddToCatalogInventory(inv.CatalogKey);
                    newVariants.Add(variant);
                }
            }

            if (newVariants.Any()) _productVariantService.Save(newVariants, false);
        }

        /// <summary>
        /// Ensures that all <see cref="IProductVariant"/> except the "master" variant for the <see cref="IProduct"/> have attributes
        /// </summary>
        /// <param name="product"><see cref="IProduct"/> to verify</param>
        private void EnsureProductVariantsHaveAttributes(IProduct product)
        {
            var variants = _productVariantService.GetByProductKey(product.Key);
            var productVariants = variants as IProductVariant[] ?? variants.ToArray();
            if (!productVariants.Any()) return;
            var removers = new List<IProductVariant>();
            foreach (var variant in productVariants.Where(variant => !variant.Attributes.Any()))
            {
                removers.Add(variant);
                product.ProductVariants.Remove(variant.Sku);
            }

            _productVariantService.Delete(removers);
        }

        /// <summary>
        /// The remove detached content from product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key.
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        private IProduct RemoveDetachedContentFromProduct(IProduct product, Guid detachedContentTypeKey)
        {
            // remove from product
            if (product.DetachedContents.Any(x => x.DetachedContentType.Key == detachedContentTypeKey))
            {
                product.DetachedContents.Clear();
            }


            // remove from variants
            foreach (var variant in product.ProductVariants.ToArray().Where(variant => variant.DetachedContents.Any(x => x.DetachedContentType.Key == detachedContentTypeKey)))
            {
                variant.DetachedContents.Clear();
            }

            return product;
        }
    }
}