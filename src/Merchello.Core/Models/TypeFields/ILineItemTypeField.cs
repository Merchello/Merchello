namespace Merchello.Core.Models.TypeFields
{
    public interface ILineItemTypeField : ITypeFieldMapper<LineItemType>
    {
        /// <summary>
        /// The product type
        /// </summary>
        ITypeField Product { get; } 
    }
}