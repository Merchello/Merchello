namespace Merchello.Core.DependencyInjection
{
    using System;
    using System.Linq;

    using LightInject;

    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Core.Persistence.Querying;
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
            // register mapping resover as IMappingResolver
            container.RegisterSingleton<IMappingResolver>(
                factory =>
                new MappingResolver(
                    factory.GetInstance<IServiceContainer>(),
                    factory.GetInstance<ILogger>(),
                    () => factory.GetInstance<IPluginManager>().ResolveBaseMappers()));

            // Query Factory
            container.Register<IQueryFactory, QueryFactory>();
        }
    }
}