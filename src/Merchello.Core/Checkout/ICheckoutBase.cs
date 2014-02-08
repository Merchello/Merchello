using System.Collections.Generic;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Checkout
{
    /// <summary>
    /// Defines a Checkout base class
    /// </summary>
    public interface ICheckoutBase
    {
        /// <summary>
        /// Restarts the checkout process, deleting all persisted data
        /// </summary>
        void RestartCheckout();

        /// <summary>
        /// Saves the bill to address
        /// </summary>
        /// <param name="billToAddress"></param>
        void SaveBillToAddress(IAddress billToAddress);

        /// <summary>
        /// Saves a single <see cref="IShipmentRateQuote"/>
        /// </summary>
        /// <param name="approvedShipmentRateQuote">The selected <see cref="IShipmentRateQuote"/> to be used when invoicing the order</param>
        void SaveShipmentRateQuote(IShipmentRateQuote approvedShipmentRateQuote);

        /// <summary>
        /// Saves a collection <see cref="IShipmentRateQuote"/>
        /// </summary>
        /// <param name="approvedShipmentRateQuotes"></param>
        /// <remarks>
        /// 
        /// This will be useful when multiple shipments are exposed
        /// 
        /// </remarks>
        void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes);


        bool ApplyTaxesToInvoice { get; set; }

        ///// <summary>
        ///// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        ///// </summary>
        ///// <param name="applyTax">True/false indicating whether or not to apply taxes to the invoice.  Defaults to true</param>
        ///// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        //IInvoice GenerateInvoice(bool applyTax = true);

        ///// <summary>
        ///// Does preliminary validation of the checkout process and then executes the start of the order fulfillment pipeline
        ///// </summary>
        ///// <param name="paymentGatewayProvider">The see <see cref="IPaymentGatewayProvider"/> to be used in payment processing and <see cref="IOrder"/> creation approval</param>
        //void CompleteCheckout(IPaymentGatewayProvider paymentGatewayProvider);
    }
}