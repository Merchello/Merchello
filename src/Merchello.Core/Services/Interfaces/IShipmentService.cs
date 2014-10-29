namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the ShipmentService which provides access to shipment operations
    /// </summary>
    public interface IShipmentService : IService
    {
        /// <summary>
        /// Creates a <see cref="IShipment"/> without persisting it to the database.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment status.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        IShipment CreateShipment(IShipmentStatus shipmentStatus, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IShipment"/> without persisting it to the database.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment status.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        IShipment CreateShipment(IShipmentStatus shipmentStatus, IAddress origin, IAddress destination, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IShipment"/> without persisting it to the database.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment status.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        IShipment CreateShipment(IShipmentStatus shipmentStatus, IAddress origin, IAddress destination, LineItemCollection items, bool raiseEvents = true);

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
        /// <param name="key">Guid pk of the Shipment to retrieve</param>
        /// <returns><see cref="IShipment"/></returns>
        IShipment GetByKey(Guid key);

        /// <summary>
        /// Gets a list of <see cref="IShipment"/> object given a ship method Key
        /// </summary>
        /// <param name="shipMethodKey">The pk of the shipMethod</param>
        /// <returns>A collection of <see cref="IShipment"/></returns>
        IEnumerable<IShipment> GetShipmentsByShipMethodKey(Guid shipMethodKey);


        /// <summary>
        /// Gets list of <see cref="IShipment"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid keys for Shipment objects to retrieve</param>
        /// <returns>List of <see cref="IShipment"/></returns>
        IEnumerable<IShipment> GetByKeys(IEnumerable<Guid> keys);

        /// <summary>
        /// Gets a collection of <see cref="IShipment"/> give an order key
        /// </summary>
        /// <param name="orderKey"></param>
        /// <returns></returns>
        IEnumerable<IShipment> GetShipmentsByOrderKey(Guid orderKey);


        /// <summary>
        /// Gets an <see cref="IShipmentStatus"/> by it's key
        /// </summary>
        /// <param name="key">The <see cref="IShipmentStatus"/> key</param>
        /// <returns><see cref="IShipmentStatus"/></returns>
        IShipmentStatus GetShipmentStatusByKey(Guid key);

        /// <summary>
        /// Returns a collection of all <see cref="IShipmentStatus"/>
        /// </summary>
        /// <returns>
        /// The collection of <see cref="IShipmentStatus"/>.
        /// </returns>
        IEnumerable<IShipmentStatus> GetAllShipmentStatuses();
    }
}
