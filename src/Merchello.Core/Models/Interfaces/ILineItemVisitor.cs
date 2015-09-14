namespace Merchello.Core.Models
{
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Defines a line item visitor
    /// </summary>
    public interface ILineItemVisitor : IVisitor<ILineItem>
    {
        ///// <summary>
        ///// Executes the "visit"
        ///// </summary>
        ///// <param name="lineItem">
        ///// The line item.
        ///// </param>
        ///// <remarks>
        ///// This is the Visitor design pattern.  PluralSight has some great intros
        ///// </remarks>
        //void Visit(ILineItem lineItem);
    }
}