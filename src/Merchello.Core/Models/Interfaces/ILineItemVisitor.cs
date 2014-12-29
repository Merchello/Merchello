namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a line item visitor
    /// </summary>
    public interface ILineItemVisitor
    {
        /// <summary>
        /// Executes the "visit"
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <remarks>
        /// This is the Visitor design pattern.  PluralSight has some great intros
        /// </remarks>
        void Visit(ILineItem lineItem);
    }
}