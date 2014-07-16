﻿using System;

namespace Merchello.Web.Search
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;    
    using Core;
    using Core.Models;
    using Core.Services;
    using global::Examine;
    using Examine;
    using Examine.Providers;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Used to wire up events for Examine
    /// </summary>
    public class ExamineEvents : ApplicationEventHandler
    {

        #region Indexers

        /// <summary>
        /// Gets the product indexer.
        /// </summary>
        private static ProductIndexer ProductIndexer
        {
            get { return (ProductIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"]; }
        }

        /// <summary>
        /// Gets the invoice indexer.
        /// </summary>
        private static InvoiceIndexer InvoiceIndexer
        {
            get { return (InvoiceIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"]; }
        }

        /// <summary>
        /// Gets the order indexer.
        /// </summary>
        private static OrderIndexer OrderIndexer
        {
            get { return (OrderIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloOrderIndexer"]; }
        }

        /// <summary>
        /// Gets the customer indexer.
        /// </summary>
        private static CustomerIndexer CustomerIndexer
        {
            get { return (CustomerIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloCustomerIndexer"]; }
        }

        #endregion

        /// <summary>
        /// The application started.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<ExamineEvents>("Initializing Merchello ProductIndex binding events");

            // Merchello registered providers
            var registeredProviders =
                ExamineManager.Instance.IndexProviderCollection.OfType<BaseMerchelloIndexer>()
                    .Count(x => x.EnableDefaultEventHandler);

            if (registeredProviders == 0)
                return;

            ProductService.Created += ProductServiceCreated;
            ProductService.Saved += ProductServiceSaved;
            ProductService.Deleted += ProductServiceDeleted;

            ProductVariantService.Created += ProductVariantServiceCreated;
            ProductVariantService.Saved += ProductVariantServiceSaved;
            ProductVariantService.Deleted += ProductVariantServiceDeleted;

            InvoiceService.Saved += InvoiceServiceSaved;
            InvoiceService.Deleted += InvoiceServiceDeleted;

            OrderService.Saved += OrderServiceSaved;
            OrderService.Deleted += OrderServiceDeleted;

            CustomerService.Created += CustomerServiceCreated;
            CustomerService.Saved += CustomerServiceSaved;
            CustomerService.Deleted += CustomerServiceDeleted;
        }

        /// <summary>
        /// The customer service deleted.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="deleteEventArgs">
        /// The delete event args.
        /// </param>
        private void CustomerServiceDeleted(ICustomerService sender, DeleteEventArgs<ICustomer> deleteEventArgs)
        {
            
        }

        private void CustomerServiceSaved(ICustomerService sender, SaveEventArgs<ICustomer> saveEventArgs)
        {
          
        }

        private void CustomerServiceCreated(ICustomerService sender, Core.Events.NewEventArgs<ICustomer> newEventArgs)
        {
            
        }

        private static void IndexCustomer(ICustomer customer)
        {
            if (customer != null && customer.HasIdentity) CustomerIndexer.AddCustomerToIndex(customer);
        }

        #region Invoice

        /// <summary>
        /// Adds saved invoices to the index
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void InvoiceServiceSaved(IInvoiceService sender, SaveEventArgs<IInvoice> e)
        {
            e.SavedEntities.ForEach(IndexInvoice);
        }

        /// <summary>
        /// Removes deleted invoices from the index 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void InvoiceServiceDeleted(IInvoiceService sender, DeleteEventArgs<IInvoice> e)
        {
            e.DeletedEntities.ForEach(DeleteInvoiceFromIndex);
        }

        /// <summary>
        /// ReIndexes an Invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to be re-indexed</param>
        private static void IndexInvoice(IInvoice invoice)
        {
            if (invoice != null && invoice.HasIdentity) InvoiceIndexer.AddInvoiceToIndex(invoice);
        }



        /// <summary>
        /// Deletes an <see cref="IInvoice"/> from the index
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to be removed from the index</param>
        private static void DeleteInvoiceFromIndex(IInvoice invoice)
        {
            InvoiceIndexer.DeleteFromIndex(((Invoice)invoice).ExamineId.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region Order

        /// <summary>
        /// Reindexes an invoice based on order saved
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void OrderServiceSaved(IOrderService sender, SaveEventArgs<IOrder> e)
        {
            e.SavedEntities.ForEach(IndexOrder);
        }

        /// <summary>
        /// Reindexes an invoice based on order deletion 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void OrderServiceDeleted(IOrderService sender, DeleteEventArgs<IOrder> e)
        {
            e.DeletedEntities.ForEach(DeleteOrderFromIndex);
        }

        /// <summary>
        /// Indexes an order
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        private static void IndexOrder(IOrder order)
        {
            if (order != null && order.HasIdentity) OrderIndexer.AddOrderToIndex(order);
        }

        /// <summary>
        /// Deletes an order from the index
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        private static void DeleteOrderFromIndex(IOrder order)
        {
            OrderIndexer.DeleteFromIndex(((Order)order).ExamineId.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

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
    }
}