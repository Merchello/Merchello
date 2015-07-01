namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A intermediate class used in the serialization and deserialization of <see cref="OfferComponentDefinition"/>s.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class OfferComponentConfiguration
    {
        /// <summary>
        /// Gets or sets the offer settings key.
        /// </summary>
        /// <remarks>
        /// Useful in constraint / award attempts
        /// </remarks>
        [DataMember]
        public Guid OfferSettingsKey { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        /// <remarks>
        /// Useful in constraint / award attempts
        /// </remarks>
        [DataMember]
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the component key.
        /// </summary>
        [DataMember]
        public Guid ComponentKey { get; set; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        [DataMember]
        public string TypeFullName { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        [DataMember]
        public IEnumerable<KeyValuePair<string, string>> Values { get; set; }
    }
}