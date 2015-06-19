namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the OfferRedeemedService.
    /// </summary>
    internal interface IOfferRedeemedService : IService
    {
        /// <summary>
        /// Creates an <see cref="IOfferRedeemed"/> record
        /// </summary>
        /// <param name="offerSettings">
        /// The offer settings.
        /// </param>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferRedeemed"/>.
        /// </returns>
        IOfferRedeemed CreateOfferRedeemedWithKey(IOfferSettings offerSettings, IInvoice invoice, bool raiseEvents = true);

        /// <summary>
        /// Saves an <see cref="IOfferRedeemed"/>
        /// </summary>
        /// <param name="offerRedeemed">
        /// The offer redeemed.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IOfferRedeemed offerRedeemed, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IOfferRedeemed"/>
        /// </summary>
        /// <param name="redemptions">
        /// The redemptions.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IEnumerable<IOfferRedeemed> redemptions, bool raiseEvents = true);

        /// <summary>
        /// Deletes an <see cref="IOfferRedeemed"/>
        /// </summary>
        /// <param name="offerRedeemed">
        /// The offer redeemed.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(IOfferRedeemed offerRedeemed, bool raiseEvents = true);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="redemptions">
        /// The redemptions.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(IEnumerable<IOfferRedeemed> redemptions, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IOfferRedeemed"/> record by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferRedeemed"/>.
        /// </returns>
        IOfferRedeemed GetByKey(Guid key);

        /// <summary>
        ///  Gets a collection of <see cref="IOfferRedeemed"/> records by an invoice key.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferRedeemed}"/>.
        /// </returns>
        IEnumerable<IOfferRedeemed> GetByInvoiceKey(Guid invoiceKey);

        /// <summary>
        ///  Gets a collection of <see cref="IOfferRedeemed"/> records by a customer key.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferRedeemed}"/>.
        /// </returns>
        IEnumerable<IOfferRedeemed> GetByCustomerKey(Guid customerKey);

        /// <summary>
        /// Gets a collection of <see cref="IOfferRedeemed"/> records by a offer settings key.
        /// </summary>
        /// <param name="offerSettingsKey">
        /// The offer settings key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferRedeemed}"/>.
        /// </returns>
        IEnumerable<IOfferRedeemed> GetByOfferSettingsKey(Guid offerSettingsKey);

        /// <summary>
        /// The get by offer settings key and customer key.
        /// </summary>
        /// <param name="offerSettingsKey">
        /// The offer settings key.
        /// </param>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferRedeemed}"/>.
        /// </returns>
        IEnumerable<IOfferRedeemed> GetByOfferSettingsKeyAndCustomerKey(Guid offerSettingsKey, Guid customerKey); 
            
        /// <summary>
        /// Gets a collection of <see cref="IOfferRedeemed"/> records by an offer provider key.
        /// </summary>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferRedeemed}"/>.
        /// </returns>
        IEnumerable<IOfferRedeemed> GetByOfferProviderKey(Guid offerProviderKey);

        /// <summary>
        /// Gets the redemption count for an offer.
        /// </summary>
        /// <param name="offerSettingsKey">
        /// The offer settings key.
        /// </param>
        /// <returns>
        /// The current count of offer redemptions.
        /// </returns>
        int GetOfferRedeemedCount(Guid offerSettingsKey);
    }
}