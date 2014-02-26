using System;
using System.Collections.Generic;
using Merchello.Core.Builders;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;

namespace Merchello.Core.Sales
{
    /// <summary>
    /// Defines a sales preparation base class
    /// </summary>
    public interface ISalePreparationBase
    {
        /// <summary>
        /// Restarts the checkout process, deleting all persisted data
        /// </summary>
        void Reset();

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

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        IInvoice PrepareInvoice();

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <param name="invoiceBuilder">The invoice builder class</param>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        IInvoice PrepareInvoice(IBuilderChain<IInvoice> invoiceBuilder);

        /// <summary>
        /// Gets a list of all possible Payment Methods
        /// </summary>
        /// <returns>A collection of <see cref="IPaymentGatewayMethod"/>s</returns>
        IEnumerable<IPaymentGatewayMethod> GetPaymentGatewayMethods();

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult ProcessPayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult ProcessPayment(IPaymentGatewayMethod paymentGatewayMethod);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult ProcessPayment(Guid paymentMethodKey, ProcessorArgumentCollection args);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult ProcessPayment(Guid paymentMethodKey);

        /// <summary>
        /// True/false indicating whether or not the <see cref="ISalePreparationBase"/> is ready to prepare an <see cref="IInvoice"/>
        /// </summary>
        bool IsReadyToInvoice();
    }
}