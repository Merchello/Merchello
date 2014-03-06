using System.Globalization;
using Merchello.Core.Builders;
using Merchello.Core.Gateways.Taxation;
using Umbraco.Core.Logging;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Extension methods for <see cref="IInvoices"/>
    /// </summary>
    public static class InvoiceExtensions
    {
        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <returns>The <see cref="ITaxCalculationResult"/> from the calculation</returns>
        public static ITaxCalculationResult CalculateTaxes(this IInvoice invoice)
        {
            return invoice.CalculateTaxes(invoice.GetBillingAddress());
        }

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="taxAddress">The address (generally country code and region) to be used to determine the taxation rates</param>
        /// <returns>The <see cref="ITaxCalculationResult"/> from the calculation</returns>
        public static ITaxCalculationResult CalculateTaxes(this IInvoice invoice, IAddress taxAddress)
        {
            return invoice.CalculateTaxes(MerchelloContext.Current, taxAddress);
        }

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="taxAddress">The address (generally country code and region) to be used to determine the taxation rates</param>
        /// <returns>The <see cref="ITaxCalculationResult"/> from the calculation</returns>
        public static ITaxCalculationResult CalculateTaxes(this IInvoice invoice, IMerchelloContext merchelloContext, IAddress taxAddress)
        {
            // remove any other tax lines
            return merchelloContext.Gateways.Taxation.CalculateTaxesForInvoice(invoice, taxAddress);
        }

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
            return invoice.PrepareOrder(merchelloContext, new OrderBuilderChain(invoice));
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
        public static IOrder PrepareOrder(this IInvoice invoice, IMerchelloContext merchelloContext, IBuilderChain<IOrder> orderBuilder)
        {
            if(!invoice.HasIdentity) merchelloContext.Services.InvoiceService.Save(invoice);

            var attempt = orderBuilder.Build();
            if (attempt.Success) return attempt.Result;

            LogHelper.Error<OrderBuilderChain>("Extension method PrepareOrder failed", attempt.Exception);
            throw attempt.Exception;
        }
    }
}