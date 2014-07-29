namespace Merchello.Core.Persistence.Factories
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// The catalog inventory factory.
    /// </summary>
    internal class CatalogInventoryFactory : IEntityFactory<ICatalogInventory, CatalogInventoryDto>
    {
        /// <summary>
        /// Builds a <see cref="ICatalogInventory"/> from a dto.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ICatalogInventory"/>.
        /// </returns>
        public ICatalogInventory BuildEntity(CatalogInventoryDto dto)
        {           
            return new CatalogInventory(dto.CatalogKey, dto.ProductVariantKey)
                {
                    Count = dto.Count,
                    LowCount = dto.LowCount,
                    Location = dto.Location,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };           
        }

        /// <summary>
        /// Builds a dto from the <see cref="ICatalogInventory"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogInventoryDto"/>.
        /// </returns>
        public CatalogInventoryDto BuildDto(ICatalogInventory entity)
        {
            return new CatalogInventoryDto()
                {
                    CatalogKey = entity.CatalogKey,
                    ProductVariantKey = entity.ProductVariantKey,
                    Count = entity.Count,
                    LowCount = entity.LowCount,
                    Location = entity.Location,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}