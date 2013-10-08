﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Umbraco.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Product Service 
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IProductVariantService _productVariantService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public ProductService()
            : this(new RepositoryFactory(), new ProductVariantService())
        { }

        public ProductService(RepositoryFactory repositoryFactory, IProductVariantService productVariantService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, productVariantService)
        { }

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

        #region IProductService Members

        /// <summary>
        /// Creates a Product without saving it to the database
        /// </summary>
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
        /// <param name="name"></param>
        /// <param name="sku"></param>
        /// <param name="price"></param>
        /// <returns></returns>
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
                    ((Product) product).WasCancelled = true;
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
            //foreach (var collection in productArray.Select(x => x.ProductVariants))
            //{
            //    _productVariantService.Save(collection);
            //}
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
        public IProduct GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a list of Product give a list of unique keys
        /// </summary>
        /// <param name="keys">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IProduct> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        public bool SkuExists(string sku)
        {
            return _productVariantService.SkuExists(sku);
        }

        #endregion

        internal IEnumerable<IProduct> GetAll()
        {
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
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
     
    }
}