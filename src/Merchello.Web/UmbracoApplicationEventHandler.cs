namespace Merchello.Web
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using log4net;
    using Core;
    using Core.Configuration;
    using Core.Events;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Sales;
    using Core.Services;

    using Merchello.Core.Checkout;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Persistence.Migrations;
    using Merchello.Core.Persistence.Migrations.Initial;
    using Merchello.Web.Routing;
    using Merchello.Web.Search;
    using Merchello.Web.Search.Provisional;
    using Merchello.Web.Workflow;

    using Models.SaleHistory;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Routing;

    using ServiceContext = Merchello.Core.Services.ServiceContext;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Handles the Umbraco Application "Starting" and "Started" event and initiates the Merchello startup
    /// </summary>
    public class UmbracoApplicationEventHandler : ApplicationEventHandler
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// The _merchello is started.
        /// </summary>
        private static bool merchelloIsStarted = false;

        /// <summary>
        /// The application initialized.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The <see cref="UmbracoApplicationBase"/>.
        /// </param>
        /// <param name="applicationContext">
        /// The <see cref="ApplicationContext"/>.
        /// </param>
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationInitialized(umbracoApplication, applicationContext);
        }

        /// <summary>
        /// The Umbraco Application Starting event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            BootManagerBase.MerchelloStarted += BootManagerBaseOnMerchelloStarted;

            try
            {
                // Initialize Merchello
                Log.Info("Attempting to initialize Merchello");
                MerchelloBootstrapper.Init(new WebBootManager());
                Log.Info("Initialization of Merchello complete");                
            }
            catch (Exception ex)
            {
                Log.Error("Initialization of Merchello failed", ex);
            }

            this.RegisterContentFinders();
        }

        /// <summary>
        /// The Umbraco Application Starting event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            Core.CoreBootManager.FinalizeBoot();

            MultiLogHelper.Info<UmbracoApplicationEventHandler>("Initializing Customer related events");

            MemberService.Saving += this.MemberServiceOnSaving;

            SalePreparationBase.Finalizing += SalePreparationBaseOnFinalizing;
            CheckoutPaymentManagerBase.Finalizing += CheckoutPaymentManagerBaseOnFinalizing;

            InvoiceService.Deleted += InvoiceServiceOnDeleted;
            OrderService.Deleted += OrderServiceOnDeleted;

            // Store settings
            StoreSettingService.Saved += StoreSettingServiceOnSaved;

            // Clear the tax method if set
            TaxMethodService.Saved += TaxMethodServiceOnSaved;

            // Auditing
            PaymentGatewayMethodBase.VoidAttempted += PaymentGatewayMethodBaseOnVoidAttempted;

            ShipmentService.StatusChanged += ShipmentServiceOnStatusChanged;

            // Basket conversion
            BasketConversionBase.Converted += OnBasketConverted;

            // Detached Content
            DetachedContentTypeService.Deleting += DetachedContentTypeServiceOnDeleting;

            ProductService.AddedToCollection += ProductServiceAddedToCollection;
            ProductService.RemovedFromCollection += ProductServiceRemovedFromCollection;
            ProductService.Deleted += ProductServiceDeleted;

            EntityCollectionService.Saved += EntityCollectionSaved;
            EntityCollectionService.Deleted += EntityCollectionDeleted;

            if (merchelloIsStarted) this.VerifyMerchelloVersion();
        }

        private void EntityCollectionSaved(IEntityCollectionService sender, SaveEventArgs<Core.Models.Interfaces.IEntityCollection> e)
        {
            var merchello = new MerchelloHelper();
            ((ProductFilterGroupQuery)merchello.Filters.Product).ClearFilterTreeCache();
        }

        private void EntityCollectionDeleted(IEntityCollectionService sender, DeleteEventArgs<Core.Models.Interfaces.IEntityCollection> e)
        {
            var merchello = new MerchelloHelper();
            ((ProductFilterGroupQuery)merchello.Filters.Product).ClearFilterTreeCache();
        }

        private void ProductServiceDeleted(IProductService sender, DeleteEventArgs<IProduct> e)
        {
            var merchello = new MerchelloHelper();
            ((ProductFilterGroupQuery)merchello.Filters.Product).ClearFilterTreeCache();
        }   

        private void ProductServiceRemovedFromCollection(object sender, EventArgs e)
        {
            var merchello = new MerchelloHelper();
            ((ProductFilterGroupQuery)merchello.Filters.Product).ClearFilterTreeCache();
        }

        private void ProductServiceAddedToCollection(object sender, EventArgs e)
        {
            var merchello = new MerchelloHelper();
            ((ProductFilterGroupQuery)merchello.Filters.Product).ClearFilterTreeCache();
        }

        /// <summary>
        /// Updates the customer's last activity date after the basket has been converted
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnBasketConverted(BasketConversionBase sender, ConvertEventArgs<BasketConversionPair> e)
        {
            foreach (var pair in e.CovertedEntities.ToArray())
            {
                var customer = (ICustomer)pair.CustomerBasket.Customer;
                customer.LastActivityDate = DateTime.Now;
                MerchelloContext.Current.Services.CustomerService.Save(customer);
            }
        }

        /// <summary>
        /// Registers Merchello content finders.
        /// </summary>
        private void RegisterContentFinders()
        {
            //// We want the product content finder to execute after Umbraco's content finders since
            //// we may ultimately rely on a database query as a fallback to when something is not found in the
            //// examine index.  If we simply did an InsertType, we would be executing a worthless query for each time
            //// a legitament Umbraco content was rendered.
            var contentFinderByIdPathIndex = ContentFinderResolver.Current.GetTypes().IndexOf(typeof(ContentFinderByIdPath));

            ContentFinderResolver.Current.InsertType<ContentFinderProductBySlug>(contentFinderByIdPathIndex + 1);
        }


        /// <summary>
        /// The detached content type service on deleting.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DetachedContentTypeServiceOnDeleting(IDetachedContentTypeService sender, DeleteEventArgs<IDetachedContentType> e)
        {
            foreach (var dc in e.DeletedEntities)
            {
                // remove detached content from products
                var products = MerchelloContext.Current.Services.ProductService.GetByDetachedContentType(dc.Key);
                MerchelloContext.Current.Services.ProductService.RemoveDetachedContent(products, dc.Key);
            }
        }

        /// <summary>
        /// Clears the product pricing tax method so that it can be re-initialized if needed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private void TaxMethodServiceOnSaved(ITaxMethodService sender, SaveEventArgs<ITaxMethod> saveEventArgs)
        {
            ((TaxationContext)MerchelloContext.Current.Gateways.Taxation).ClearProductPricingMethod();
        }

        /// <summary>
        /// The boot manager base on merchello started.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void BootManagerBaseOnMerchelloStarted(object sender, EventArgs eventArgs)
        {
            merchelloIsStarted = true;
        }

        #region Shipment Audits

        /// <summary>
        /// Handles the <see cref="ShipmentService"/> status changed
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args
        /// </param>
        private void ShipmentServiceOnStatusChanged(IShipmentService sender, StatusChangeEventArgs<IShipment> e)
        {
            foreach (var shipment in e.StatusChangedEntities)
            {
                shipment.AuditStatusChanged();
            }
        }



        #endregion

        #region Payment Audits

        /// <summary>
        /// Handles the <see cref="PaymentGatewayMethodBase"/> Void Attempted
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args
        /// </param>
        private void PaymentGatewayMethodBaseOnVoidAttempted(PaymentGatewayMethodBase sender, PaymentAttemptEventArgs<IPaymentResult> e)
        {
            if (e.Entity.Payment.Success) e.Entity.Payment.Result.AuditPaymentVoided();
        }

        #endregion

        /// <summary>
        /// Handles the <see cref="InvoiceService"/> Deleted event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="deleteEventArgs">
        /// The delete event args.
        /// </param>
        private void InvoiceServiceOnDeleted(IInvoiceService sender, DeleteEventArgs<IInvoice> deleteEventArgs)
        {
            Task.Factory.StartNew(
            () =>
            {
                foreach (var invoice in deleteEventArgs.DeletedEntities)
                {
                    try
                    {
                        invoice.AuditDeleted();
                    }
                    catch (Exception ex)
                    {
                        MultiLogHelper.Error<UmbracoApplicationEventHandler>("Failed to log invoice deleted", ex);
                    }
                }
            });
        }

        /// <summary>
        /// Handles the <see cref="OrderService"/> Deleted event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="deleteEventArgs">
        /// The delete event args.
        /// </param>
        private void OrderServiceOnDeleted(IOrderService sender, DeleteEventArgs<IOrder> deleteEventArgs)
        {
            Task.Factory.StartNew(
            () =>
            {
                foreach (var order in deleteEventArgs.DeletedEntities)
                {
                    try
                    {
                        order.AuditDeleted();
                    }
                    catch (Exception ex)
                    {
                        MultiLogHelper.Error<UmbracoApplicationEventHandler>("Failed to log order deleted", ex);
                    }
                }
            });
        }

        /// <summary>
        /// The store setting service on saved.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void StoreSettingServiceOnSaved(IStoreSettingService sender, SaveEventArgs<IStoreSetting> e)
        {
            var setting = e.SavedEntities.FirstOrDefault(x => x.Key == Core.Constants.StoreSettingKeys.GlobalTaxationApplicationKey);
            if (setting == null) return;

            var taxationContext = (TaxationContext)MerchelloContext.Current.Gateways.Taxation;
            taxationContext.ClearProductPricingMethod();
            if (setting.Value == "Product") return;
            
            var taxMethodService = ((ServiceContext)MerchelloContext.Current.Services).TaxMethodService;
            var methods = taxMethodService.GetAll().ToArray();
            foreach (var method in methods)
            {
                method.ProductTaxMethod = false;
            }

            taxMethodService.Save(methods);
        }

        /// <summary>
        /// Performs audits on SalePreparationBase.Finalizing
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="salesPreparationEventArgs">
        /// The sales preparation event args.
        /// </param>
        /// TODO RSS remove this when SalePreparation is removed
        [Obsolete]
        private void SalePreparationBaseOnFinalizing(SalePreparationBase sender, SalesPreparationEventArgs<IPaymentResult> salesPreparationEventArgs)
        {
            var result = salesPreparationEventArgs.Entity;

            result.Invoice.AuditCreated();

            if (result.Payment.Success)
            {
                if (result.Invoice.InvoiceStatusKey == Core.Constants.DefaultKeys.InvoiceStatus.Paid)
                {
                    result.Payment.Result.AuditPaymentCaptured(result.Payment.Result.Amount);
                }
                else
                {
                    result.Payment.Result.AuditPaymentAuthorize(result.Invoice);
                }
            }
            else
            {
                result.Payment.Result.AuditPaymentDeclined();
            }
        }

        /// <summary>
        /// The checkout payment manager base on finalizing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckoutPaymentManagerBaseOnFinalizing(CheckoutPaymentManagerBase sender, CheckoutEventArgs<IPaymentResult> e)
        {
            var result = e.Item;

            result.Invoice.AuditCreated();

            if (result.Payment.Success)
            {
                // Reset the Customer's CheckoutManager
                e.Customer.Basket().GetCheckoutManager().Reset();

                if (result.Invoice.InvoiceStatusKey == Core.Constants.DefaultKeys.InvoiceStatus.Paid)
                {
                    result.Payment.Result.AuditPaymentCaptured(result.Payment.Result.Amount);
                }
                else
                {
                    result.Payment.Result.AuditPaymentAuthorize(result.Invoice);
                }
            }
            else
            {
                result.Payment.Result.AuditPaymentDeclined();
            }
        }

        /// <summary>
        /// Handles the member saving event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        /// <remarks>
        /// Merchello customers are associated with Umbraco members by their username.  If the 
        /// member changes their username we have to update the association.
        /// </remarks>
        private void MemberServiceOnSaving(IMemberService sender, SaveEventArgs<IMember> saveEventArgs)
        {
            foreach (var member in saveEventArgs.SavedEntities)
            {
                if (MerchelloConfiguration.Current.CustomerMemberTypes.Any(x => x == member.ContentTypeAlias))
                {
                    var original = ApplicationContext.Current.Services.MemberService.GetByKey(member.Key);

                    var customerService = MerchelloContext.Current.Services.CustomerService;

                    ICustomer customer;
                    if (original == null)
                    {
                        // assert there is not already a customer with the login name
                        customer = customerService.GetByLoginName(member.Username);

                        if (customer != null)
                        {
                            MultiLogHelper.Info<UmbracoApplicationEventHandler>("A customer already exists with the loginName of: " + member.Username + " -- ABORTING customer creation");
                            return;
                        }

                        customerService.CreateCustomerWithKey(member.Username, string.Empty, string.Empty, member.Email);

                        return;
                    }

                    if (original.Username == member.Username && original.Email == member.Email) return;

                    customer = customerService.GetByLoginName(original.Username);

                    if (customer == null) return;

                    ((Customer)customer).LoginName = member.Username;
                    customer.Email = member.Email;

                    customerService.Save(customer);
                }
            }
        }

        /// <summary>
        /// Verifies that the Merchello Version (binary) is consistent with the configuration version.
        /// </summary>
        /// <remarks>
        /// This process also does database schema migrations (for Merchello) if necessary
        /// </remarks>
        private void VerifyMerchelloVersion()
        {
            LogHelper.Info<UmbracoApplicationEventHandler>("Verifying Merchello Version.");
            var manager = new WebMigrationManager();
            manager.Upgraded += MigrationManagerOnUpgraded;
            manager.EnsureMerchelloVersion();
        }


        /// <summary>
        /// The migration manager on upgraded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The merchello migration event args.
        /// </param>
        private async void MigrationManagerOnUpgraded(object sender, MerchelloMigrationEventArgs e)
        {
            var response = await ((WebMigrationManager)sender).PostAnalyticInfo(e.MigrationRecord);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var ex = new Exception(response.ReasonPhrase);
                MultiLogHelper.Error(typeof(UmbracoApplicationEventHandler), "Failed to record Merchello Migration Record", ex);
            }
        }
    }
}
