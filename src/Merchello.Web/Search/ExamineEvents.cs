namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
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

            ShipmentService.UpdatedOrder += ShipmentServiceOnUpdatedOrder;

            CustomerService.Created += CustomerServiceCreated;
            CustomerService.Saved += CustomerServiceSaved;
            CustomerService.Deleted += CustomerServiceDeleted;

            //Add event so we can inject an extra field for media items
            ProductIndexer.GatheringNodeData += ProductIndexer_GatheringNodeData;

            //CustomerAddressService.Saved += CustomerAddressServiceSaved;
            //CustomerAddressService.Deleted += CustomerAddressServiceDeleted;
        }

        /// <summary>
        /// Used after we have gathered all information on a product/productvariant so we can inject extra fields into the index only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProductIndexer_GatheringNodeData(object sender, IndexingNodeDataEventArgs e)
        {
            var value = "-1";

            if (e.Fields.ContainsKey("downloadMediaId"))
            {
                int id;
                if (int.TryParse(e.Fields["downloadMediaId"], out id))
                {
                    if (ApplicationContext.Current != null && id > 0)
                    {
                        var mediaItem = ApplicationContext.Current.Services.MediaService.GetById(id);
                        if (mediaItem != null)
                        {
                            value = string.Join(" ", mediaItem.Properties.Select(x => x.Id.ToString(CultureInfo.InvariantCulture)));
                        }
                    }
                }
            }

            e.Fields.Add("downloadMediaPropertyIds", value);
        }

        // TODO RSS - come up with another way of updating the customer index ... we should not need to requiry the customer here

        /// <summary>
        /// The customer address service deleted.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="deleteEventArgs">
        /// The delete event args.
        /// </param>
        public void CustomerAddressServiceDeleted(ICustomerAddressService sender, DeleteEventArgs<ICustomerAddress> deleteEventArgs)
        {
            ReIndexCustomers(deleteEventArgs.DeletedEntities.Select(x => x.Key));
        }

        /// <summary>
        /// The customer address service saved.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        public void CustomerAddressServiceSaved(ICustomerAddressService sender, SaveEventArgs<ICustomerAddress> saveEventArgs)
        {
            this.ReIndexCustomers(saveEventArgs.SavedEntities.Select(x => x.Key));
        }

        /// <summary>
        /// The re index customers.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        private void ReIndexCustomers(IEnumerable<Guid> keys)
        {
            if (MerchelloContext.Current == null) return;
            
            var customers = MerchelloContext.Current.Services.CustomerService.GetByKeys(keys);

            customers.ForEach(IndexCustomer);
        }

        #region Customers

        /// <summary>
        /// The customer service deleted.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The delete event args.
        /// </param>
        public void CustomerServiceDeleted(ICustomerService sender, DeleteEventArgs<ICustomer> args)
        {
            args.DeletedEntities.ForEach(DeleteCustomerFromIndex);
        }

        /// <summary>
        /// The customer service saved.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void CustomerServiceSaved(ICustomerService sender, SaveEventArgs<ICustomer> args)
        {
            args.SavedEntities.ForEach(IndexCustomer);
        }

        /// <summary>
        /// The customer service created.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void CustomerServiceCreated(ICustomerService sender, Core.Events.NewEventArgs<ICustomer> args)
        {
            IndexCustomer(args.Entity);
        }

        /// <summary>
        /// Indexes a customer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        public static void IndexCustomer(ICustomer customer)
        {
            if (customer != null && customer.HasIdentity) CustomerIndexer.AddCustomerToIndex(customer);
        }

        /// <summary>
        /// The delete customer from index.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        public static void DeleteCustomerFromIndex(ICustomer customer)
        {
            if (customer != null && customer.HasIdentity) CustomerIndexer.DeleteCustomerFromIndex(customer);
        }

#endregion

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
        private void ShipmentServiceOnUpdatedOrder(IShipmentService sender, SaveEventArgs<IOrder> e)
        {
            e.SavedEntities.ForEach(IndexOrder);
        }

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