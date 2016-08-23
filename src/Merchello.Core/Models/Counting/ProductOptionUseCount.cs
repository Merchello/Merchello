namespace Merchello.Core.Models.Counting
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the usage of an option.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductOptionUseCount : IProductOptionUseCount
    {
        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        [DataMember]
        public EntityUseCount Option { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the option is shared.
        /// </summary>
        [DataMember]
        public bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        [DataMember]
        public IEnumerable<EntityUseCount> Choices { get; set; } 
    }
}