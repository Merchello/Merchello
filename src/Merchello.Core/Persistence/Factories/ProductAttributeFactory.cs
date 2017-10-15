namespace Merchello.Core.Persistence.Factories
{
    using Models;
    using Models.DetachedContent;
    using Models.Rdbms;

    /// <summary>
    ///     Responsible for building <see cref="IProductAttribute" /> and <see cref="ProductAttributeDto" />.
    /// </summary>
    internal class ProductAttributeFactory : IEntityFactory<IProductAttribute, ProductAttributeDto>
    {
        /// <summary>
        ///     Builds the <see cref="IProductAttribute" />.
        /// </summary>
        /// <param name="dto">
        ///     The dto.
        /// </param>
        /// <returns>
        ///     The <see cref="IProductAttribute" />.
        /// </returns>
        public IProductAttribute BuildEntity(ProductAttributeDto dto)
        {
            var values = DetachedContentValuesSerializationHelper.Deserialize(dto.DetachedContentValues);

            var valuesCollection = new DetachedDataValuesCollection(values);

            var attribute = new ProductAttribute(dto.Name, dto.Sku)
            {
                Key = dto.Key,
                OptionKey = dto.OptionKey,
                SortOrder = dto.SortOrder,
                IsDefaultChoice = dto.IsDefaultChoice,
                DetachedDataValues = valuesCollection,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            return attribute;
        }

        /// <summary>
        ///     Builds the <see cref="ProductAttributeDto" />.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <returns>
        ///     The <see cref="ProductAttributeDto" />.
        /// </returns>
        public ProductAttributeDto BuildDto(IProductAttribute entity)
        {
            return new ProductAttributeDto
            {
                Key = entity.Key,
                OptionKey = entity.OptionKey,
                Name = entity.Name,
                Sku = entity.Sku,
                SortOrder = entity.SortOrder,
                IsDefaultChoice = entity.IsDefaultChoice,
                DetachedContentValues = DetachedContentValuesSerializationHelper.Serialize(entity.DetachedDataValues),
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}