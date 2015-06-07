namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The offer component definition display.
    /// </summary>
    public class OfferComponentDefinitionDisplay
    {
        ///// <summary>
        ///// Gets or sets the key.
        ///// </summary>
        //public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the offer settings key.
        /// </summary>
        public Guid OfferSettingsKey { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the component key.
        /// </summary>
        public Guid ComponentKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether requires configuration.
        /// </summary>
        public bool RequiresConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        public string TypeFullName { get; set; }

        /// <summary>
        /// Gets or sets the Type to which this component can be grouped with
        /// </summary>
        public string TypeGrouping { get; set; }

        /// <summary>
        /// Gets or sets the component type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OfferComponentType ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the display configuration format.
        /// This text is used by the back office UI to display configured values
        /// </summary>
        public string DisplayConfigurationFormat { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the editor view.
        /// </summary>
        public DialogEditorViewDisplay DialogEditorView { get; set; }

        /// <summary>
        /// Gets or sets the type to which this component is restricted (if any)
        /// </summary>
        public string RestrictToType { get; set; } 
    }

    /// <summary>
    /// Utility class for mapping <see cref="OfferComponentDefinition"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class OfferComponentDefinitionDisplayExtensions
    {
        /// <summary>
        /// The to offer component definition display.
        /// </summary>
        /// <param name="component">
        /// The offer component
        /// </param>
        /// <returns>
        /// The <see cref="OfferComponentDefinitionDisplay"/>.
        /// </returns>
        public static OfferComponentDefinitionDisplay ToOfferComponentDefinitionDisplay(this OfferComponentBase component)
        {
            return AutoMapper.Mapper.Map<OfferComponentBase, OfferComponentDefinitionDisplay>(component);
        }

        /// <summary>
        /// Maps a <see cref="OfferComponentDefinitionDisplay"/> to a <see cref="OfferComponentDefinition"/>.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="OfferComponentDefinition"/>.
        /// </returns>
        public static OfferComponentDefinition ToOfferComponentDefinition(this OfferComponentDefinitionDisplay definition)
        {
            return
                new OfferComponentDefinition(
                    new OfferComponentConfiguration()
                        {
                            OfferSettingsKey = definition.OfferSettingsKey,
                            OfferCode = definition.OfferCode,
                            ComponentKey = definition.ComponentKey,
                            TypeFullName = definition.TypeFullName,
                            Values = definition.ExtendedData.AsEnumerable()
                        });
        }

        /// <summary>
        /// Creates an <see cref="OfferComponentDefinitionCollection"/> from a collection of <see cref="OfferComponentDefinitionDisplay"/>.
        /// </summary>
        /// <param name="definitions">
        /// The definitions.
        /// </param>
        /// <returns>
        /// The <see cref="OfferComponentDefinitionCollection"/>.
        /// </returns>
        public static OfferComponentDefinitionCollection AsOfferComponentDefinitionCollection(this IEnumerable<OfferComponentDefinitionDisplay> definitions)
        {
            var collection = new OfferComponentDefinitionCollection();
            foreach (var d in definitions.Where(x => x != null))
            {
                collection.Add(d.ToOfferComponentDefinition());
            }
            return collection;
        }
    }
}