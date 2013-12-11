using System;
using System.Collections.Generic;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.UnitOfWork;

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
    }
}