namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a single product option.
        /// </summary>
        /// <param name="option">
        /// The option to be saved
        /// </param>
        public void Save(IProductOption option)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a collection of product options
        /// </summary>
        /// <param name="options">
        /// The collection of product options to be saved
        /// </param>
        public void Save(IEnumerable<IProductOption> options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a product option
        /// </summary>
        /// <param name="option">
        /// The option to be deleted
        /// </param>
        /// <remarks>
        /// This performs a check to ensure the option is valid to be deleted
        /// </remarks>
        public void Delete(IProductOption option)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a collection of product options
        /// </summary>
        /// <param name="options">
        /// The collection of product options to be deleted
        /// </param>
        /// <remarks>
        /// This performs a check to ensure the option is valid to be deleted
        /// </remarks>
        public void Delete(IEnumerable<IProductOption> options)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        /// <returns>
        /// The <see cref="Page{IProductOption}"/>.
        /// </returns>
        public Page<IProductOption> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            throw new NotImplementedException();
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
        /// <returns>
        /// The <see cref="Page{IProductOption}"/>.
        /// </returns>
        public Page<IProductOption> GetPage(
            string term,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            throw new NotImplementedException();
        }
    }
}