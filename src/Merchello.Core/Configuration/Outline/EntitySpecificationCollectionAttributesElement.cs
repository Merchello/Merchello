namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// Provides a configuration element for assigning entity collection providers to be available for entity specification collection selection
    /// by entity type.
    /// </summary>
    public class EntitySpecificationCollectionAttributesElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the products entity collection providers collection
        /// </summary>
        [ConfigurationProperty("products", IsRequired = true), ConfigurationCollection(typeof(EntityCollectionProviderCollection), AddItemName = "entityCollectionProvider")]
        public EntityCollectionProviderCollection Products
        {
            get { return (EntityCollectionProviderCollection)this["products"]; }
        }
    }
}
