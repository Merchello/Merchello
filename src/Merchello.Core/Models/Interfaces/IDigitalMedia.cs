namespace Merchello.Core.Models.Interfaces
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    ///// TODO SR - This is just a scaffold for example.  Do whatever you need to do =)

    /// <summary>
    /// Represents digital media.
    /// </summary>
    public interface IDigitalMedia : IEntity, IHasExtendedData
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        
        Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the reference to an order
        /// </summary>
        
        DateTime? FirstAccessed { get; set; }
    }
}