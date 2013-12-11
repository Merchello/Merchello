using System;
using System.Collections.Generic;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    public class ShippingService : IShippingService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public ShippingService()
            : this(new RepositoryFactory())
        { }

        public ShippingService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public ShippingService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }


        #region Shipment


        public void Save(IShipment shipment, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IShipment shipment, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public IShipment GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipment> GetShipmentsForShipMethod(Guid shipMethodKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipment> GetByKeys(IEnumerable<Guid> keys)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ShipCountry


        public void Save(IShipCountry shipCountry)
        {
            throw new NotImplementedException();
        }

        public void Delete(IShipCountry shipCountry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipCountry> GetShipCountriesByWarehouseCatalogKey(Guid catalogKey)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region ShipMethod


        public void Save(IShipMethod shipMethod)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<IShipMethod> shipMethodList)
        {
            throw new NotImplementedException();
        }

        public void Delete(IShipMethod shipMethod)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipMethod> GetShipMethodsByShipCountryKey(Guid shipCountryKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipMethod> GetShipMethodsByGatewayProviderKey(Guid gatewayProviderKey)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region ShipRateTier

        public void Save(IShipRateTier shipRateTier)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<IShipRateTier> shipRateTierList)
        {
            throw new NotImplementedException();
        }

        public void Delete(IShipRateTier shipRateTier)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey)
        {
            throw new NotImplementedException();
        }

        #endregion



        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShippingService, Events.NewEventArgs<IShipment>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShippingService, Events.NewEventArgs<IShipment>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShippingService, SaveEventArgs<IShipment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShippingService, SaveEventArgs<IShipment>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShippingService, DeleteEventArgs<IShipment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShippingService, DeleteEventArgs<IShipment>> Deleted;

        #endregion
     
    }
}