namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// The PaymentService interface.
    /// </summary>
    public interface IPaymentService : IService
    {
        /// <summary>
        /// Creates a payment without saving it to the database
        /// </summary>
        /// <param name="paymentMethodType">The type of the payment method</param>
        /// <param name="amount">The amount of the payment</param>
        /// <param name="paymentMethodKey">The optional paymentMethodKey</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Returns <see cref="IPayment"/></returns>
        IPayment CreatePayment(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey, bool raiseEvents = true);

        /// <summary>
        /// Creates and saves a payment
        /// </summary>
        /// <param name="paymentMethodType">The type of the payment method</param>
        /// <param name="amount">The amount of the payment</param>
        /// <param name="paymentMethodKey">The optional paymentMethodKey</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Returns <see cref="IPayment"/></returns>
        IPayment CreatePaymentWithKey(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey, bool raiseEvents = true);

        /////// <summary>
        /////// Creates and saves a payment
        /////// </summary>
        /////// <param name="paymentTfKey">The payment typefield key</param>
        /////// <param name="amount">The amount of the payment</param>
        /////// <param name="paymentMethodKey">The optional paymentMethodKey</param>
        /////// <returns>Returns <see cref="IPayment"/></returns>
        ////IPayment CreatePaymentWithKey(Guid paymentTfKey, decimal amount, Guid? paymentMethodKey);

        /// <summary>
        /// Saves a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IPayment payment, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IPayment"/>
        /// </summary>
        /// <param name="payments">A collection of <see cref="IPayment"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IPayment> payments, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IPayment"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IPayment payment, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IPayment"/>
        /// </summary>
        /// <param name="payments">
        /// The payments.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        void Delete(IEnumerable<IPayment> payments, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IPayment"/>
        /// </summary>
        /// <param name="key">The unique 'key' (GUID) of the <see cref="IPayment"/></param>
        /// <returns><see cref="IPaymentMethod"/></returns>
        IPayment GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> given a list of keys
        /// </summary>
        /// <param name="keys">A collection of payment keys</param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        IEnumerable<IPayment> GetByKeys(IEnumerable<Guid> keys);
            
        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> for a given PaymentGatewayProvider
        /// </summary>
        /// <param name="paymentMethodKey">The unique 'key' of the PaymentGatewayProvider</param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        IEnumerable<IPayment> GetPaymentsByPaymentMethodKey(Guid? paymentMethodKey);

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> for a given invoice
        /// </summary>
        /// <param name="invoiceKey">The unique 'key' of the invoice</param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        IEnumerable<IPayment> GetPaymentsByInvoiceKey(Guid invoiceKey);

        /// <summary>
        /// Get a list of payments by customer key.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IPayment"/>.
        /// </returns>
        IEnumerable<IPayment> GetPaymentsByCustomerKey(Guid customerKey); 
            
        /// <summary>
        /// Creates and saves an AppliedPayment
        /// </summary>
        /// <param name="paymentKey">The payment key</param>
        /// <param name="invoiceKey">The invoice 'key'</param>
        /// <param name="appliedPaymentType">The applied payment type</param>
        /// <param name="description">The description of the payment application</param>
        /// <param name="amount">The amount of the payment to be applied</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>An <see cref="IAppliedPayment"/></returns>
        IAppliedPayment ApplyPaymentToInvoice(Guid paymentKey, Guid invoiceKey, AppliedPaymentType appliedPaymentType, string description, decimal amount, bool raiseEvents = true);

        /// <summary>
        /// Saves an <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IAppliedPayment appliedPayment, bool raiseEvents = true);

        /// <summary>
        /// Deletes a <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IAppliedPayment appliedPayment, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayments">The collection of <see cref="IAppliedPayment"/>s to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IAppliedPayment> appliedPayments, bool raiseEvents = true);

        /// <summary>
        /// Gets a collection of <see cref="IAppliedPayment"/>s by the payment key
        /// </summary>
        /// <param name="paymentKey">The payment key</param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        IEnumerable<IAppliedPayment> GetAppliedPaymentsByPaymentKey(Guid paymentKey);

        /// <summary>
        /// Gets a collection of <see cref="IAppliedPayment"/>s by the invoice key
        /// </summary>
        /// <param name="invoiceKey">The invoice key</param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        IEnumerable<IAppliedPayment> GetAppliedPaymentsByInvoiceKey(Guid invoiceKey);
    }
}