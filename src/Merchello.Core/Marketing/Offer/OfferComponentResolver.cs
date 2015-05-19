namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Umbraco.Core.ObjectResolution;

    /// <summary>
    /// Represents a discount constraint resolver.
    /// </summary>
    internal class OfferComponentResolver : ResolverBase<OfferComponentResolver>, IOfferComponentResolver
    {
        /// <summary>
        /// The instance types.
        /// </summary>
        private readonly List<Type> _instanceTypes = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentResolver"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public OfferComponentResolver(IEnumerable<Type> values)
        {
            this._instanceTypes = values.ToList();
        }

        /// <summary>
        /// Gets the instance types.
        /// </summary>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal IEnumerable<Type> InstanceTypes
        {
            get
            {
                return this._instanceTypes;
            }
        }

        public T GetComponent<T>(OfferComponentDefinition definition) where T : OfferComponentBase
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OfferComponentBase> GetComponents(IEnumerable<OfferComponentDefinition> definitions)
        {
            throw new NotImplementedException();
        }
    }
}