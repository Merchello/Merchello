namespace Merchello.Core
{
    using LightInject;

    using Merchello.Core.Acquired.ObjectResolution;
    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.SqlSyntax;

    /// <summary>
    /// An internal "service registry" singleton to store the Service Resolver.
    /// </summary>
    internal class SR : SingleObjectResolverBase<SR, SR>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SR"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        internal SR(IServiceContainer container)
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