using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Shipment Service 
    /// </summary>
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

        #region IShipmentService Members

        /// <summary>
        /// Creates a Shipment
        /// </summary>
        public IShipment CreateShipment(IShipMethod shipMethod, IInvoice invoice, IAddress address)
        {
            return CreateShipment(shipMethod, invoice, address.Address1, address.Address2, address.Locality,
                address.Region, address.PostalCode, address.CountryCode, address.Phone);
        }

        public IShipment CreateShipment(IInvoice invoice, string address1, string address2, string locality,
            string region, string postalCode, string countryCode, string phone)
        {
            return CreateShipment(null, invoice, address1, address2, locality, region, postalCode, countryCode, phone);
        }

        /// <summary>
        /// Creates an <see cref="IShipment"/> object
        /// </summary>
        public IShipment CreateShipment(IShipMethod shipMethod, IInvoice invoice, string address1, string address2, string locality, string region, string postalCode, string countryCode, string phone)
        {
            var shipment = new Shipment(invoice.Id)
                {
                    Address1 = address1, 
                    Address2 = address2, 
                    Locality = locality, 
                    Region = region, 
                    PostalCode = postalCode, 
                    CountryCode = countryCode, 
                    ShipMethodId = shipMethod == null ? (int?)null : shipMethod.Id,
                    Phone = phone
                };
                
            Created.RaiseEvent(new Events.NewEventArgs<IShipment>(shipment), this);

            return shipment;
        }

        /// <summary>
        /// Saves a single <see cref="IShipment"/> object
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IShipment shipment, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IShipment>(shipment), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipmentRepository(uow))
                {
                    repository.AddOrUpdate(shipment);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipment>(shipment), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipment"/> objects.
        /// </summary>
        /// <param name="shipmentList">Collection of <see cref="Shipment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            var shipmentArray = shipmentList as IShipment[] ?? shipmentList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IShipment>(shipmentArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipmentRepository(uow))
                {
                    foreach (var shipment in shipmentArray)
                    {
                        repository.AddOrUpdate(shipment);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipment>(shipmentArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IShipment"/> object
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IShipment shipment, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IShipment>( shipment), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipmentRepository(uow))
                {
                    repository.Delete( shipment);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipment>( shipment), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IShipment"/> objects
        /// </summary>
        /// <param name="shipmentList">Collection of <see cref="IShipment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            var shipmentArray = shipmentList as IShipment[] ?? shipmentList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IShipment>(shipmentArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipmentRepository(uow))
                {
                    foreach (var shipment in shipmentArray)
                    {
                        repository.Delete(shipment);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipment>(shipmentArray), this);
        }

        /// <summary>
        /// Gets a Shipment by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Shipment</param>
        /// <returns><see cref="IShipment"/></returns>
        public IShipment GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }


        /// <summary>
        /// Gets a list of all shipments for a given ship method
        /// </summary>
        /// <param name="shipMethodId">The id of the ship method</param>
        /// <returns>A collection of <see cref="IShipment"/></returns>
        public IEnumerable<IShipment> GetShipmentsForShipMethod(int shipMethodId)
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IShipment>.Builder.Where(x => x.ShipMethodId == shipMethodId);
                return repository.GetByQuery(query);                
            }
        }

        /// <summary>
        /// Gets a list fo all shipments for a given invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public IEnumerable<IShipment> GetShipmentsForInvoice(int invoiceId)
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IShipment>.Builder.Where(x => x.InvoiceId == invoiceId);                
                return repository.GetByQuery(query);                
            }
        }

        /// <summary>
        /// Gets a list of Shipment give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IShipment> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        public IEnumerable<IShipment> GetAll()
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShippingService, DeleteEventArgs<IShipment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShippingService, DeleteEventArgs<IShipment>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShippingService, SaveEventArgs<IShipment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShippingService, SaveEventArgs<IShipment>> Saved;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShippingService, Events.NewEventArgs<IShipment>> Created;

        #endregion


     
    }
}