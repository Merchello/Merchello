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
        /// Gets or sets the localize area.
        /// </summary>
        [ConfigurationProperty("localizeArea", IsRequired = false, DefaultValue = "merchelloTree")]
        public string LocalizeArea
        {
            get
            {
                return (string)this["localizeArea"];
            }

            set
            {
                this["localizeArea"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the localize area.
        /// </summary>
        [ConfigurationProperty("localizeName", IsRequired = false, DefaultValue = "")]
        public string LocalizeName
        {
            get
            {
                return (string)this["localizeName"];
            }

            set
            {
                this["localizeName"] = value;
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
        [ConfigurationProperty("childSettings", IsRequired = false), ConfigurationCollection(typeof(SettingsCollection), AddItemName = "setting")]
        public SettingsCollection ChildSettings
        {
            get
            {
                return (SettingsCollection)this["childSettings"];
            }

            set
            {
                this["childSettings"] = value;
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

        /// <summary>
        /// Gets or sets the self managed entity collection provider collections.
        /// </summary>
        [ConfigurationProperty("selfManagedEntityCollectionProviders", IsRequired = false), ConfigurationCollection(typeof(EntityCollectionProviderCollection), AddItemName = "entityCollectionProvider")]
        public EntityCollectionProviderCollection SelfManagedEntityCollectionProviderCollections
        {
            get
            {
                return (EntityCollectionProviderCollection)this["selfManagedEntityCollectionProviders"];
            }

            set
            {
                this["selfManagedEntityCollectionProviders"] = value;
            }
        }
    }
}