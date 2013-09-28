using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net.Core;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Events;
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

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public ProductService()
            : this(new RepositoryFactory())
        { }

        public ProductService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public ProductService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IProductService Members

        /// <summary>
        /// Creates a Product
        /// </summary>
        public IProductActual CreateProduct(string sku, string name, decimal price)
        {
            var product = new ProductActual()
            {
                Sku = sku,
                Name = name,
                Price = price,
                CostOfGoods = null,
                SalePrice = null,
                Weight = null,
                Length = null,
                Width = null,
                Height = null,
                Taxable = true,
                Shippable = false,
                Download = false,
                Template = false
            };

            Created.RaiseEvent(new NewEventArgs<IProductActual>(product), this);

            return product;
        }

        /// <summary>
        /// Saves a single <see cref="IProductActual"/> object
        /// </summary>
        /// <param name="productActual">The <see cref="IProductActual"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IProductActual productActual, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IProductActual>(productActual), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductRepository(uow))
                {
                    repository.AddOrUpdate(productActual);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IProductActual>(productActual), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IProductActual"/> objects.
        /// </summary>
        /// <param name="productList">Collection of <see cref="ProductActual"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IProductActual> productList, bool raiseEvents = true)
        {
            var productArray = productList as IProductActual[] ?? productList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IProductActual>(productArray), this);

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

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IProductActual>(productArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IProductActual"/> object
        /// </summary>
        /// <param name="productActual">The <see cref="IProductActual"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IProductActual productActual, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IProductActual>(productActual), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductRepository(uow))
                {
                    repository.Delete(productActual);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IProductActual>(productActual), this);
        }


        /// <summary>
        /// Deletes a collection <see cref="IProductActual"/> objects
        /// </summary>
        /// <param name="productList">Collection of <see cref="IProductActual"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IProductActual> productList, bool raiseEvents = true)
        {
            var productArray = productList as IProductActual[] ?? productList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IProductActual>(productArray), this);

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

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IProductActual>(productArray), this);
        }

        /// <summary>
        /// Gets a Product by its unique id - pk
        /// </summary>
        /// <param name="key">Guid key for the Product</param>
        /// <returns><see cref="IProductActual"/></returns>
        public IProductActual GetByKey(Guid key)
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
        public IEnumerable<IProductActual> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        #endregion

        internal IEnumerable<IProductActual> GetAll()
        {
            using (var repository = _repositoryFactory.CreateProductRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IProductService, DeleteEventArgs<IProductActual>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IProductService, DeleteEventArgs<IProductActual>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IProductService, SaveEventArgs<IProductActual>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IProductService, SaveEventArgs<IProductActual>> Saved;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IProductService, NewEventArgs<IProductActual>> Created;

        #endregion


     
    }
}