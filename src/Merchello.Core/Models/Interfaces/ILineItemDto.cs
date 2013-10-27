using System;

namespace Merchello.Core.Models.Rdbms
{
    /// <summary>
    /// Defines a LineItemDto
    /// </summary>
    internal interface ILineItemDto
    {
        int Id { get; set; }
        int ContainerId { get; set; }
        Guid LineItemTfKey { get; set; }
        string Sku { get; set; }
        string Name { get; set; }
        int Quantity { get; set; }
        decimal Amount { get; set; }
        bool Exported { get; set; }
        string ExtendedData { get; set; }
        DateTime UpdateDate { get; set; }
        DateTime CreateDate { get; set; }              
    }
}