namespace Merchello.Core.Models
{
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a line item container.
    /// </summary>
    public interface ILineItemContainer : IVersionTaggedEntity, IHasCurrencyCode
    {
        /// <summary>
        /// Gets the collection of <see cref="ILineItem"/>
        /// </summary>
        
        //LineItemCollection Items { get; }
    }
}