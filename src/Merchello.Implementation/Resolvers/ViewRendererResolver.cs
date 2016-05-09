namespace Merchello.Implementation.Resolvers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Web.Mvc;

    using Umbraco.Core;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    using CallingConventions = System.Reflection.CallingConventions;

    /// <summary>
    /// Resolver for ViewRenders.
    /// </summary>
    internal class ViewRendererResolver
    {
        /// <summary>
        /// The singleton instance of the view renderer.
        /// </summary>
        private static ViewRendererResolver _current;

        /// <summary>
        /// The _cache.
        /// </summary>
        private readonly CacheHelper _cache;

        /// <summary>
        /// The instance types.
        /// </summary>
        private readonly IEnumerable<Type> _instanceTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRendererResolver"/> class. 
        /// </summary>
        /// <param name="types">
        /// The list of resolved types.
        /// </param>
        /// <param name="cache">
        /// The <see cref="CacheHelper"/>.
        /// </param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal ViewRendererResolver(IEnumerable<Type> types, CacheHelper cache)
        {
            var instanceTypes = types as Type[] ?? types.ToArray();
            Mandate.ParameterNotNull(cache, "cache");

            this._cache = cache;
            this._instanceTypes = instanceTypes;
        }

        /// <summary>
        /// Gets a value indicating whether the singleton has been instantiated.
        /// </summary>
        public static bool HasCurrent
        {
            get
            {
                return _current != null;
            }
        }

        /// <summary>
        /// Gets or sets the current instance.
        /// </summary>
        public static ViewRendererResolver Current
        {
            get
            {
                return _current;
            }

            internal set
            {
                _current = value;
            }
        }
    }
}