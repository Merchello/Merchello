namespace Merchello.Web.Search
{
    using System;

    /// <summary>
    /// Defines a ProxyEntityServiceResolver.
    /// </summary>
    internal interface IProxyQueryManager
    {
        /// <summary>
        /// Creates an instance of a service.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service to be returned
        /// </typeparam>
        /// <returns>
        /// The service.
        /// </returns>
        TService Instance<TService>()
            where TService : IEntityProxyQuery, new();

        /// <summary>
        /// Creates an instance of a service with arguments
        /// </summary>
        /// <param name="constructorArgumentValues">
        /// The constructor argument values.
        /// </param>
        /// <typeparam name="TService">
        /// The type of the service to resolve
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if service cannot be resolved
        /// </exception>
        TService Instance<TService>(object[] constructorArgumentValues)
            where TService : class, IEntityProxyQuery;
    }
}