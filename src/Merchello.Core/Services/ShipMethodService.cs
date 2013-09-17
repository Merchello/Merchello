using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Events;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the ShipMethod Service 
    /// </summary>
    public class ShipMethodService : IShipMethodService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public ShipMethodService()
            : this(new RepositoryFactory())
        { }

        public ShipMethodService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public ShipMethodService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IShipMethodService Members


        /// <summary>
        /// Creates an <see cref="IShipMethod"/> object
        /// </summary>
        public IShipMethod CreateShipMethod(string name, int gatewayAlias, Guid shipMethodTypeFieldKey, decimal surcharge, string serviceCode)
        {
            var shipMethod = new ShipMethod()
                {
                    Name = name, 
                    GatewayAlias = gatewayAlias, 
                    ShipMethodTypeFieldKey = shipMethodTypeFieldKey, 
                    Surcharge = surcharge, 
                    ServiceCode = serviceCode
                };
                
            Created.RaiseEvent(new NewEventArgs<IShipMethod>(shipMethod), this);

            return shipMethod;
        }

        /// <summary>
        /// Saves a single <see cref="IShipMethod"/> object
        /// </summary>
        /// <param name="shipMethod">The <see cref="IShipMethod"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IShipMethod shipMethod, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IShipMethod>(shipMethod), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipMethodRepository(uow))
                {
                    repository.AddOrUpdate(shipMethod);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipMethod>(shipMethod), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipMethod"/> objects.
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="ShipMethod"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IShipMethod> shipMethodList, bool raiseEvents = true)
        {
            var shipMethodArray = shipMethodList as IShipMethod[] ?? shipMethodList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IShipMethod>(shipMethodArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipMethodRepository(uow))
                {
                    foreach (var shipMethod in shipMethodArray)
                    {
                        repository.AddOrUpdate(shipMethod);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipMethod>(shipMethodArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IShipMethod"/> object
        /// </summary>
        /// <param name="shipMethod">The <see cref="IShipMethod"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IShipMethod shipMethod, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IShipMethod>( shipMethod), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipMethodRepository(uow))
                {
                    repository.Delete( shipMethod);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipMethod>( shipMethod), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IShipMethod"/> objects
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="IShipMethod"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IShipMethod> shipMethodList, bool raiseEvents = true)
        {
            var shipMethodArray = shipMethodList as IShipMethod[] ?? shipMethodList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IShipMethod>(shipMethodArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipMethodRepository(uow))
                {
                    foreach (var shipMethod in shipMethodArray)
                    {
                        repository.Delete(shipMethod);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipMethod>(shipMethodArray), this);
        }

        /// <summary>
        /// Gets a ShipMethod by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the ShipMethod</param>
        /// <returns><see cref="IShipMethod"/></returns>
        public IShipMethod GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateShipMethodRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of ShipMethod give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IShipMethod> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateShipMethodRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        public IEnumerable<IShipMethod> GetAll()
        {
            using (var repository = _repositoryFactory.CreateShipMethodRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShipMethodService, DeleteEventArgs<IShipMethod>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShipMethodService, DeleteEventArgs<IShipMethod>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShipMethodService, SaveEventArgs<IShipMethod>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShipMethodService, SaveEventArgs<IShipMethod>> Saved;

        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IShipMethodService, NewEventArgs<IShipMethod>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShipMethodService, NewEventArgs<IShipMethod>> Created;

        #endregion


     
    }
}