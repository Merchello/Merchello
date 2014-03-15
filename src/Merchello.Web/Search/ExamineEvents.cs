using System;
using System.Globalization;
using System.Linq;
using Examine;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine;
using Merchello.Examine.Providers;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Web.Search
{
    /// <summary>
    /// Used to wire up events for Examine
    /// </summary>
    public class ExamineEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<ExamineEvents>("Initializing Merchello ProductIndex binding events");

            // Merchello registered providers
            var registeredProviders =
                ExamineManager.Instance.IndexProviderCollection.OfType<BaseMerchelloIndexer>()
                    .Count(x => x.EnableDefaultEventHandler);

            if(registeredProviders == 0)
                return;

            ProductService.Created += ProductServiceCreated;
            ProductService.Saved += ProductServiceSaved;
            ProductService.Deleted += ProductServiceDeleted;

            ProductVariantService.Created += ProductVariantServiceCreated;
            ProductVariantService.Saved += ProductVariantServiceSaved;
            ProductVariantService.Deleted += ProductVariantServiceDeleted;

            InvoiceService.Created += InvoiceServiceCreated;
            InvoiceService.Saved += InvoiceServiceSaved;
            InvoiceService.Deleted += InvoiceServiceDeleted;

            OrderService.Created += OrderServiceCreated;
            OrderService.Saved += OrderServiceSaved;
            OrderService.Deleted += OrderServiceDeleted;
        }

    
        #region Product

        /// <summary>
        /// Adds a product with all of its variants to the index if the product has an identity
        /// </summary>
        static void ProductServiceCreated(IProductService sender, Core.Events.NewEventArgs<IProduct> e)
        {
            if(e.Entity.HasIdentity) IndexProduct(e.Entity);
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
            if(e.Entity.HasIdentity) IndexProductVariant(e.Entity);
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
            ProductIndexer.AddProductToIndex(product);
        }

        
        private static void IndexProductVariant(IProductVariant productVariant)
        {
            ProductIndexer.ReIndexNode(productVariant.SerializeToXml().Root, IndexTypes.ProductVariant);
        }

        private static void DeleteProductFromIndex(IProduct product)
        {
            ProductIndexer.DeleteProductFromIndex(product);    
        }

        private static void DeleteProductVariantFromIndex(IProductVariant productVariant)
        {
            ProductIndexer.DeleteFromIndex(((ProductVariant)productVariant).ExamineId.ToString(CultureInfo.InvariantCulture));
        }

#endregion


        private void InvoiceServiceCreated(IInvoiceService sender, Core.Events.NewEventArgs<IInvoice> e)
        {
            if (e.Entity.HasIdentity) IndexInvoice(e.Entity);
        }

        private void InvoiceServiceSaved(IInvoiceService sender, SaveEventArgs<IInvoice> e)
        {
            e.SavedEntities.ForEach(IndexInvoice);
        }

        private void InvoiceServiceDeleted(IInvoiceService sender, DeleteEventArgs<IInvoice> e)
        {
            e.DeletedEntities.ForEach(DeleteInvoiceFromIndex);
        }

        private void IndexInvoice(IInvoice entity)
        {
            if(entity != null) InvoiceIndexer.AddInvoiceToIndex(entity);
        }

        private static void DeleteInvoiceFromIndex(IInvoice invoice)
        {
            InvoiceIndexer.DeleteFromIndex(((Invoice)invoice).ExamineId.ToString());
        }

        private void OrderServiceCreated(IOrderService sender, Core.Events.NewEventArgs<IOrder> e)
        {
            if (e.Entity.HasIdentity) IndexInvoice(InvoiceToReIndex(e.Entity.InvoiceKey));
        }

        private void OrderServiceDeleted(IOrderService sender, DeleteEventArgs<IOrder> e)
        {
            foreach (var order in e.DeletedEntities)
            {
                IndexInvoice(InvoiceToReIndex(order.InvoiceKey));
            }
        }

        private void OrderServiceSaved(IOrderService sender, SaveEventArgs<IOrder> e)
        {
            foreach (var order in e.SavedEntities)
            {
                IndexInvoice(InvoiceToReIndex(order.InvoiceKey));
            }
        }


        private static IInvoice InvoiceToReIndex(Guid key)
        {
            return MerchelloContext.Current.Services.InvoiceService.GetByKey(key);
        }



        #region Indexers


        private static ProductIndexer ProductIndexer
        {
            get { return (ProductIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"]; }
        }

        private static InvoiceIndexer InvoiceIndexer
        {
            get { return (InvoiceIndexer) ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"]; }
        }

        #endregion
    }
}