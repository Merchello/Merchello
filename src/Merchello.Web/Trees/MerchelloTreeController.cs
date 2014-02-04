using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Merchello.Web.Trees
{
    [Tree("merchello", "merchello", "Merchello")]
    [PluginController("Merchello")]
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
                collection.Add(CreateTreeNode("shipping", "settings", queryStrings, "Shipping", "icon-truck", false, "merchello/merchello/Shipping/manage"));
                //collection.Add(CreateTreeNode("vendors", "settings", queryStrings, "Vendors", "icon-handshake", false, "merchello/merchello/Vendors/manage"));
                collection.Add(CreateTreeNode("taxation", "settings", queryStrings, "Taxation", "icon-piggy-bank", false, "merchello/merchello/Taxation/manage"));
                collection.Add(CreateTreeNode("payment", "settings", queryStrings, "Payment", "icon-bill-dollar", false, "merchello/merchello/Payment/manage"));
                collection.Add(CreateTreeNode("notifications", "settings", queryStrings, "Notifications", "icon-chat", false, "merchello/merchello/Notifications/manage"));
                //collection.Add(CreateTreeNode("debuglog", "settings", queryStrings, "Debug Log", "icon-alert", false, "merchello/merchello/Debug/manage"));
            }
            else if (id == "reports")
            {
                collection.Add(CreateTreeNode("salesOverTime", "reports", queryStrings, "Sales Over Time", "icon-loading", false, "merchello/merchello/SalesOverTime/manage"));
                collection.Add(CreateTreeNode("salesByItem", "reports", queryStrings, "Sales By Item", "icon-barcode", false, "merchello/merchello/SalesByItem/manage"));
                collection.Add(CreateTreeNode("taxesByDestination", "reports", queryStrings, "Taxes By Destination", "icon-piggy-bank", false, "merchello/merchello/TaxesByDestination/manage"));
            }
            else
            {
                collection.Add(CreateTreeNode("catalog", "", queryStrings, "Catalog", "icon-barcode", false, "merchello/merchello/ProductList/manage"));
                collection.Add(CreateTreeNode("orders", "", queryStrings, "Orders", "icon-receipt-dollar", false, "merchello/merchello/OrderList/manage"));
                //collection.Add(CreateTreeNode("customers", "", queryStrings, "Customers", "icon-user", false, "merchello/merchello/CustomerList/manage"));
                collection.Add(CreateTreeNode("reports", "", queryStrings, "Reports", "icon-bar-chart", true, "merchello/merchello/Reports/manage"));
                collection.Add(CreateTreeNode("settings", "", queryStrings, "Settings", "icon-settings", true, "merchello/merchello/Settings/manage"));
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

        // TODO : Umbraco Refactored the TreeNodeController and it's underlying collections.  This
        // TODO : broke with the introduction of the Umbraco Nightly Build 111
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (id == "settings")
            {
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            }

            //if (id == Constants.System.Root.ToInvariantString())
            //{
            //    // root actions              
            //    menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            //    return menu;
            //}
            //else if (id == "catalog")
            //{
            //    //create product
            //    menu.Items.Add<MerchelloActionNewProduct>(ui.Text("actions", MerchelloActionNewProduct.Instance.Alias));
            //}
            //else
            //{
                //only have refres for each node
             //menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            //}

            return menu;
        }

    }
}