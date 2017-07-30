namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <inheritdoc/>
    internal abstract class LineItemDto : ILineItemDto
    {
        /// <inheritdoc/>
        public string ExtendedData { get; set; }


        /// <inheritdoc/>
        public virtual Guid LineItemTfKey { get; set; }

        /// <inheritdoc/>
        public virtual string Sku { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public int Quantity { get; set; }

        /// <inheritdoc/>
        public decimal Price { get; set; }

        /// <inheritdoc/>
        public bool Exported { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}