using System.Net.Http.Formatting;
using Umbraco.Core;
using Umbraco.Web.Trees;
using umbraco.BusinessLogic.Actions;
using Umbraco.Web.Trees.Menu;

namespace Merchello.Web.UI.Trees
{
    [Tree("merchello", "merchello", "Merchello Tree")]
    public class MerchelloTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            //we only support one tree level for data types
            //if (id != Constants.System.Root.ToInvariantString())
            //{
            //    throw new HttpResponseException(HttpStatusCode.NotFound);
            //}
            var collection = new TreeNodeCollection();
            if (id == "settings")
            {
                collection.Add(CreateTreeNode("regions", queryStrings, "Regions", "icon-autofill", false));
                collection.Add(CreateTreeNode("shipping", queryStrings, "Shipping", "icon-autofill", false));
                collection.Add(CreateTreeNode("taxation", queryStrings, "Taxation", "icon-autofill", false));
                collection.Add(CreateTreeNode("payment", queryStrings, "Payment", "icon-autofill", false));
                collection.Add(CreateTreeNode("debuglog", queryStrings, "Debug Log", "icon-autofill", false));
            }
            else
            {
                collection.Add(CreateTreeNode("catalog", queryStrings, "Catalog", "icon-autofill", false, "merchello/merchello/ListProducts/"));
                collection.Add(CreateTreeNode("invoice", queryStrings, "Invoice", "icon-autofill", false, "ListProducts.html"));
                collection.Add(CreateTreeNode("customers", queryStrings, "Customers", "icon-autofill", false, "ListProducts"));
                collection.Add(CreateTreeNode("reports", queryStrings, "Reports", "icon-autofill", false));
                collection.Add(CreateTreeNode("settings", queryStrings, "Settings", "icon-autofill", true));
            }

            //collection.AddRange(
            //    Services.DataTypeService.GetAllDataTypeDefinitions()
            //            .OrderBy(x => x.Name)
            //            .Select(dt =>
            //                    CreateTreeNode(
            //                        dt.Id.ToInvariantString(),
            //                        queryStrings,
            //                        dt.Name,
            //                        "icon-autofill",
            //                        false)));
            return collection;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (id == Constants.System.Root.ToInvariantString())
            {
                // root actions              
                menu.AddMenuItem<RefreshNode, ActionRefresh>(true);
                return menu;
            }
            else if (id == "catalog")
            {
                //create product
                menu.AddMenuItem<MerchelloActionNewProduct>();
            }
            else
            {
                //only have refres for each node
                menu.AddMenuItem<RefreshNode, ActionRefresh>(true);
            }

            return menu;
        }

    }
}