namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The tree element.
    /// </summary>
    public class TreeElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [ConfigurationProperty("id", IsKey = true)]
        public string Id
        {
            get
            {
                return (string)this["id"];
            }

            set
            {
                this["id"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [ConfigurationProperty("title", IsRequired = true)]
        public string Title
        {
            get
            {
                return (string)this["title"];
            }

            set
            {
                this["title"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        [ConfigurationProperty("icon", IsRequired = true)]
        public string Icon
        {
            get
            {
                return (string)this["icon"];
            }

            set
            {
                this["icon"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the route path.
        /// </summary>
        [ConfigurationProperty("routePath", IsRequired = true)]
        public string RoutePath
        {
            get
            {
                return (string)this["routePath"];
            }

            set
            {
                this["routePath"] = value;
            }
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
        /// Gets or sets the sort order.
        /// </summary>
        [ConfigurationProperty("sortOrder", IsRequired = false, DefaultValue = 0)]
        public int SortOrder
        {
            get
            {
                return (int)this["sortOrder"];
            }

            set
            {
                this["sortOrder"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sub tree.
        /// </summary>
        [ConfigurationProperty("subTree", IsRequired = false), ConfigurationCollection(typeof(TreeCollection), AddItemName = "tree")]
        public TreeCollection SubTree
        {
            get
            {
                return (TreeCollection)this["subTree"];
            }

            set
            {
                this["subTree"] = value;
            }
        }
    }
}