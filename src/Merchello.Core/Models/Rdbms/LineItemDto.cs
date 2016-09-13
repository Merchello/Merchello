namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <inheritdoc/>
    internal abstract class LineItemDto : EntityDto, ILineItemDto
    {
        /// <inheritdoc/>
        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        /// <inheritdoc/>
        public abstract Guid ContainerKey { get; set; }

        /// <inheritdoc/>
        [Column("lineItemTfKey")]
        public virtual Guid LineItemTfKey { get; set; }

        /// <inheritdoc/>
        [Column("sku")]
        public virtual string Sku { get; set; }

        /// <inheritdoc/>
        [Column("name")]
        public string Name { get; set; }

        /// <inheritdoc/>
        [Column("quantity")]
        public int Quantity { get; set; }

        /// <inheritdoc/>
        [Column("price")]
        public decimal Price { get; set; }

        /// <inheritdoc/>
        [Column("exported")]
        public bool Exported { get; set; }
    }
}