using Merchello.Core.Gateways.Taxation;

namespace Merchello.Core.Models
{
    public static class InvoiceExtensions
    {

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <returns>The <see cref="IInvoiceTaxResult"/> from the calculation</returns>
        public static IInvoiceTaxResult CalculateTaxes(this IInvoice invoice)
        {
            return invoice.CalculateTaxes(invoice.GetBillingAddress());
        }

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="taxAddress">The address (generally country code and region) to be used to determine the taxation rates</param>
        /// <returns>The <see cref="IInvoiceTaxResult"/> from the calculation</returns>
        public static IInvoiceTaxResult CalculateTaxes(this IInvoice invoice, IAddress taxAddress)
        {
            return invoice.CalculateTaxes(MerchelloContext.Current, taxAddress);
        }

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="taxAddress">The address (generally country code and region) to be used to determine the taxation rates</param>
        /// <returns>The <see cref="IInvoiceTaxResult"/> from the calculation</returns>
        public static IInvoiceTaxResult CalculateTaxes(this IInvoice invoice, IMerchelloContext merchelloContext, IAddress taxAddress)
        {
            // remove any other tax lines
            return merchelloContext.Gateways.Taxation.CalculateTaxesForInvoice(invoice, taxAddress);
        }
         
    }
}