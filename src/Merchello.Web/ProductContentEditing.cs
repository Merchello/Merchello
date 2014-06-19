namespace Merchello.Web
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Core;
    using Umbraco.Core.Models;
    using Umbraco.Web.Models.ContentEditing;

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
            var tab = new Tab<ContentPropertyDisplay>
            {
                Alias = Constants.ProductEditor.TabAlias,
                Label = Constants.ProductEditor.TabLabel,
                Id = Constants.ProductEditor.TabId,
                IsActive = true
            };

            AddProperties(ref tab);

            //Is there a better way?
            var tabs = new List<Tab<ContentPropertyDisplay>> {tab};
            tabs.AddRange(contentItem.Tabs);
            contentItem.Tabs = tabs;
        }

        private static bool ShouldAddProductEditor(ContentItemDisplay contentItem)
        {
            return contentItem.Properties.Any(p => p.Alias == Constants.ProductEditor.ProductKeyPropertyAlias);
        }

        private static void AddProperties(ref Tab<ContentPropertyDisplay> tab)
        {
            var listViewProperties = new List<ContentPropertyDisplay>
            {
                new ContentPropertyDisplay
                {
                    Alias =
                        string.Concat(Umbraco.Core.Constants.PropertyEditors.InternalGenericPropertiesPrefix,
                            Constants.ProductEditor.PropertyAlias),
                    // Must prefix with _umb_ so Umbraco knows not to save this property
                    Label = "",
                    Value = null,
                    //View = "../App_Plugins/Merchello/PropertyEditors/ProductPicker/Views/merchelloproductselector.html",
                    View = "../App_Plugins/Merchello/Backoffice/Merchello/ProductVariantEdit.html",
                    HideLabel = true,
                    Config = new Dictionary<string, object>()
                }
            };
            tab.Properties = listViewProperties;
        }
    }
}
