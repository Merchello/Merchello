﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the ProductVariantService
    /// </summary>
    public class ProductVariantService : IProductVariantService
    {

        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
 
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public ProductVariantService()
            : this(new RepositoryFactory())
        { }

        public ProductVariantService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public ProductVariantService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="product"><see cref="IProduct"/></param>
        /// <param name="attributes"><see cref="IProductVariant"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        public IProductVariant CreateVariantWithKey(IProduct product, ProductAttributeCollection attributes, bool raiseEvents = true)
        {
            var skuSeparator = MerchelloConfiguration.Current.DefaultSkuSeparator;

            // verify the order of the attributes so that a sku can be constructed in the same order as the UI
            var optionIds = product.ProductOptionsForAttributes(attributes).OrderBy(x => x.SortOrder).Select(x => x.Id).Distinct();

            // the base sku
            var sku = product.Sku;
            var name = string.Format("{0} - ", product.Name);

            foreach (var att in optionIds.Select(id => attributes.FirstOrDefault(x => x.OptionId == id)).Where(att => att != null))
            {
                name += att.Name + " ";
                sku += skuSeparator + att.Sku;
            }

            return CreateVariantWithKey(product, name.Trim(), sku, product.Price, attributes, raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="product"><see cref="IProduct"/></param>
        /// <param name="name">The name of the product variant</param>
        /// <param name="sku">The unique sku of the product variant</param>
        /// <param name="price">The price of the product variant</param>
        /// <param name="attributes"><see cref="IProductVariant"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        public IProductVariant CreateVariantWithKey(IProduct product, string name, string sku, decimal price, ProductAttributeCollection attributes, bool raiseEvents = true)
        {
            Mandate.ParameterNotNull(product, "product");
            Mandate.ParameterNotNull(attributes, "attributes");
            Mandate.ParameterCondition(attributes.Count == product.ProductOptions.Count(x => x.Required), "An attribute must be assigned for every required option");            
            // verify there is not already a variant with these attributes
            Mandate.ParameterCondition(false == ProductVariantWithAttributesExists(product, attributes), "A ProductVariant already exists for the ProductAttributeCollection");

            var productVariant = new ProductVariant(product.Key, attributes, name, sku, price)
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
                TrackInventory = product.TrackInventory,
                OutOfStockPurchase = product.OutOfStockPurchase,
                Taxable = product.Taxable,
                Shippable = product.Shippable,
                Download = product.Download
            };

            if(raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IProductVariant>(productVariant), this))
            {
                productVariant.WasCancelled = true;
                return productVariant;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductVariantRepository(uow))
                {
                    repository.AddOrUpdate(productVariant);
                    uow.Commit();
                }
            }

            if(raiseEvents)
            Created.RaiseEvent(new Events.NewEventArgs<IProductVariant>(productVariant), this);

            product.ProductVariants.Add(productVariant);

            return productVariant;
        }

        /// <summary>
        /// Saves a single instance of a <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariant"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IProductVariant productVariant, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IProductVariant>(productVariant), this))
            {
                ((ProductVariant)productVariant).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductVariantRepository(uow))
                {
                    repository.AddOrUpdate(productVariant);
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Saving.IsRaisedEventCancelled(new SaveEventArgs<IProductVariant>(productVariant), this);
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
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductVariantRepository(uow))
                {
                    foreach (var variant in productVariants)
                    {
                        repository.AddOrUpdate(variant);
                    }
                    uow.Commit();
                }
            }

            if(raiseEvents)
            Saved.RaiseEvent(new SaveEventArgs<IProductVariant>(productVariants), this);
        }

        /// <summary>
        /// Ensures that all <see cref="IProductVariant"/> except the "master" variant for the <see cref="IProduct"/> have attributes
        /// </summary>
        /// <param name="product"><see cref="IProduct"/> to varify</param>
        public void EnsureProductVariantsHaveAttributes(IProduct product)
        {
            var variants = GetByProductKey(product.Key);
            var productVariants = variants as IProductVariant[] ?? variants.ToArray();
            if (!productVariants.Any()) return;
            foreach (var variant in productVariants.Where(variant => !variant.Attributes.Any()))
            {
                Delete(variant);
                product.ProductVariants.Remove(variant.Sku);
            }            
        }

        /// <summary>
        /// Ensures that every <see cref="IProductVariant"/> for every <see cref="IProduct"/> (except it's master variant) in the collection has related <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="productList">The collection of <see cref="IProduct"/> to ensure</param>
        public void EnsureProductVariantsHaveAttributes(IEnumerable<IProduct> productList)
        {
            foreach(var p in productList) EnsureProductVariantsHaveAttributes(p);
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
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductVariantRepository(uow))
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
        /// <param name="productVariantList">The collction of <see cref="IProductVariant"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IProductVariant> productVariantList, bool raiseEvents = true)
        {
            var productVariants = productVariantList as IProductVariant[] ?? productVariantList.ToArray();

            if (raiseEvents) 
            Deleting.RaiseEvent(new DeleteEventArgs<IProductVariant>(productVariants), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateProductVariantRepository(uow))
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
        /// <param name="key">Guid key of the Product to retrieve</param>
        /// <returns><see cref="IProductVariant"/></returns>
        public IProductVariant GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateProductVariantRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets list of <see cref="IProductVariant"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid keys for ProductVariant objects to retrieve</param>
        /// <returns>List of <see cref="IProduct"/></returns>
        public IEnumerable<IProductVariant> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateProductVariantRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> object for a given Product Key
        /// </summary>
        /// <param name="productKey">Guid product key of the <see cref="IProductVariant"/> collection to retrieve</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        public IEnumerable<IProductVariant> GetByProductKey(Guid productKey)
        {
            using (var repository = _repositoryFactory.CreateProductVariantRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetByProductKey(productKey);
            }
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
            using (var repository = _repositoryFactory.CreateProductVariantRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.ProductVariantWithAttributesExists(product, attributes);
            }
        }

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        public bool SkuExists(string sku)
        {
            using (var repository = _repositoryFactory.CreateProductVariantRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.SkuExists(sku);
            }
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
    }
}