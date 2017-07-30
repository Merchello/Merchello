namespace Merchello.Core.Models
{
    using Merchello.Core.Vistors;

    /// <summary>
    /// Represents a line item visitor.
    /// </summary>
    public interface ILineItemVisitor : IVisitor<ILineItem>
    {
    }
}