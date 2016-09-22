namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Represents a provider that instantiates services.
    /// </summary>
    internal class ActivatorServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Gets an instance of a service.
        /// </summary>
        /// <param name="serviceType">
        /// The type of the service.
        /// </param>
        /// <returns>
        /// The instantiated service.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }
    }
}