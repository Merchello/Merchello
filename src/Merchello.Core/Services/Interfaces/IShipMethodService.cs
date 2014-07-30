namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// The ShipMethodService interface.
    /// </summary>
    public interface IShipMethodService : IService
    {
        ///// <summary>
        ///// Creates a <see cref="IShipMethod"/>.  This is useful due to the data constraint
        ///// preventing two ShipMethods being created with the same ShipCountry and ServiceCode for any provider.
        ///// </summary>
        ///// <param name="providerKey">The unique gateway provider key (Guid)</param>
        ///// <param name="IShipCountry"></param>
        ///// <param name="serviceCode">The ShipMethods service code</param>
        ///// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        //Attempt<IShipMethod> CreateShipMethodWithKey(Guid providerKey, IShipCountry shipCountryKey, string name, string serviceCode, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod">
        /// The ship Method.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IShipMethod shipMethod, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="IShipMethod"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IShipMethod> shipMethodList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod">
        /// The ship Method.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(IShipMethod shipMethod, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethods">The collection of <see cref="IShipMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IShipMethod> shipMethods, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IShipMethod"/> given it's unique 'key' (Guid)
        /// </summary>
        /// <param name="key">The <see cref="IShipMethod"/>'s unique 'key' (Guid)</param>
        /// <returns><see cref="IShipMethod"/></returns>
        IShipMethod GetByKey(Guid key);

        /// <summary>
        /// Gets a list of <see cref="IShipMethod"/> objects given a <see cref="IGatewayProviderSettings"/> key and a <see cref="IShipCountry"/> key
        /// </summary>
        /// <param name="providerKey">
        /// The provider Key.
        /// </param>
        /// <param name="shipCountryKey">
        /// The ship Country Key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IShipMethod"/>
        /// </returns>
        IEnumerable<IShipMethod> GetShipMethodsByProviderKey(Guid providerKey, Guid shipCountryKey);

        /// <summary>
        /// Gets a list of all <see cref="IShipMethod"/> objects given a <see cref="IGatewayProviderSettings"/> key
        /// </summary>
        /// <param name="providerKey">
        /// The provider Key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IShipMethod"/>
        /// </returns>
        IEnumerable<IShipMethod> GetShipMethodsByProviderKey(Guid providerKey); 

    }
}