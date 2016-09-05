namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The strategies collection.
    /// </summary>
    public class StrategiesCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="StrategyElement"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new StrategyElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element"> Then <see cref="TypeFieldElement"/></param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StrategyElement) element).Alias;
        }

        /// <summary>
        /// Default. Returns the SettingsItemsElement with the index of index from the collection
        /// </summary>
        public StrategyElement this[object index]
        {
            get { return (StrategyElement)this.BaseGet(index); }
        }
    }
}