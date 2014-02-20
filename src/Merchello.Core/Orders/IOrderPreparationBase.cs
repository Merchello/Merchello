using System.Collections.Generic;
using Merchello.Core.Builders;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;

namespace Merchello.Core.Checkout
{
    /// <summary>
    /// Defines a Checkout base class
    /// </summary>
    public interface IOrderPreparationBase
    {
        /// <summary>
        /// Restarts the checkout process, deleting all persisted data
        /// </summary>
        void RestartCheckout();

        /// <summary>
        /// Saves the bill to address
        /// </summary>
        /// <param name="billToAddress">The billing <see cref="IAddress"/></param>
        void SaveBillToAddress(IAddress billToAddress);

        /// <summary>
        /// Saves the ship to address
        /// </summary>
        /// <param name="shipToAddress">The shipping <see cref="IAddress"/></param>
        void SaveShipToAddress(IAddress shipToAddress);

        /// <summary>
        /// Gets the bill to address
        /// </summary>
        /// <returns>Return the billing <see cref="IAddress"/></returns>
        IAddress GetBillToAddress();

        /// <summary>
        /// Gets the ship to address
        /// </summary>
        /// <remarks>Returns the shipping <see cref="IAddress"/></remarks>
        /// <returns></returns>
        IAddress GetShipToAddress();

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

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        IInvoice GenerateInvoice();

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <param name="invoiceBuilder">The invoice builder class</param>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        IInvoice GenerateInvoice(IBuilderChain<IInvoice> invoiceBuilder);

        ///// <summary>
        ///// Does preliminary validation of the checkout process and then executes the start of the order fulfillment pipeline
        ///// </summary>
        ///// <param name="paymentGatewayProvider">The see <see cref="IPaymentGatewayProvider"/> to be used in payment processing and <see cref="IOrder"/> creation approval</param>
        //void CompleteCheckout(IPaymentGatewayProvider paymentGatewayProvider);
    }
}