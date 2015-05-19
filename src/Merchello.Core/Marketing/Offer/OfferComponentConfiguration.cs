namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A intermediate class used in the serialization and deserialization of <see cref="OfferComponentDefinition"/>s.
    /// </summary>
    public class OfferComponentConfiguration
    {
        /// <summary>
        /// Gets or sets the component key.
        /// </summary>
        public Guid ComponentKey { get; set; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Values { get; set; }
    }
}