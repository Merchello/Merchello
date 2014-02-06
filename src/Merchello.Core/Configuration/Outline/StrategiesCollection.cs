using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class StrategiesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new StrategyElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element"><see cref="TypeFieldElement"/></param>
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

    public class StrategyElement : ConfigurationElement
    {
        /// <summary>
        /// Gets/sets the alias (key) value for the strategies collection element.
        /// </summary>
        [ConfigurationProperty("alias", IsKey = true)]
        public string Alias
        {
            get { return (string)this["alias"]; }
            set { this["alias"] = value; }
        }

        /// <summary>
        /// Gets/sets the type associated with the setting.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
    }
}