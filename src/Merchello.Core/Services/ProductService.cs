namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Models;

    using Persistence.Querying;
    using Persistence.UnitOfWork;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

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
        /// The uow provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        /// <summary>
        /// The product variant service.
        /// </summary>
        private readonly IProductVariantService _productVariantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        public ProductService()
            : this(new RepositoryFactory(), new ProductVariantService())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="productVariantService">
        /// The product variant service.
        /// </param>
        public ProductService(RepositoryFactory repositoryFactory, IProductVariantService productVariantService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, productVariantService)
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
        /// <param name="productVariantService">
        /// The product variant service.
        /// </param>
        public ProductService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IProductVariantService productVariantService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(productVariantService, "productVariantService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;

            // included the ProductVariantService so that events will trigger if variants
            // need to be deleted due to a product save removing attributes
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
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        public IProduct CreateProduct(string name, string sku, decimal price)
        {
            var templateVariant = new ProductVariant(name, sku, price);
            var product = new Product(templateVariant);
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IProduct>(product), this))
            {
                product.WasCancelled = true;
                return product;
            }

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
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        public IProduct CreateProductWithKey(string name, string sku, decimal price)
        {
            var templateVariant = new ProductVariant(name, sku, price);
            var product = new Product(templateVariant);
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IProduct>(product), this))
            {
                product.WasCancelled = true;
                return product;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductRepository(uow))
                {
                    repository.AddOrUpdate(product);
                    uow.Commit();
                }
            }

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
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductRepository(uow))
                {
                    repository.AddOrUpdate(product);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IProduct>(product), this);

            // verify that all variants of this product still have attributes - or delete them
            _productVariantService.EnsureProductVariantsHaveAttributes(product);

            // save any remaining variants changes in the variants collection
            if (product.ProductVariants.Any())
            _productVariantService.Save(product.ProductVariants);
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
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductRepository(uow))
                {
                    foreach (var product in productArray)
                    {
                        repository.AddOrUpdate(product);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IProduct>(productArray), this);

            // verify that all variants of these products still have attributes - or delete them
            _productVariantService.EnsureProductVariantsHaveAttributes(productArray);

            // save any remaining variants changes in the variants collections
            foreach (var collection in productArray.Select(x => x.ProductVariants).Where(collection => collection.Any()))
            {
                _productVariantService.Save(collection);
            }
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
                    ((Product) product).WasCancelled = true;
                    return;
                }
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductRepository(uow))
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
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductRepository(uow))
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
        /// Gets a Product by its unique id - pk
        /// </summary>
        /// <param name="key">Guid key for the Product</param>
        /// <returns><see cref="IProductVariant"/></returns>
        public override IProduct GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
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
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetPage(page, itemsPerPage, null, ValidateSortByField(sortBy), sortDirection);
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
        internal override int Count(IQuery<IProduct> query)
        {
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
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
            using (var repository = _repositoryFactory.CreateProductVariantRepository(_uowProvider.GetUnitOfWork()))
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
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
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
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.SearchKeys(searchTerm, page, itemsPerPage, ValidateSortByField(sortBy), sortDirection);
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
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
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
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
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
        /// Gets all the products
        /// </summary>
        /// <returns>
        /// A collection of all <see cref="IProduct"/>.
        /// </returns>
        public IEnumerable<IProduct> GetAll()
        {
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }
    }
}