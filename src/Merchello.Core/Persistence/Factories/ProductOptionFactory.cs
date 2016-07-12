namespace Merchello.Core.Persistence.Factories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Responsible for building <see cref="IProductOption"/> and <see cref="ProductOptionDto"/>.
    /// </summary>
    internal class ProductOptionFactory : IEntityFactory<IProductOption, ProductOptionDto>
    {
       

        /// <summary>
        /// Builds the <see cref="IProductOption"/> entity.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        public IProductOption BuildEntity(ProductOptionDto dto)
        {
            var option = new ProductOption(dto.Name, dto.Required)
                {
                    Key = dto.Key,
                    SortOrder = dto.Product2ProductOptionDto == null ? 0 : dto.Product2ProductOptionDto.SortOrder,
                    Required = dto.Required,
                    Shared = dto.Shared,
                    DetachedContentTypeKey = dto.DetachedContentTypeKey,
                    SharedCount = dto.SharedCount,
                    UpdateDate = dto.UpdateDate,
                    CreateDate = dto.CreateDate
                };

            return option;
        }

        /// <summary>
        /// Builds the <see cref="ProductOptionDto"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionDto"/>.
        /// </returns>
        public ProductOptionDto BuildDto(IProductOption entity)
        {
            return new ProductOptionDto()
                {
                    Key = entity.Key,
                    Name = entity.Name,
                    Required = entity.Required,
                    Shared = entity.Shared,
                    DetachedContentTypeKey = entity.DetachedContentTypeKey,
                    SharedCount = entity.SharedCount,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}