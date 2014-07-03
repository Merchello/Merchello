namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The tree element.
    /// </summary>
    public class TreeElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        [ConfigurationProperty("id", IsKey = true)]
        public string Id
        {
            get { return (string)this["id"]; }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        [ConfigurationProperty("title", IsRequired = true)]
        public string Title
        {
            get
            {
                return (string)this["title"];
            }
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        [ConfigurationProperty("icon", IsRequired = true)]
        public string Icon
        {
            get
            {
                return (string)this["icon"];
            }
        }

        /// <summary>
        /// Gets the route path.
        /// </summary>
        [ConfigurationProperty("routePath", IsRequired = true)]
        public string RoutePath
        {
            get
            {
                return (string)this["routePath"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tree is visible.
        /// </summary>
        [ConfigurationProperty("visible", IsRequired = false, DefaultValue = true)]
        public bool Visible
        {
            get
            {
                return (bool)this["visible"];
            }
        }

        /// <summary>
        /// Gets the sub tree.
        /// </summary>
        [ConfigurationProperty("subTree", IsRequired = false), ConfigurationCollection(typeof(TreeCollection), AddItemName = "tree")]
        public TreeCollection SubTree
        {
            get { return (TreeCollection)this["subTree"]; }
        }
    }
}