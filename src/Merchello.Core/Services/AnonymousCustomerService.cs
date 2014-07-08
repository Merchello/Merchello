namespace Merchello.Core.Services
{
    using System.Collections.Generic;
    using System.Threading;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents an anonymous customer service.
    /// </summary>
    internal class AnonymousCustomerService : IAnonymousCustomerService
    {
         #region fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The unit of work provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
        /// </summary>
        public AnonymousCustomerService()
            : this(new RepositoryFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public AnonymousCustomerService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public AnonymousCustomerService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            
            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region Event Handlers


        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, Events.NewEventArgs<IAnonymousCustomer>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, Events.NewEventArgs<IAnonymousCustomer>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, SaveEventArgs<IAnonymousCustomer>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, SaveEventArgs<IAnonymousCustomer>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IAnonymousCustomerService, DeleteEventArgs<IAnonymousCustomer>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, DeleteEventArgs<IAnonymousCustomer>> Deleted;


        #endregion


        /// <summary>
        /// The create anonymous customer with key.
        /// </summary>
        /// <returns>
        /// The <see cref="IAnonymousCustomer"/>.
        /// </returns>
        public IAnonymousCustomer CreateAnonymousCustomerWithKey()
        {
            var anonymous = new AnonymousCustomer();

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    repository.AddOrUpdate(anonymous);
                    uow.Commit();
                }
            }

            return anonymous;
        }

        /// <summary>
        /// Saves the <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">
        /// The anonymous customer
        /// </param>
        public void Save(IAnonymousCustomer anonymous)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    repository.AddOrUpdate(anonymous);
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes the <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">
        /// The anonymous customer
        /// </param>
        public void Delete(IAnonymousCustomer anonymous)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    repository.Delete(anonymous);
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes a collection of <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymouses">
        /// The anonymous customers to be deleted
        /// </param>
        public void Delete(IEnumerable<IAnonymousCustomer> anonymouses)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();

                using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    foreach (var anonymous in anonymouses)
                    {
                        repository.Delete(anonymous);
                    }

                    uow.Commit();
                }
            }
        }
    }
}