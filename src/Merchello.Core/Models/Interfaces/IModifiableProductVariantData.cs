namespace Merchello.Core.Models
{
    using NodaMoney;

    /// <summary>
    /// Represents modifiable product data.
    /// </summary>
    /// <remarks>
    /// 1.9.1 used in Data Modifier layer in the MerchelloHelper
    /// </remarks>
    public interface IProductVariantDataModifierData : IDataModifierData 
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        Money Price { get; set; }

        /// <summary>
        /// Gets or sets the sale price.
        /// </summary>
        Money SalePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether on sale.
        /// </summary>
        bool OnSale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether available.
        /// </summary>
        bool Available { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether track inventory.
        /// </summary>
        bool TrackInventory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shippable.
        /// </summary>
        bool Shippable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether taxable.
        /// </summary>
        bool Taxable { get; set; }
    }
}