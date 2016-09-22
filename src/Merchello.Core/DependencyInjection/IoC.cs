namespace Merchello.Core.DependencyInjection
{
    using System;

    using LightInject;

    using Merchello.Core.Acquired.ObjectResolution;

    using Ensure = Merchello.Core.Ensure;

    /// <summary>
    /// An internal "service registry" singleton to store the Service Resolver.
    /// </summary>
    internal class IoC : SingleObjectResolverBase<IoC, IoC>
    {
        private readonly IServiceContainer _serviceRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="IoC"/> class.
        /// </summary>
        /// <param name="serviceRegistry">
        /// The container.
        /// </param>
        internal IoC(IServiceContainer serviceRegistry)
        {
            Ensure.ParameterNotNull(serviceRegistry, nameof(serviceRegistry));
           
            this._serviceRegistry = serviceRegistry;
        }

        /// <summary>
        /// Gets the <see cref="IServiceContainer"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// Throws an exception if the singleton has not been instantiated.
        /// </exception>
        public static IServiceContainer Container
        {
            get
            {
                if (!HasCurrent) throw new NullReferenceException("Singleton has not been instantiated");
                return Current._serviceRegistry;
            }
        }
    }
}