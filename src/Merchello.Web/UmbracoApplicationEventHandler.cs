using System.Runtime.InteropServices;

namespace Merchello.Web
{
    using System;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Core;
    using Core.Configuration;
    using Core.Events;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Sales;
    using Core.Services;
    using Models.SaleHistory;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
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

            // Initialize Merchello
            Log.Info("Attempting to initialize Merchello");
            try
            {
                MerchelloBootstrapper.Init(new WebBootManager());
                Log.Info("Initialization of Merchello complete");                
            }
            catch (Exception ex)
            {
                Log.Error("Initialization of Merchello failed", ex);
            }
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

            LogHelper.Info<UmbracoApplicationEventHandler>("Initializing Customer related events");

            MemberService.Saving += this.MemberServiceOnSaving;

            SalePreparationBase.Finalizing += SalePreparationBaseOnFinalizing;

            InvoiceService.Deleted += InvoiceServiceOnDeleted;
            OrderService.Deleted += OrderServiceOnDeleted;

            // Auditing
            PaymentGatewayMethodBase.VoidAttempted += PaymentGatewayMethodBaseOnVoidAttempted;

            ShipmentService.StatusChanged += ShipmentServiceOnStatusChanged;
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
                        LogHelper.Error<UmbracoApplicationEventHandler>("Failed to log invoice deleted", ex);
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
                        LogHelper.Error<UmbracoApplicationEventHandler>("Failed to log order deleted", ex);
                    }
                }
            });
        }


        /// <summary>
        /// Performs audits on SalePrepartionBase.Finalizing
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="salesPreparationEventArgs">
        /// The sales preparation event args.
        /// </param>
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
                            LogHelper.Info<UmbracoApplicationEventHandler>("A customer already exists with the loginName of: " + member.Username + " -- ABORTING customer creation");
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
    }
}
