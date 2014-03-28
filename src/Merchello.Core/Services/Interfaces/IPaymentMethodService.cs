using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the PaymentMethodService
    /// </summary>
    internal interface IPaymentMethodService : IService
    {
        /// <summary>
        /// Saves a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethod">The <see cref="IPaymentMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IPaymentMethod paymentMethod, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="paymentMethods">A collection of <see cref="IPaymentMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IPaymentMethod> paymentMethods, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethod">The <see cref="IPaymentMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IPaymentMethod paymentMethod, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethods">The collection of <see cref="IPaymentMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IPaymentMethod> paymentMethods, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="key">The unique 'key' (Guid) of the <see cref="IPaymentMethod"/></param>
        /// <returns><see cref="IPaymentMethod"/></returns>
        IPaymentMethod GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IPaymentMethod"/> for a given PaymentGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the PaymentGatewayProvider</param>
        /// <returns>A collection of <see cref="IPaymentMethod"/></returns>
        IEnumerable<IPaymentMethod> GetPaymentMethodsByProviderKey(Guid providerKey);

        /// <summary>
        /// Returns a <see cref="IPaymentMethod"/> given is't paymentCode 
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the PaymentGatewayProvider</param>
        /// <param name="paymentCode">The paymentCode</param>
        IPaymentMethod GetPaymentMethodByPaymentCode(Guid providerKey, string paymentCode);

        /// <summary>
        /// Gets a collection of all <see cref="IPaymentMethod"/>
        /// </summary>
        IEnumerable<IPaymentMethod> GetAll();


    }
}