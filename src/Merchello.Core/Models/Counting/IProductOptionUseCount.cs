namespace Merchello.Core.Models.Counting
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the usage of an option.
    /// </summary>
    public interface IProductOptionUseCount
    {
        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        [DataMember]
        EntityUseCount Option { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the option is shared.
        /// </summary>
        [DataMember]
        bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        [DataMember]
        IEnumerable<EntityUseCount> Choices { get; set; }
    }
}