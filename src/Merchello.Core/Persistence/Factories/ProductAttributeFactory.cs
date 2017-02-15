namespace Merchello.Core.Persistence.Factories
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.Rdbms;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// Responsible for building <see cref="IProductAttribute"/> and <see cref="ProductAttributeDto"/>.
    /// </summary>
    internal class ProductAttributeFactory : IEntityFactory<IProductAttribute, ProductAttributeDto>
    {
        /// <summary>
        /// Builds the <see cref="IProductAttribute"/>.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IProductAttribute"/>.
        /// </returns>
        public IProductAttribute BuildEntity(ProductAttributeDto dto)
        {
            var values = dto.DetachedContentValues.IsNullOrWhiteSpace()
                             ? Enumerable.Empty<KeyValuePair<string, string>>()
                             : JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(dto.DetachedContentValues);

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
        /// Builds the <see cref="ProductAttributeDto"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeDto"/>.
        /// </returns>
        public ProductAttributeDto BuildDto(IProductAttribute entity)
        {
            return new ProductAttributeDto()
                {
                    Key = entity.Key,
                    OptionKey = entity.OptionKey,
                    Name = entity.Name,
                    Sku = entity.Sku,
                    SortOrder = entity.SortOrder,
                    IsDefaultChoice = entity.IsDefaultChoice,
                    DetachedContentValues = entity.DetachedDataValues.Any() ? 
                                                JsonConvert.SerializeObject(entity.DetachedDataValues.AsEnumerable()) :
                                                null,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}