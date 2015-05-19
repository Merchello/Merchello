namespace Merchello.Core.Marketing.Offer
{
    using System.Linq;

    /// <summary>
    /// Utility extensions for the <see cref="OfferComponentDefinition"/> class.
    /// </summary>
    internal static class OfferComponentDefinitionExtensions
    {
        /// <summary>
        /// Maps the <see cref="OfferComponentDefinition"/> to a <see cref="OfferComponentConfiguration"/> so 
        /// that it can be serialized and saved to the database as JSON more easily.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="typeName">
        /// The type Name.
        /// </param>
        /// <returns>
        /// The <see cref="OfferComponentConfiguration"/>.
        /// </returns>
        internal static OfferComponentConfiguration AsOfferComponentConfiguration(this OfferComponentDefinition definition, string typeName)
        {
            return new OfferComponentConfiguration()
                {
                    ComponentKey = definition.ComponentKey,
                    TypeName = typeName,
                    Values = definition.ExtendedData.AsEnumerable()
                };
        }
    }
}