using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Merchello.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Models.ContentEditing;

namespace Merchello.Web
{
    internal class ProductContentEditing
    {
        internal static void BindMappings()
        {
            Mapper.CreateMap<IContent, ContentItemDisplay>().AfterMap(HandleAfterMap);
        }

        internal static void HandleAfterMap(IContent content, ContentItemDisplay contentItem)
        {
            if (ShouldAddProductEditor(contentItem))
                AddTab(contentItem);
        }

        private static void AddTab(ContentItemDisplay contentItem)
        {
            var tab = new Tab<ContentPropertyDisplay>();
            tab.Alias = Constants.ProductEditor.TabAlias;
            tab.Label = Constants.ProductEditor.TabLabel;
            tab.Id = Constants.ProductEditor.TabId;
            tab.IsActive = true;

            AddProperties(ref tab);

            //Is there a better way?
            var tabs = new List<Tab<ContentPropertyDisplay>>();
            tabs.Add(tab);
            tabs.AddRange(contentItem.Tabs);
            contentItem.Tabs = tabs;
        }

        private static bool ShouldAddProductEditor(ContentItemDisplay contentItem)
        {
            return contentItem.Properties.Any(p => p.Alias == Constants.ProductEditor.ProductKeyPropertyAlias);
        }

        private static void AddProperties(ref Tab<ContentPropertyDisplay> tab)
        {
            var listViewProperties = new List<ContentPropertyDisplay>();
            listViewProperties.Add(new ContentPropertyDisplay
            {
                Alias = string.Concat(Umbraco.Core.Constants.PropertyEditors.InternalGenericPropertiesPrefix, Constants.ProductEditor.PropertyAlias), // Must prefix with _umb_ so Umbraco knows not to save this property
                Label = "",
                Value = null,
                View = "boolean",
                HideLabel = true,
                Config = new Dictionary<string, object>()
            });
            tab.Properties = listViewProperties;
        }
    }
}
