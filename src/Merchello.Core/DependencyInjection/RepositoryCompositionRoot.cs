namespace Merchello.Core.DependencyInjection
{
    using LightInject;

    using Merchello.Core.Logging;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Core.Persistence.Migrations.Initial;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.Repositories;
    using Merchello.Core.Persistence.UnitOfWork;
    using Merchello.Core.Plugins;

    /// <summary>
    /// Sets the IoC container for the Merchello data layer/repositories/sql/database/etc...
    /// </summary>
    public sealed class RepositoryCompositionRoot : ICompositionRoot
    {
        /// <summary>
        /// Composes configuration services by adding services to the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public void Compose(IServiceRegistry container)
        {
            container.Register<IDatabaseSchemaCreation, DatabaseSchemaCreation>();

            // register repository factory
            container.RegisterSingleton<RepositoryFactory>();

            container.RegisterSingleton<IDatabaseUnitOfWorkProvider, NPocoUnitOfWorkProvider>();

            // register mapping resover as IMappingResolver
            container.RegisterSingleton<IMappingResolver>(
                factory =>
                new MappingResolver(
                    factory.GetInstance<IServiceContainer>(),
                    factory.GetInstance<ILogger>(),
                    () => factory.GetInstance<IPluginManager>().ResolveBaseMappers()));

            // Query Factory
            container.Register<IQueryFactory, QueryFactory>();

            // container.Register<IDatabaseUnitOfWork, NPocoUnitOfWork>();

            // resolve ctor dependency from GetInstance() runtimeArguments, if possible - 'factory' is
            // the container, 'info' describes the ctor argument, and 'args' contains the args that
            // were passed to GetInstance() - use first arg if it is the right type,
            //
            // for IDatabaseUnitOfWork
            container.RegisterConstructorDependency((factory, info, args) => args.Length > 0 ? args[0] as IDatabaseUnitOfWork : null);
            // for IUnitOfWork
            container.RegisterConstructorDependency((factory, info, args) => args.Length > 0 ? args[0] as IUnitOfWork : null);

            // register repositories
            // repos depend on various things, and a IDatabaseUnitOfWork (registered above)
            // some repositories have an annotated ctor parameter to pick the right cache helper
            container.Register<IMigrationStatusRepository, MigrationStatusRepository>();
        }
    }
}