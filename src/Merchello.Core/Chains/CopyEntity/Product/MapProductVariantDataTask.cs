namespace Merchello.Core.Chains.CopyEntity.Product
{
    using System.Linq;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Maps the variant specific information.
    /// </summary>
    internal sealed class MapProductVariantDataTask : CopyProductTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapProductVariantDataTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="original">
        /// The original <see cref="IProduct"/>.
        /// </param>
        public MapProductVariantDataTask(IMerchelloContext merchelloContext, IProduct original)
            : base(merchelloContext, original)
        {
        }

        /// <summary>
        /// The perform task.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IProduct> PerformTask(IProduct entity)
        {
            foreach (var origVariant in Original.ProductVariants.ToArray())
            {
                var cloneVariant = this.GetOrignalMatchingVariant(origVariant);

                cloneVariant.Barcode = origVariant.Barcode;
                cloneVariant.Available = false;
                cloneVariant.CostOfGoods = origVariant.CostOfGoods;
                cloneVariant.Download = origVariant.Download;
                cloneVariant.DownloadMediaId = origVariant.DownloadMediaId;
                cloneVariant.Height = origVariant.Height;
                cloneVariant.Length = origVariant.Length;
                cloneVariant.Weight = origVariant.Weight;
                cloneVariant.Width = origVariant.Width;
                cloneVariant.Manufacturer = origVariant.Manufacturer;
                cloneVariant.ManufacturerModelNumber = origVariant.ManufacturerModelNumber;
                cloneVariant.TrackInventory = origVariant.TrackInventory;
                cloneVariant.OutOfStockPurchase = origVariant.OutOfStockPurchase;
                cloneVariant.Shippable = origVariant.Shippable;
                cloneVariant.Taxable = origVariant.Taxable;
            }

            return Attempt<IProduct>.Succeed(entity);
        }
    }
}