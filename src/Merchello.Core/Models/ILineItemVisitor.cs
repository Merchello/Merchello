namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a line item vistor
    /// </summary>
    public interface ILineItemVisitor
    {
        void Visit(ILineItem lineItem);
    }
}