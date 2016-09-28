namespace Merchello.Core.Persistence
{
    using System;

    using LightInject;

    using Merchello.Core.Persistence.Repositories;
    using Merchello.Core.Persistence.UnitOfWork;

    /// <summary>
    /// Represents a factory responsible for instantiating repositories.
    /// </summary>
    internal class RepositoryFactory
    {
        /// <summary>
        /// The service container.
        /// </summary>
        private readonly IServiceContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class with a container.
        /// </summary>
        /// <param name="container">A container.</param>
        public RepositoryFactory(IServiceContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            this._container = container;
        }

        /// <summary>
        /// Creates a repository.
        /// </summary>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <param name="uow">A unit of work.</param>
        /// <param name="name">The optional name of the repository.</param>
        /// <returns>The created repository for the unit of work.</returns>
        public virtual TRepository CreateRepository<TRepository>(IDatabaseUnitOfWork uow, string name = null)
            where TRepository : IRepository
        {
            return string.IsNullOrWhiteSpace(name)
                ? this._container.GetInstance<IDatabaseUnitOfWork, TRepository>(uow)
                : this._container.GetInstance<IDatabaseUnitOfWork, TRepository>(uow, name);
        }
    }
}