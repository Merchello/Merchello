namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The LineItemContainer interface.
    /// </summary>
    public interface ILineItemContainer : IVersionTaggedEntity
    {
        /// <summary>
        /// Gets the collection of <see cref="ILineItem"/>
        /// </summary>
        [DataMember]
        LineItemCollection Items { get; }

        /// <summary>
        /// Accepts visitor class to visit invoice line items
        /// </summary>
        /// <param name="visitor">The <see cref="ILineItemVisitor"/> class</param>
        void Accept(ILineItemVisitor visitor);
    }
}