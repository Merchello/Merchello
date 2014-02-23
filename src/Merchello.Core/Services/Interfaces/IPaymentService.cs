using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    public interface IPaymentService : IService
    {
        /// <summary>
        /// Creates and saves a payment
        /// </summary>
        /// <param name="paymentMethodType">The type of the paymentmethod</param>
        /// <param name="amount">The amount of the payment</param>
        /// <param name="paymentMethodKey">The optional paymentMethodKey</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Returns <see cref="IPayment"/></returns>
        IPayment CreatePaymentWithKey(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey, bool raiseEvents = true);

        ///// <summary>
        ///// Creates and saves a payment
        ///// </summary>
        ///// <param name="paymentTfKey">The payment typefield key</param>
        ///// <param name="amount">The amount of the payment</param>
        ///// <param name="paymentMethodKey">The optional paymentMethodKey</param>
        ///// <returns>Returns <see cref="IPayment"/></returns>
        //IPayment CreatePaymentWithKey(Guid paymentTfKey, decimal amount, Guid? paymentMethodKey);

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
        /// Gets a <see cref="IPayment"/>
        /// </summary>
        /// <param name="key">The unique 'key' (Guid) of the <see cref="IPayment"/></param>
        /// <returns><see cref="IPaymentMethod"/></returns>
        IPayment GetByKey(Guid key);

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
        IEnumerable<IPayment> GetPaymentsForInvoice(Guid invoiceKey);

    }
}