using System;
using System.Globalization;
using Examine;
using Merchello.Core;
using Merchello.Core.Events;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Sales;
using Merchello.Core.Services;
using Merchello.Examine;
using Merchello.Web;
using Moq;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Web;
using WebBootManager = Merchello.Web.WebBootManager;

namespace Merchello.Tests.Base.TestHelpers
{
    public abstract class MerchelloAllInTestBase
    {
        protected ICustomerBase CurrentCustomer;
        protected DbPreTestDataWorker DbPreTestDataWorker;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            
            AutoMapperMappings.CreateMappings();  

            // Umbraco Application
            var applicationMock = new Mock<UmbracoApplication>();

            // Sets Umbraco SqlSytax and ensure database is setup
            DbPreTestDataWorker = new DbPreTestDataWorker();
            DbPreTestDataWorker.ValidateDatabaseSetup();
            DbPreTestDataWorker.DeleteAllAnonymousCustomers();

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
           // ItemCacheService.Saved += BasketItemCacheSaved;

            SalePreparationBase.Finalizing += SalePreparationBaseOnFinalizing;


        }

        private void SalePreparationBaseOnFinalizing(SalePreparationBase sender, SalesPreparationEventArgs<IPaymentResult> args)
        {
            var result = args.Entity;

            if (result.ApproveOrderCreation)
            {
                // order
                var order = result.Invoice.PrepareOrder(MerchelloContext.Current);

                MerchelloContext.Current.Services.OrderService.Save(order);
            }

            var customerKey = result.Invoice.CustomerKey;

            // Clean up the sales prepartation item cache
            if (customerKey == null || Guid.Empty.Equals(customerKey)) return;
            var customer = MerchelloContext.Current.Services.CustomerService.GetAnyByKey(customerKey.Value);

            if (customer == null) return;
            var itemCacheService = MerchelloContext.Current.Services.ItemCacheService;
            var itemCache = itemCacheService.GetItemCacheByCustomer(customer, ItemCacheType.Checkout);
            itemCacheService.Delete(itemCache);
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

            SalePreparationBase.Finalizing -= SalePreparationBaseOnFinalizing;
        }

        //#region BasketCheckoutEvents

        ///// <summary>
        ///// Purges customer <see cref="BasketCheckoutPreparation"/> information on customer <see cref="IBasket"/> saves.  The will
        ///// also handle the Basket items saves & deletes
        ///// </summary>
        //static void BasketItemCacheSaved(IItemCacheService sender, SaveEventArgs<IItemCache> e)
        //{
        //    foreach (var item in e.SavedEntities.Where(item => item.ItemCacheType == ItemCacheType.Basket))
        //    {
        //        CheckoutPreparationBase.RestartCheckout(MerchelloContext.Current, item.EntityKey);
        //    }
        //}

        //#endregion

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