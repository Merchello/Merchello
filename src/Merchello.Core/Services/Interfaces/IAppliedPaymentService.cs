namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the AppliedPaymentService
    /// </summary>
    internal interface IAppliedPaymentService : IService
    {
        /// <summary>
        /// Creates and saves an AppliedPayment
        /// </summary>
        /// <param name="paymentKey">The PaymentGatewayProvider key</param>
        /// <param name="invoiceKey">The invoice 'key'</param>
        /// <param name="appliedPaymentType">The applied payment type</param>
        /// <param name="description">The description of the payment application</param>
        /// <param name="amount">The amount of the payment to be applied</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>An <see cref="IAppliedPayment"/></returns>
        IAppliedPayment CreateAppliedPaymentWithKey(Guid paymentKey, Guid invoiceKey, AppliedPaymentType appliedPaymentType, string description, decimal amount, bool raiseEvents = true);

        /// <summary>
        /// Saves an <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IAppliedPayment appliedPayment, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayments">The collection of <see cref="IAppliedPayment"/>s to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IAppliedPayment> appliedPayments, bool raiseEvents = true);

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
        /// Returns a <see cref="IAppliedPayment"/> by it's unique 'key'
        /// </summary>
        /// <param name="key">The unique 'key' of the <see cref="IAppliedPayment"/></param>
        /// <returns>An <see cref="IAppliedPayment"/></returns>
        IAppliedPayment GetByKey(Guid key);

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
        /// <returns>A collection <see cref="IAppliedPayment"/></returns>
        IEnumerable<IAppliedPayment> GetAppliedPaymentsByInvoiceKey(Guid invoiceKey);
    }
}