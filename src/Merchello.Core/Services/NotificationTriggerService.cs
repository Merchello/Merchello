using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    internal class NotificationTriggerService : INotificationTriggerService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        
        public NotificationTriggerService()
            : this(new RepositoryFactory())
        { }

        public NotificationTriggerService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public NotificationTriggerService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Returns a collection of all <see cref="INotificationTrigger"/>
        /// </summary>
        public IEnumerable<INotificationTrigger> GetAll()
        {
            using (var repository = _repositoryFactory.CreateNotificationTriggerRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        internal static string GetBindingValue(Type service, Type eventArg)
        {
            return string.Format("{0}|{1}", service.Name, eventArg.Name);
        }
    }
}