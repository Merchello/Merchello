namespace Merchello.Core.Persistence.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.Rdbms;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// A factory responsible for building product variant detached content models
    /// </summary>
    internal class ProductVariantDetachedContentFactory : IEntityFactory<IProductVariantDetachedContent, ProductVariantDetachedContentDto>
    {
        /// <summary>
        /// The detached content type factory.
        /// </summary>
        private readonly Lazy<DetachedContentTypeFactory> _detachedContentTypeFactory = new Lazy<DetachedContentTypeFactory>(() => new DetachedContentTypeFactory());

        /// <summary>
        /// Builds <see cref="IProductVariantDetachedContent"/>.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariantDetachedContent"/>.
        /// </returns>
        public IProductVariantDetachedContent BuildEntity(ProductVariantDetachedContentDto dto)
        {
            var detachedContentType = _detachedContentTypeFactory.Value.BuildEntity(dto.DetachedContentType);

            var values = GetDetachedContentKeyValues(dto);          

            var valuesCollection = new DetachedDataValuesCollection(values);

            var detachedContent = new ProductVariantDetachedContent(
                                        dto.ProductVariantKey, 
                                        detachedContentType, 
                                        dto.CultureName, 
                                        valuesCollection)
                                      {
                                          Key = dto.Key,
                                          Slug = dto.Slug ?? string.Empty,
                                          TemplateId = dto.TemplateId ?? 0,
                                          CanBeRendered = dto.CanBeRendered,
                                          CreateDate = dto.CreateDate,
                                          UpdateDate = dto.UpdateDate
                                      };

            detachedContent.ResetDirtyProperties();

            return detachedContent;
        }

        /// <summary>
        /// Builds the <see cref="ProductVariantDetachedContentDto"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDetachedContentDto"/>.
        /// </returns>
        public ProductVariantDetachedContentDto BuildDto(IProductVariantDetachedContent entity)
        {
            return new ProductVariantDetachedContentDto()
                       {
                           Key = entity.Key,
                           CultureName = entity.CultureName,
                           DetachedContentTypeKey = entity.DetachedContentType.Key,
                           ProductVariantKey = entity.ProductVariantKey,
                           Slug = !entity.Slug.IsNullOrWhiteSpace() ? entity.Slug : null,
                           TemplateId = entity.TemplateId > 0 ? entity.TemplateId : null,
                           CanBeRendered = entity.CanBeRendered,
                           Values = JsonConvert.SerializeObject(entity.DetachedDataValues.AsEnumerable()),
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate
                       };
        }

        /// <summary>
        /// The get detached content key values.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The collection of detached content values.
        /// </returns>
        public IEnumerable<KeyValuePair<string, string>> GetDetachedContentKeyValues(ProductVariantDetachedContentDto dto)
        {
            try
            {
                return dto.Values.IsNullOrWhiteSpace()
                                 ? Enumerable.Empty<KeyValuePair<string, string>>()
                                 : JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(dto.Values);
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<ProductVariantDetachedContentFactory>("Failed to deserialize detached content values", ex);
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

        }
    }
}