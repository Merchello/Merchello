using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the ShippingService, which provides access to shipping operations
    /// </summary>
    public interface IShippingService : IService
    {
        /// <summary>
        /// Creates a Shipment
        /// </summary>
        IShipment CreateShipment(IShipMethod shipMethod, IInvoice invoice, string address1, string address2, string locality, string region, string postalCode, string countryCode, string phone);

        /// <summary>
        /// Creates a Shipment
        /// </summary>
        IShipment CreateShipment(IInvoice invoice, string address1, string address2, string locality, string region, string postalCode, string countryCode, string phone);

        /// <summary>
        /// Creates a Shipment
        /// </summary>
        IShipment CreateShipment(IShipMethod shipMethod, IInvoice invoice, ICustomerAddress customerAddress);

        /// <summary>
        /// Saves a single <see cref="IShipment"/> object
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IShipment shipment, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IShipment"/> objects
        /// </summary>
        /// <param name="shipmentList">Collection of <see cref="IShipment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IShipment> shipmentList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IShipment"/> object
        /// </summary>
        /// <param name="shipment"><see cref="IShipment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IShipment shipment, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IShipment"/> objects
        /// </summary>
        /// <param name="shipmentList">Collection of <see cref="IShipment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IShipment> shipmentList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IShipment"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the Shipment to retrieve</param>
        /// <returns><see cref="IShipment"/></returns>
        IShipment GetById(int id);

        /// <summary>
        /// Gets a list of <see cref="IShipment"/> object given a ship method id
        /// </summary>
        /// <param name="shipMethodId">The id of the shipMethod</param>
        /// <returns>A collection of <see cref="IShipment"/></returns>
        IEnumerable<IShipment> GetShipmentsForShipMethod(int shipMethodId);


        /// <summary>
        /// Gets a list of <see cref="IShipment"/> object given a invoice id
        /// </summary>
        /// <param name="invoiceId">The id of the invoice</param>
        /// <returns>A collection of <see cref="IShipment"/></returns>
        IEnumerable<IShipment> GetShipmentsForInvoice(int invoiceId);
            
            
        /// <summary>
        /// Gets list of <see cref="IShipment"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for Shipment objects to retrieve</param>
        /// <returns>List of <see cref="IShipment"/></returns>
        IEnumerable<IShipment> GetByIds(IEnumerable<int> ids);

    }
}
