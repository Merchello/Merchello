namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The entity collection provider element.
    /// </summary>
    public class EntityCollectionProviderElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [ConfigurationProperty("key", IsKey = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [ConfigurationProperty("icon", IsRequired = true)]
        public string Icon
        {
            get { return (string)this["icon"]; }
            set { this["icon"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tree is visible.
        /// </summary>
        [ConfigurationProperty("visible", IsRequired = false, DefaultValue = true)]
        public bool Visible
        {
            get
            {
                return (bool)this["visible"];
            }

            set
            {
                this["visible"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [ConfigurationProperty("ref", IsRequired = false, DefaultValue = "")]
        public string Ref
        {
            get { return (string)this["ref"]; }
            set { this["ref"] = value; }
        }
    }
}