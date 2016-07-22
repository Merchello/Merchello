namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Counting;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;

    using RepositoryFactory = Merchello.Core.Persistence.RepositoryFactory;

    /// <summary>
    /// Represents a product option service.
    /// </summary>
    public class ProductOptionService : MerchelloRepositoryService, IProductOptionService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionService"/> class.
        /// </summary>
        public ProductOptionService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProductOptionService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProductOptionService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProductOptionService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
             : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
        /// </param>
        public ProductOptionService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IProductOptionService, Events.NewEventArgs<IProductOption>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IProductOptionService, Events.NewEventArgs<IProductOption>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IProductOptionService, SaveEventArgs<IProductOption>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IProductOptionService, SaveEventArgs<IProductOption>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IProductOptionService, DeleteEventArgs<IProductOption>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IProductOptionService, DeleteEventArgs<IProductOption>> Deleted;

#endregion

        /// <summary>
        /// Creates a <see cref="IProductOption"/> without saving it to the database.
        /// </summary>
        /// <param name="name">
        /// The option name.
        /// </param>
        /// <param name="shared">
        /// A value indicating whether or not this is a shared option (usable by multiple products).
        /// </param>
        /// <param name="required">
        /// The required.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        public IProductOption CreateProductOption(string name, bool shared = false, bool required = true, bool raiseEvents = true)
        {
            var option = new ProductOption(name)
                        {
                            UseName = name,
                            Shared = shared, 
                            Required = required
                        };

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IProductOption>(option), this))
            {
                option.WasCancelled = true;
                return option;
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IProductOption>(option), this);

            return option;
        }

        /// <summary>
        /// Creates a <see cref="IProductOption"/> and saves it to the database.
        /// </summary>
        /// <param name="name">
        /// The option name.
        /// </param>
        /// <param name="shared">
        /// A value indicating whether or not this is a shared option (usable by multiple products).
        /// </param>
        /// <param name="required">
        /// The required.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        public IProductOption CreateProductOptionWithKey(
            string name,
            bool shared = false,
            bool required = true,
            bool raiseEvents = true)
        {
            var option = CreateProductOption(name, shared, required, raiseEvents);

            Save(option);

            return option;
        }

        /// <summary>
        /// Saves a single product option.
        /// </summary>
        /// <param name="option">
        /// The option to be saved
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Save(IProductOption option, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IProductOption>(option), this))
                {
                    ((ProductOption)option).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductOptionRepository(uow))
                {
                    repository.AddOrUpdate(option);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IProductOption>(option), this);
        }

        /// <summary>
        /// Saves a collection of product options
        /// </summary>
        /// <param name="options">
        /// The collection of product options to be saved
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Save(IEnumerable<IProductOption> options, bool raiseEvents = true)
        {
            var optionsArray = options as IProductOption[] ?? options.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IProductOption>(optionsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductOptionRepository(uow))
                {
                    foreach (var option in optionsArray)
                    {
                        repository.AddOrUpdate(option);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IProductOption>(optionsArray), this);
        }

        /// <summary>
        /// Deletes a product option
        /// </summary>
        /// <param name="option">
        /// The option to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <remarks>
        /// This performs a check to ensure the option is valid to be deleted
        /// </remarks>
        public void Delete(IProductOption option, bool raiseEvents = true)
        {
            if (!EnsureSafeOptionDelete(option))
            {
                MultiLogHelper.Warn<ProductOptionService>("A ProductOption delete attempt was aborted.  The option cannot be deleted due to it being shared with one or more products.");
                return;
            }

            if (raiseEvents)
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IProductOption>(option), this))
                {
                    ((ProductOption)option).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateProductOptionRepository(uow))
                {

                    repository.Delete(option);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IProductOption>(option), this);
        }


        

        /// <summary>
        /// Gets a <see cref="IProductOption"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        public IProductOption GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateProductOptionRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }


        /// <summary>
        /// Gets a collection of <see cref="IProductOption"/> by a list of keys.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        public IEnumerable<IProductOption> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateProductOptionRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets the usage information about the product option.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOptionUseCount"/>.
        /// </returns>
        public IProductOptionUseCount GetProductOptionUseCount(IProductOption option)
        {
            using (var repository = RepositoryFactory.CreateProductOptionRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetProductOptionUseCount(option);
            }
        }

        /// <summary>
        /// Gets the number of occurrences that an option has been shared.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The count of option shares.
        /// </returns>
        public int GetProductOptionShareCount(IProductOption option)
        {
            using (var repository = RepositoryFactory.CreateProductOptionRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetSharedProductOptionCount(option.Key);
            }
        }

        /// <summary>
        /// Gets a page of <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <param name="sharedOnly">
        /// Limit to only shared options.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProductOption}"/>.
        /// </returns>
        public Page<IProductOption> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending, bool sharedOnly = true)
        {
            return GetPage(string.Empty, page, itemsPerPage, sortBy, sortDirection, sharedOnly);
        }

        /// <summary>
        /// Gets a page of <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="term">
        /// A search term to filter by
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <param name="sharedOnly">
        /// Limit to only shared options.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProductOption}"/>.
        /// </returns>
        public Page<IProductOption> GetPage(
            string term,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending,
            bool sharedOnly = true)
        {
            using (var repository = RepositoryFactory.CreateProductOptionRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetPage(term, page, itemsPerPage, sortBy, sortDirection, sharedOnly);
            }
        }

        /// <summary>
        /// Deletes a collection of product options
        /// </summary>
        /// <param name="options">
        /// The collection of product options to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <remarks>
        /// This performs a check to ensure the option is valid to be deleted
        /// 
        /// THIS is INTERNAL due to sharing policies
        /// 
        /// </remarks>
        internal void Delete(IEnumerable<IProductOption> options, bool raiseEvents = true)
        {
            var optionsArray = options as IProductOption[] ?? options.ToArray();
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IProductOption>(optionsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();

                using (var repository = RepositoryFactory.CreateProductOptionRepository(uow))
                {
                    foreach (var option in optionsArray)
                    {
                        repository.Delete(option);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IProductOption>(optionsArray), this);
        }

        /// <summary>
        /// Gets all the product options.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        internal IEnumerable<IProductOption> GetAll()
        {
            using (var repository = RepositoryFactory.CreateProductOptionRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Ensures the option is safe to delete.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the option can be deleted.
        /// </returns>
        private bool EnsureSafeOptionDelete(IProductOption option)
        {
            using (var repository = RepositoryFactory.CreateProductOptionRepository(UowProvider.GetUnitOfWork()))
            {
                var count = repository.GetSharedProductOptionCount(option.Key);

                return option.Shared ? count == 0 : count == 1;
            }
        }
    }
}