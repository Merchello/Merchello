namespace Merchello.Core
{
    using LightInject;

    using Merchello.Core.Acquired.ObjectResolution;
    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.SqlSyntax;

    /// <summary>
    /// An internal singleton to store the Service Resolver.
    /// </summary>
    internal class ServiceLocator : SingleObjectResolverBase<ServiceLocator, ServiceLocator>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        internal ServiceLocator(IServiceContainer container)
        {
            Ensure.ParameterNotNull(container, nameof(container));
           
            this.Container = container;
        }

        /// <summary>
        /// Gets the <see cref="IServiceContainer"/>.
        /// </summary>
        public IServiceContainer Container { get; }
    }
}