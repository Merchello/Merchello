namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.BackOffice;
    using Merchello.Core.EntityCollections;

    /// <summary>
    /// Represents a configuration element for parsing the back office tree configuration XML.
    /// </summary>
    internal class BackOfficeTreesElement : RawXmlConfigurationElement
    {
        /// <summary>
        /// Gets the collection of <see cref="IDashboardTreeNode"/> from the merchelloExtensibility configuration file.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IDashboardTreeNode}"/>.
        /// </returns>
        public IEnumerable<IDashboardTreeNode> GetTrees()
        {
            return RawXml.Elements("tree").Select(Build);
        }


        private IDashboardTreeNode Build(XElement xt)
        {
            // visibility
            var visible = xt.Attribute("visible").Value.TryConvertTo<bool>() ;

            // self managed providers before static providers
            var smb = (xt.Element("selfManagedProvidersBeforeStaticProviders")?.Value ?? "false").TryConvertTo<bool>();

            // sort order
            var sort = xt.Attribute("sortOrder").Value.TryConvertTo<int>();

            var node = new DashboardTreeNode
            {
                RouteId = xt.Attribute("id").Value,
                Icon = xt.Attribute("icon").Value,
                Title = xt.Attribute("title").Value,
                RoutePath = xt.Attribute("routePath").Value,
                SortOrder = sort.Success ? sort.Result : 0,
                Visible = !visible.Success || visible.Result,
                SelfManagedProvidersBeforeStaticProviders = smb.Success && smb.Result,
                SelfManagedEntityCollectionProviders = xt.Elements("selfManagedEntityCollectionProviders/entityCollectionProvider")
                            .Select(this.BuildLink)
            };
            

            return node;
        }

        private IDashboardTreeNodeKeyLink BuildLink(XElement xp)
        {
            var key = xp.Attribute("key").Value.TryConvertTo<Guid>();
            if (!key.Success) throw key.Exception;

            var visible = xp.Attribute("visible").Value.TryConvertTo<bool>();

            return new DashboardTreeNodeKeyLink
                       {
                           Key = key.Result,
                           Title = xp.Attribute("title").Value,
                           Icon = xp.Attribute("icon").Value,
                           Visible = !visible.Success || visible.Result
                       };
        }

    }
}