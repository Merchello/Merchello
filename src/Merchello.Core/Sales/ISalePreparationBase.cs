namespace Merchello.Core.Sales
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Gateways.Payment;
    using Gateways.Shipping;
    using Models;

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
        /// <returns>A shipping <see cref="IAddress"/></returns>
        IAddress GetShipToAddress();

        /// <summary>
        /// Saves a single <see cref="IShipmentRateQuote"/>
        /// </summary>
        /// <param name="approvedShipmentRateQuote">The selected <see cref="IShipmentRateQuote"/> to be used when invoicing the order</param>
        void SaveShipmentRateQuote(IShipmentRateQuote approvedShipmentRateQuote);
        
        /// <summary>
        /// Saves a collection <see cref="IShipmentRateQuote"/>
        /// </summary>
        /// <param name="approvedShipmentRateQuotes">
        /// A collection of <see cref="IShipmentRateQuote"/> to be saved
        /// </param>
        /// <remarks>
        /// 
        /// This will be useful when multiple shipments are exposed
        /// 
        /// </remarks>
        void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes);

        /// <summary>
        /// Clears all <see cref="IShipmentRateQuote"/>s previously saved
        /// </summary>
        void ClearShipmentRateQuotes();

        /// <summary>
        /// Saves a <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethod">The <see cref="IPaymentMethod"/> to be saved</param>
        void SavePaymentMethod(IPaymentMethod paymentMethod);

        /// <summary>
        /// Gets the previously saved <see cref="IPaymentMethod"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IPaymentMethod"/>.
        /// </returns>
        IPaymentMethod GetPaymentMethod();

        /// <summary>
        /// Prepares an <see cref="IInvoice"/> representing the bill for the current "sale"
        /// </summary>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        IInvoice PrepareInvoice();

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "sale"
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
        /// Attempts to authorize a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(Guid paymentMethodKey, ProcessorArgumentCollection args);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(Guid paymentMethodKey);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey, ProcessorArgumentCollection args);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey);

        /// <summary>
        /// True/false indicating whether or not the <see cref="ISalePreparationBase"/> is ready to prepare an <see cref="IInvoice"/>
        /// </summary>
        /// <returns>
        /// True or false indicating whether or not an invoice can be created
        /// </returns>
        bool IsReadyToInvoice();

        /// <summary>
        /// Adds a <see cref="ILineItem"/> to the collection of items
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <remarks>
        /// Intended for custom line item types
        /// http://issues.merchello.com/youtrack/issue/M-381
        /// </remarks>
        void AddItem(ILineItem lineItem);

        /// <summary>
        /// Removes a line item for the collection of items
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        void RemoveItem(ILineItem lineItem);
    }
}