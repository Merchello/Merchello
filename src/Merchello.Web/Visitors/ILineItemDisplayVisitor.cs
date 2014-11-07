namespace Merchello.Web.Visitors
{
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Defines a LineItemDisplayVisitor.
    /// </summary>
    public interface ILineItemDisplayVisitor
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
        void Visit(LineItemDisplayBase lineItem);
    }
}