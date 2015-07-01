namespace Merchello.Core.Persistence.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    using Newtonsoft.Json;

    /// <summary>
    /// A factory responsible for building offer settings and it's respective DTO object.
    /// </summary>
    internal class OfferSettingsFactory : IEntityFactory<IOfferSettings, OfferSettingsDto>
    {
        /// <summary>
        /// Builds a <see cref="IOfferSettings"/> given an <see cref="OfferSettingsDto"/>
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        public IOfferSettings BuildEntity(OfferSettingsDto dto)
        {
            var configurations = JsonConvert.DeserializeObject<IEnumerable<OfferComponentConfiguration>>(dto.ConfigurationData);

            var definitionCollection = new OfferComponentDefinitionCollection();
            foreach (var config in configurations)
            {
                definitionCollection.Add(new OfferComponentDefinition(config));
            }

            var settings = new OfferSettings(dto.Name, dto.OfferCode, dto.OfferProviderKey, definitionCollection)
                {
                    Key = dto.Key,
                    Active = dto.Active,
                    OfferStartsDate = dto.OfferStartsDate.ConvertDateTimeNullToMinValue(),
                    OfferEndsDate = dto.OfferEndsDate.ConvertDateTimeNullToMaxValue(),
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };

            settings.ResetDirtyProperties();

            return settings;
        }

        /// <summary>
        /// Responsible for building the <see cref="OfferSettingsDto"/>
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="OfferSettingsDto"/>.
        /// </returns>
        public OfferSettingsDto BuildDto(IOfferSettings entity)
        {
            var configurations = entity.ComponentDefinitions.Select(x => x.AsOfferComponentConfiguration()).ToArray();

            return new OfferSettingsDto()
                       {
                           Key = entity.Key,
                           Name = entity.Name,
                           OfferCode = entity.OfferCode,
                           OfferProviderKey = entity.OfferProviderKey,
                           OfferStartsDate = entity.OfferStartsDate.ConverDateTimeMinValueToNull(),
                           OfferEndsDate = entity.OfferEndsDate.ConvertDateTimeMaxValueToNull(),
                           Active = entity.Active,
                           ConfigurationData = JsonConvert.SerializeObject(configurations),
                           UpdateDate = entity.UpdateDate,
                           CreateDate = entity.CreateDate
                       };
        }
    }
}