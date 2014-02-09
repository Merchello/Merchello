using System;
using System.Globalization;
using System.Linq;
using Examine;
using Merchello.Core;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine;
using Merchello.Web;
using Merchello.Web.Workflow;
using NUnit.Framework;
using Rhino.Mocks;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    public abstract class MerchelloAllInTestBase
    {
        protected ICustomerBase CurrentCustomer;
        protected DbPreTestDataWorker DbPreTestDataWorker;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            // Sets Umbraco SqlSytax and ensure database is setup
            DbPreTestDataWorker = new DbPreTestDataWorker();
            DbPreTestDataWorker.ValidateDatabaseSetup();

            // Merchello CoreBootStrap
            var bootManager = new WebBootManager();
            bootManager.Initialize();

            if(MerchelloContext.Current == null) Assert.Ignore("MerchelloContext.Current is null");

            CurrentCustomer = DbPreTestDataWorker.MakeExistingAnonymousCustomer();

            // Product saves

            ProductService.Created += ProductServiceCreated;
            ProductService.Saved += ProductServiceSaved;
            ProductService.Deleted += ProductServiceDeleted;
            ProductVariantService.Created += ProductVariantServiceCreated;
            ProductVariantService.Saved += ProductVariantServiceSaved;
            ProductVariantService.Deleted += ProductVariantServiceDeleted;

            // BasketCheckout 
            ItemCacheService.Created += BasketItemCacheCreated;
            ItemCacheService.Saved += BasketItemCacheSaved;


        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            MerchelloContext.Current.Dispose();

            ProductService.Created -= ProductServiceCreated;
            ProductService.Saved -= ProductServiceSaved;
            ProductService.Deleted -= ProductServiceDeleted;
            ProductVariantService.Created -= ProductVariantServiceCreated;
            ProductVariantService.Saved -= ProductVariantServiceSaved;
            ProductVariantService.Deleted -= ProductVariantServiceDeleted;

            // BasketCheckout 
            ItemCacheService.Created -= BasketItemCacheCreated;
            ItemCacheService.Saved -= BasketItemCacheSaved;
            
        }

        #region BasketCheckoutEvents


        /// <summary>
        /// Purges customer <see cref="BasketCheckoutPreparation"/> information on customer <see cref="IBasket"/> creation
        /// </summary>
        static void BasketItemCacheCreated(IItemCacheService sender, Core.Events.NewEventArgs<IItemCache> e)
        {
            if (e.Entity.ItemCacheType != ItemCacheType.Basket) return;
            ClearCheckoutItemCache(e.Entity.EntityKey);
        }

        /// <summary>
        /// Purges customer <see cref="BasketCheckoutPreparation"/> information on customer <see cref="IBasket"/> saves.  The will
        /// also handle the Basket items saves & deletes
        /// </summary>
        static void BasketItemCacheSaved(IItemCacheService sender, SaveEventArgs<IItemCache> e)
        {
            foreach (var item in e.SavedEntities.Where(item => item.ItemCacheType == ItemCacheType.Basket))
            {
                ClearCheckoutItemCache(item.EntityKey);
            }
        }


        private static void ClearCheckoutItemCache(Guid entityKey)
        {
            CheckoutPreparationBase.RestartCheckout(MerchelloContext.Current, entityKey);
        }

        #endregion

        #region ExamineEvents

        /// <summary>
        /// Adds a product with all of its variants to the index if the product has an identity
        /// </summary>
        static void ProductServiceCreated(IProductService sender, Core.Events.NewEventArgs<IProduct> e)
        {
            if (e.Entity.HasIdentity) IndexProduct(e.Entity);
        }

        /// <summary>
        /// Adds or updates a product with all of its variants to the index
        /// </summary>        
        static void ProductServiceSaved(IProductService sender, SaveEventArgs<IProduct> e)
        {
            e.SavedEntities.ForEach(IndexProduct);
        }

        /// <summary>
        /// Removes a product with all of its variants from the index
        /// </summary>   
        static void ProductServiceDeleted(IProductService sender, DeleteEventArgs<IProduct> e)
        {
            e.DeletedEntities.ForEach(DeleteProductFromIndex);
        }

        /// <summary>
        /// Adds or updates a product variant to the index if the product variant has an identity
        /// </summary>
        static void ProductVariantServiceCreated(IProductVariantService sender, Core.Events.NewEventArgs<IProductVariant> e)
        {
            if (e.Entity.HasIdentity) IndexProductVariant(e.Entity);
        }

        /// <summary>
        /// Adds or updates a product variant to the index
        /// </summary>
        static void ProductVariantServiceSaved(IProductVariantService sender, SaveEventArgs<IProductVariant> e)
        {
            e.SavedEntities.ForEach(IndexProductVariant);
        }

        /// <summary>
        /// Removes a product variant from the index
        /// </summary>
        static void ProductVariantServiceDeleted(IProductVariantService sender, DeleteEventArgs<IProductVariant> e)
        {
            e.DeletedEntities.ForEach(DeleteProductVariantFromIndex);
        }


        private static void IndexProduct(IProduct product)
        {
            product.ProductVariants.ForEach(IndexProductVariant);
            IndexProductVariant(((Product)product).MasterVariant, product.ProductOptions);
        }

        private static void IndexProductVariant(IProductVariant productVariant)
        {
            IndexProductVariant(productVariant, null);
        }

        private static void IndexProductVariant(IProductVariant productVariant, ProductOptionCollection productOptions)
        {
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].ReIndexNode(productVariant.SerializeToXml(productOptions).Root, IndexTypes.ProductVariant);
        }

        private static void DeleteProductFromIndex(IProduct product)
        {
            product.ProductVariants.ForEach(DeleteProductVariantFromIndex);
            DeleteProductVariantFromIndex(((Product)product).MasterVariant);
        }

        private static void DeleteProductVariantFromIndex(IProductVariant productVariant)
        {
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].DeleteFromIndex(((ProductVariant)productVariant).ExamineId.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}