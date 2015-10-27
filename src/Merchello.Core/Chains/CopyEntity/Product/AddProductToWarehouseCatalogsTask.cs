namespace Merchello.Core.Chains.CopyEntity.Product
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The add product to warehouse catalogs task.
    /// </summary>
    internal sealed class AddProductToWarehouseCatalogsTask : CopyProductTaskBase
    {
        /// <summary>
        /// The collection of <see cref="IWarehouseCatalog"/>.
        /// </summary>
        private Lazy<IEnumerable<IWarehouseCatalog>> _warehouseCatalogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddProductToWarehouseCatalogsTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="original">
        /// The original.
        /// </param>
        public AddProductToWarehouseCatalogsTask(IMerchelloContext merchelloContext, IProduct original)
            : base(merchelloContext, original)
        {
            this.Initialize();
        }

        /// <summary>
        /// Adds the product to the same warehouse catalogs as the original product.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IProduct> PerformTask(IProduct entity)
        {
            if (!Original.CatalogInventories.Any()) return Attempt<IProduct>.Succeed(entity);

            // there are generally not too many of these so just get them all to reduce the number of db trips
            var catalogKeys = Original.CatalogInventories.Select(x => x.CatalogKey).ToArray();
            foreach (var key in catalogKeys)
            {
                var catalog = _warehouseCatalogs.Value.FirstOrDefault(x => x.Key == key);
                if (catalog != null) entity.AddToCatalogInventory(catalog);
            }

            entity.ProductVariants.ForEach(this.AddToCatalog);

            return Attempt<IProduct>.Succeed(entity);
        }

        /// <summary>
        /// Adds the variants to the catalog.
        /// </summary>
        /// <param name="variant">
        /// The variant.
        /// </param>
        private void AddToCatalog(IProductVariant variant)
        {
            var matching = this.GetOrignalMatchingVariant(variant);
            var catalogKeys = matching.CatalogInventories.Select(x => x.CatalogKey).ToArray();

            foreach (var key in catalogKeys)
            {
                if (variant.CatalogInventories.All(x => x.CatalogKey != key))
                {
                    var catalog = _warehouseCatalogs.Value.FirstOrDefault(x => x.Key == key);
                    if (catalog != null) variant.AddToCatalogInventory(catalog);   
                }
            }
        }

        /// <summary>
        /// Initializes the task.
        /// </summary>
        private void Initialize()
        {
             _warehouseCatalogs = new Lazy<IEnumerable<IWarehouseCatalog>>(() => Services.WarehouseService.GetAllWarehouseCatalogs().ToArray());
        }
    }
}