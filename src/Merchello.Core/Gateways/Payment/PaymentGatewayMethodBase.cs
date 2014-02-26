using System;
using System.Linq;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Represents a base GatewayPaymentMethod 
    /// </summary>
    public abstract class PaymentGatewayMethodBase : IPaymentGatewayMethod
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IPaymentMethod _paymentMethod;

        protected PaymentGatewayMethodBase(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(paymentMethod, "paymentMethod");

            _gatewayProviderService = gatewayProviderService;
            _paymentMethod = paymentMethod;
        }

        /// <summary>
        /// Does the actual work of creating and processing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment. (Maybe a username, password or some Api Key)</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformProcessPayment(IInvoice invoice, ProcessorArgumentCollection args);

        /// <summary>
        /// Processes a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult ProcessPayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");
            

            // persist the invoice
            if(!invoice.HasIdentity)
                GatewayProviderService.Save(invoice);

            // collect the payment authorization
            var response = PerformProcessPayment(invoice, args);

            if (!response.Result.Success) return response;

            // Apply the payment to the invoice if it was not done in the sub class           
            var payment = response.Result.Result;
            if (payment.AppliedPayments(GatewayProviderService).FirstOrDefault(x => x.InvoiceKey == invoice.Key) == null)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, PaymentMethod.Name, payment.Amount);
            }

            // Check configuration for override on ApproveOrderCreation
            if (!response.ApproveOrderCreation)
                ((PaymentResult)response).ApproveOrderCreation = MerchelloConfiguration.Current.AlwaysApproveOrderCreation;

            // give response
            return response;
        }
        
        /// <summary>
        /// Gets the <see cref="IPaymentMethod"/>
        /// </summary>
        public IPaymentMethod PaymentMethod 
        {
            get { return _paymentMethod; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        protected IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }
    }
}