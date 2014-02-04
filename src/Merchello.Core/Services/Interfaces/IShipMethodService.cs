using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
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
        /// <param name="shipMethod"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
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
        /// <param name="shipMethod"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IShipMethod shipMethod, bool raiseEvents = true);

        /// <summary>
        /// Gets a list of <see cref="IShipMethod"/> objects given a <see cref="IGatewayProvider"/> key and a <see cref="IShipCountry"/> key
        /// </summary>
        /// <returns>A collection of <see cref="IShipMethod"/></returns>
        IEnumerable<IShipMethod> GetGatewayProviderShipMethods(Guid providerKey, Guid shipCountryKey);

    }
}