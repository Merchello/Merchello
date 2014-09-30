namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using Builders;
    using Formatters;
    using Gateways.Payment;
    using Gateways.Taxation;    
    using Newtonsoft.Json;
    using Services;
    using Umbraco.Core.Logging;
    using Formatting = Newtonsoft.Json.Formatting;    

    /// <summary>
    /// Extension methods for <see cref="IInvoice"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    public static class InvoiceExtensions
    {
        /// <summary>
        /// Returns a constructed invoice number (including it's invoice number prefix - if any)
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <returns>The prefixed invoice number</returns>
        public static string PrefixedInvoiceNumber(this IInvoice invoice)
        {
            return string.IsNullOrEmpty(invoice.InvoiceNumberPrefix)
                ? invoice.InvoiceNumber.ToString(CultureInfo.InvariantCulture)
                : string.Format("{0}-{1}", invoice.InvoiceNumberPrefix, invoice.InvoiceNumber);
        }

        /// <summary>
        /// Returns the currency code associated with the invoice
        /// </summary>
        /// <param name="invoice">The invoice</param>
        /// <returns>The currency code associated with the invoice</returns>
        public static string CurrencyCode(this IInvoice invoice)
        {
            var allCurrencyCodes =
                invoice.Items.Select(x => x.ExtendedData.GetValue(Constants.ExtendedDataKeys.CurrencyCode)).Distinct().ToArray();

            return allCurrencyCodes.Any() ? allCurrencyCodes.First() : string.Empty;
        }

        #region Address

        /// <summary>
        /// Utility extension method to add an <see cref="IAddress"/> to an <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to which to set the address information</param>
        /// <param name="address">The billing address <see cref="IAddress"/></param>
        public static void SetBillingAddress(this IInvoice invoice, IAddress address)
        {
            invoice.BillToName = address.Name;
            invoice.BillToCompany = address.Organization;
            invoice.BillToAddress1 = address.Address1;
            invoice.BillToAddress2 = address.Address2;
            invoice.BillToLocality = address.Locality;
            invoice.BillToRegion = address.Region;
            invoice.BillToPostalCode = address.PostalCode;
            invoice.BillToCountryCode = address.CountryCode;
            invoice.BillToPhone = address.Phone;
            invoice.BillToEmail = address.Email;
        }

        /// <summary>
        /// Utility extension to extract the billing <see cref="IAddress"/> from an <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice</param>
        /// <returns>
        /// The billing address saved in the invoice
        /// </returns>
        public static IAddress GetBillingAddress(this IInvoice invoice)
        {
            return new Address()
            {
                Name = invoice.BillToName,
                Organization = invoice.BillToCompany,
                Address1 = invoice.BillToAddress1,
                Address2 = invoice.BillToAddress2,
                Locality = invoice.BillToLocality,
                Region = invoice.BillToRegion,
                PostalCode = invoice.BillToPostalCode,
                CountryCode = invoice.BillToCountryCode,
                Phone = invoice.BillToPhone,
                Email = invoice.BillToEmail
            };
        }

        /// <summary>
        /// Gets a collection of <see cref="IReplaceablePattern"/> for the invoice
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <returns>
        /// The collection of replaceable patterns
        /// </returns>
        internal static IEnumerable<IReplaceablePattern> ReplaceablePatterns(this IInvoice invoice)
        {
            // TODO localization needed on pricing and datetime
            var patterns = new List<IReplaceablePattern>
            {
                ReplaceablePattern.GetConfigurationReplaceablePattern("InvoiceNumber", invoice.PrefixedInvoiceNumber()),
                ReplaceablePattern.GetConfigurationReplaceablePattern("InvoiceDate", invoice.InvoiceDate.ToShortDateString()),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToName", invoice.BillToName),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToAddress1", invoice.BillToAddress1),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToAddress2", invoice.BillToAddress2),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToLocality", invoice.BillToLocality),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToRegion", invoice.BillToRegion),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToPostalCode", invoice.BillToPostalCode),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToCountryCode", invoice.BillToCountryCode),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToEmail", invoice.BillToEmail),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToPhone", invoice.BillToPhone),
                ReplaceablePattern.GetConfigurationReplaceablePattern("BillToCompany", invoice.BillToCompany),
                ReplaceablePattern.GetConfigurationReplaceablePattern("TotalPrice", invoice.Total.ToString("C")),
                ReplaceablePattern.GetConfigurationReplaceablePattern("InvoiceStatus", invoice.InvoiceStatus.Name)                
            };

            patterns.AddRange(invoice.LineItemReplaceablePatterns());
           
            return patterns;
        }
       
        #endregion

        #region Customer

        /// <summary>
        /// Gets the customer from an invoice (if applicable)
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public static ICustomer Customer(this IInvoice invoice)
        {
            return invoice.Customer(MerchelloContext.Current);
        }

        /// <summary>
        /// The customer.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public static ICustomer Customer(this IInvoice invoice, IMerchelloContext merchelloContext)
        {
            if (invoice.CustomerKey == null) return null;

            return merchelloContext.Services.CustomerService.GetByKey(invoice.CustomerKey.Value);
        }

        #endregion

        #region Order

        /// <summary>
        /// Prepares an <see cref="IOrder"/> without saving it to the database.  
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to base the order on</param>
        /// <returns>The <see cref="IOrder"/></returns>        
        public static IOrder PrepareOrder(this IInvoice invoice)
        {
            return invoice.PrepareOrder(MerchelloContext.Current);
        }

        /// <summary>
        /// Prepare an <see cref="IOrder"/> with saving it to the database
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to base the order or</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>The <see cref="IOrder"/></returns>
        public static IOrder PrepareOrder(this IInvoice invoice, IMerchelloContext merchelloContext)
        {
            var orderStatus =
                merchelloContext.Services.OrderService.GetOrderStatusByKey(
                    Constants.DefaultKeys.OrderStatus.NotFulfilled);

            return invoice.PrepareOrder(merchelloContext, new OrderBuilderChain(orderStatus, invoice));
        }

        /// <summary>
        /// Prepares an <see cref="IOrder"/> without saving it to the database.  
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to base the order on</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="orderBuilder">The <see cref="IBuilderChain{IOrder}"/></param>
        /// <returns>The <see cref="IOrder"/></returns>
        /// <remarks>
        /// 
        /// This method will save the invoice in the event it has not previously been saved
        /// 
        /// </remarks>
        public static IOrder PrepareOrder(
            this IInvoice invoice,
            IMerchelloContext merchelloContext,
            IBuilderChain<IOrder> orderBuilder)
        {
            if (!invoice.HasIdentity) merchelloContext.Services.InvoiceService.Save(invoice);

            var attempt = orderBuilder.Build();
            if (attempt.Success) return attempt.Result;

            LogHelper.Error<OrderBuilderChain>("Extension method PrepareOrder failed", attempt.Exception);
            throw attempt.Exception;
        }

        #endregion

        #region AppliedPayments

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IInvoice invoice)
        {
            return invoice.AppliedPayments(MerchelloContext.Current);
        }

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        internal static IEnumerable<IAppliedPayment> AppliedPayments(
            this IInvoice invoice,
            IMerchelloContext merchelloContext)
        {
            return invoice.AppliedPayments(merchelloContext.Services.GatewayProviderService);
        }

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="gatewayProviderService">The <see cref="IGatewayProviderService"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(
            this IInvoice invoice,
            IGatewayProviderService gatewayProviderService)
        {
            return gatewayProviderService.GetAppliedPaymentsByInvoiceKey(invoice.Key);
        }

        #endregion

        #region Payments

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> applied to the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        public static IEnumerable<IPayment> Payments(this IInvoice invoice)
        {
            return invoice.Payments(MerchelloContext.Current);
        }

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> applied to the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        internal static IEnumerable<IPayment> Payments(this IInvoice invoice, IMerchelloContext merchelloContext)
        {
            return merchelloContext.Services.PaymentService.GetPaymentsByInvoiceKey(invoice.Key);
        }


        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizePayment(this IInvoice invoice,
            IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            return paymentGatewayMethod.AuthorizePayment(invoice, args);
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizePayment(this IInvoice invoice,
            IPaymentGatewayMethod paymentGatewayMethod)
        {
            Mandate.ParameterCondition(invoice.HasIdentity,
                "The invoice must be saved before a payment can be authorized.");
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            return invoice.AuthorizePayment(paymentGatewayMethod, new ProcessorArgumentCollection());
        }


        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizePayment(this IInvoice invoice, Guid paymentMethodKey,
            ProcessorArgumentCollection args)
        {
            return invoice.AuthorizePayment(MerchelloContext.Current, paymentMethodKey, args);
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult AuthorizePayment(this IInvoice invoice, IMerchelloContext merchelloContext,
            Guid paymentMethodKey, ProcessorArgumentCollection args)
        {

            var paymentMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return invoice.AuthorizePayment(paymentMethod, args);

        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizePayment(this IInvoice invoice, Guid paymentMethodKey)
        {
            return invoice.AuthorizePayment(paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice,
            IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            return paymentGatewayMethod.AuthorizeCapturePayment(invoice, invoice.Total, args);
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice,
            IPaymentGatewayMethod paymentGatewayMethod)
        {
            return invoice.AuthorizeCapturePayment(paymentGatewayMethod, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice, Guid paymentMethodKey,
            ProcessorArgumentCollection args)
        {
            return invoice.AuthorizeCapturePayment(MerchelloContext.Current, paymentMethodKey, args);
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice, IMerchelloContext merchelloContext,
            Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return invoice.AuthorizeCapturePayment(paymentMethod, args);
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice, Guid paymentMethodKey)
        {
            return invoice.AuthorizeCapturePayment(paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="paymentGatewayMethod"></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IInvoice invoice, IPayment payment,
            IPaymentGatewayMethod paymentGatewayMethod, decimal amount, ProcessorArgumentCollection args)
        {
            return paymentGatewayMethod.CapturePayment(invoice, payment, amount, args);
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="paymentGatewayMethod"></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IInvoice invoice, IPayment payment,
            IPaymentGatewayMethod paymentGatewayMethod, decimal amount)
        {
            return invoice.CapturePayment(payment, paymentGatewayMethod, amount, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IInvoice invoice, IPayment payment, Guid paymentMethodKey,
            decimal amount, ProcessorArgumentCollection args)
        {
            return invoice.CapturePayment(MerchelloContext.Current, payment, paymentMethodKey, amount, args);
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="payment">The</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult CapturePayment(this IInvoice invoice, IMerchelloContext merchelloContext,
            IPayment payment, Guid paymentMethodKey, decimal amount, ProcessorArgumentCollection args)
        {
            var paymentGatewayMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return invoice.CapturePayment(payment, paymentGatewayMethod, amount, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IInvoice invoice, IPayment payment,
            IPaymentGatewayMethod paymentGatewayMethod, decimal amount, ProcessorArgumentCollection args)
        {
            return paymentGatewayMethod.RefundPayment(invoice, payment, amount, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IInvoice invoice, IPayment payment,
            IPaymentGatewayMethod paymentGatewayMethod, decimal amount)
        {
            return invoice.RefundPayment(payment, paymentGatewayMethod, amount, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IInvoice invoice, IPayment payment, Guid paymentMethodKey, decimal amount)
        {
            return invoice.RefundPayment(payment, paymentMethodKey, amount, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IInvoice invoice, IPayment payment, Guid paymentMethodKey, decimal amount,
            ProcessorArgumentCollection args)
        {
            return invoice.RefundPayment(MerchelloContext.Current, payment, paymentMethodKey, amount, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult RefundPayment(this IInvoice invoice, IMerchelloContext merchelloContext,
            IPayment payment, Guid paymentMethodKey, decimal amount, ProcessorArgumentCollection args)
        {
            var paymentGatewayMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return invoice.RefundPayment(payment, paymentGatewayMethod, amount, args);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult VoidPayment(this IInvoice invoice, IPayment payment,
            IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            return paymentGatewayMethod.VoidPayment(invoice, payment, args);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentGatewayMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult VoidPayment(this IInvoice invoice, IPayment payment, Guid paymentMethodKey)
        {
            return invoice.VoidPayment(payment, paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentGatewayMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult VoidPayment(this IInvoice invoice, IPayment payment, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            return invoice.VoidPayment(MerchelloContext.Current, payment, paymentMethodKey, args);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentGatewayMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult VoidPayment(this IInvoice invoice, IMerchelloContext merchelloContext, IPayment payment, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentGatewayMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return paymentGatewayMethod.VoidPayment(invoice, payment, args);
        }



        #endregion


        #region Taxation

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/>
        /// </param>
        /// <param name="quoteOnly">
        /// A value indicating whether or not the taxes should be calculated as a quote
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/> from the calculation
        /// </returns>
        public static ITaxCalculationResult CalculateTaxes(this IInvoice invoice, bool quoteOnly = true)
        {
            return invoice.CalculateTaxes(invoice.GetBillingAddress(), quoteOnly);
        }

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="taxAddress">The address (generally country code and region) to be used to determine the taxation rates</param>
        /// <param name="quoteOnly">A value indicating whether or not the taxes should be calculated as a quote</param>
        /// <returns>The <see cref="ITaxCalculationResult"/> from the calculation</returns>
        public static ITaxCalculationResult CalculateTaxes(this IInvoice invoice, IAddress taxAddress, bool quoteOnly = true)
        {
            return invoice.CalculateTaxes(MerchelloContext.Current, taxAddress, quoteOnly);
        }

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="taxAddress">The address (generally country code and region) to be used to determine the taxation rates</param>
        /// <param name="quoteOnly">A value indicating whether or not the taxes should be calculated as a quote</param>
        /// <returns>The <see cref="ITaxCalculationResult"/> from the calculation</returns>
        internal static ITaxCalculationResult CalculateTaxes(this IInvoice invoice, IMerchelloContext merchelloContext, IAddress taxAddress, bool quoteOnly = true)
        {
            // remove any other tax lines
            return merchelloContext.Gateways.Taxation.CalculateTaxesForInvoice(invoice, taxAddress, quoteOnly);
        }

        #endregion


        #region Totals

        /// <summary>
        /// Sums the total price of invoice items
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        public static decimal TotalItemPrice(this IInvoice invoice)
        {                                                                 
            return invoice.Items.Where(x => x.LineItemType == LineItemType.Product).Sum(x => x.TotalPrice);
        }

        /// <summary>
        /// Sums the total shipping amount for the invoice items
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        public static decimal TotalShipping(this IInvoice invoice)
        {
            return invoice.Items.Where(x => x.LineItemType == LineItemType.Shipping).Sum(x => x.TotalPrice);
        }

        /// <summary>
        /// Sums the total tax amount for the invoice items
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        public static decimal TotalTax(this IInvoice invoice)
        {
            return invoice.Items.Where(x => x.LineItemType == LineItemType.Tax).Sum(x => x.TotalPrice);
        }

        /// <summary>
        /// The total discounts.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal TotalDiscounts(this IInvoice invoice)
        {
            return invoice.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.TotalPrice);
        }

    #endregion


        #region Examine Serialization

        /// <summary>
        /// Serializes <see cref="IInvoice"/> object
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <remarks>
        /// Intended to be used by the Merchello.Examine.Providers.MerchelloInvoiceIndexer
        /// </remarks>
        /// <returns>
        /// The <see cref="XDocument"/>.
        /// </returns>
        internal static XDocument SerializeToXml(this IInvoice invoice)
        {
            string xml;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("invoice");
                    writer.WriteAttributeString("id", ((Invoice)invoice).ExamineId.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("invoiceKey", invoice.Key.ToString());
                    writer.WriteAttributeString("customerKey", invoice.CustomerKey.ToString());
                    writer.WriteAttributeString("invoiceNumberPrefix", invoice.InvoiceNumberPrefix);
                    writer.WriteAttributeString("invoiceNumber", invoice.InvoiceNumber.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("prefixedInvoiceNumber", invoice.PrefixedInvoiceNumber());
                    writer.WriteAttributeString("invoiceDate", invoice.InvoiceDate.ToString("s"));
                    writer.WriteAttributeString("invoiceStatusKey", invoice.InvoiceStatusKey.ToString());
                    writer.WriteAttributeString("versionKey", invoice.VersionKey.ToString());
                    writer.WriteAttributeString("billToName", invoice.BillToName);
                    writer.WriteAttributeString("billToAddress1", invoice.BillToAddress1);
                    writer.WriteAttributeString("billToAddress2", invoice.BillToAddress2);
                    writer.WriteAttributeString("billToLocality", invoice.BillToLocality);
                    writer.WriteAttributeString("billToRegion", invoice.BillToRegion);
                    writer.WriteAttributeString("billToPostalCode", invoice.BillToPostalCode);
                    writer.WriteAttributeString("billToCountryCode", invoice.BillToCountryCode);
                    writer.WriteAttributeString("billToEmail", invoice.BillToEmail);
                    writer.WriteAttributeString("billtoPhone", invoice.BillToPhone);
                    writer.WriteAttributeString("billtoCompany", invoice.BillToCompany);
                    writer.WriteAttributeString("exported", invoice.Exported.ToString());
                    writer.WriteAttributeString("archived", invoice.Archived.ToString());
                    writer.WriteAttributeString("total", invoice.Total.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("invoiceStatus", GetInvoiceStatusJson(invoice.InvoiceStatus));
                    writer.WriteAttributeString("invoiceItems", GetGenericItemsCollection(invoice.Items));
                    writer.WriteAttributeString("createDate", invoice.CreateDate.ToString("s"));
                    writer.WriteAttributeString("updateDate", invoice.UpdateDate.ToString("s"));
                    writer.WriteAttributeString("allDocs", "1");
                    writer.WriteEndElement(); 
                    writer.WriteEndDocument();
                    xml = sw.ToString();
                }
            }

            return XDocument.Parse(xml);
            
        }

        /// <summary>
        /// The get invoice status JSON.
        /// </summary>
        /// <param name="invoiceStatus">
        /// The invoice status.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetInvoiceStatusJson(IInvoiceStatus invoiceStatus)
        {
            return JsonConvert.SerializeObject(
                    new
                        {
                            key = invoiceStatus.Key,
                            name = invoiceStatus.Name,
                            alias = invoiceStatus.Alias,
                            reportable = invoiceStatus.Reportable,
                            active = invoiceStatus.Active,
                            sortOrder = invoiceStatus.SortOrder
                        }, 
                        Formatting.None);
        }
        

        /// <summary>
        /// The get generic items collection.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetGenericItemsCollection(IEnumerable<ILineItem> items)
        {
            return JsonConvert.SerializeObject(
                items.Select(x => 
                    new
                        {
                            key = x.Key,
                            containerKey = x.ContainerKey,
                            name = x.Name,
                            lineItemTfKey = x.LineItemTfKey,
                            lineItemType = x.LineItemType.ToString(),
                            sku = x.Sku,
                            price = x.Price,
                            quantity = x.Quantity,
                            exported = x.Exported
                        }), 
                Formatting.None);
        }

        #endregion
    }
}