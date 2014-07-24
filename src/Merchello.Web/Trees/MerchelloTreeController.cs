using Merchello.Core.Models;
using umbraco.cms.presentation;

namespace Merchello.Web.Trees
{
    using System.Linq;
    using System.Net.Http.Formatting;
    using Core.Configuration;
    using Core.Configuration.Outline;
    using umbraco;
    using umbraco.BusinessLogic.Actions;
    using umbraco.dialogs;
    using Umbraco.Web.Models.Trees;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.Trees;

    /// <summary>
    /// The merchello tree controller.
    /// </summary>
    [Tree("merchello", "merchello", "Merchello")]
    [PluginController("Merchello")]
    public class MerchelloTreeController : TreeController
    {
        /// <summary>
        /// The get tree nodes.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNodeCollection"/>.
        /// </returns>
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var collection = new TreeNodeCollection();

            var backoffice = MerchelloConfiguration.Current.BackOffice;

            var rootTrees = backoffice.GetTrees().Where(x => x.Visible);

            var currentTree = rootTrees.FirstOrDefault(x => x.Id == id && x.Visible);

            collection.AddRange(
                currentTree != null
                    ? currentTree.SubTree.GetTrees().Where(x => x.Visible)
                            .Select(tree => GetTreeNodeFromConfigurationElement(tree, queryStrings, currentTree))
                    : backoffice.GetTrees().Where(x => x.Visible)
                            .Select(tree => GetTreeNodeFromConfigurationElement(tree, queryStrings)));

            return collection;
        }

        /// <summary>
        /// The get menu for node.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="MenuItemCollection"/>.
        /// </returns>
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (id == "settings")
            {
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            }

            if (id == "orders")     
            {                                                                                                 
                menu.Items.Add<CreateChildEntity, ActionNew>("Create Order", true).Alias = "createOrder";
            }

            ////if (id == "catalog")
            ////{
            //    //create product
            ////    menu.Items.Add<MerchelloActionNewProduct>(ui.Text("actions", MerchelloActionNewProduct.Instance.Alias));
            ////}

            return menu;
        }

        /// <summary>
        /// The get tree node from configuration element.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <param name="parentTree">
        /// The parent tree.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode"/>.
        /// </returns>
        private TreeNode GetTreeNodeFromConfigurationElement(TreeElement tree, FormDataCollection queryStrings, TreeElement parentTree = null)
        {

            var hasSubs = tree.SubTree != null && tree.SubTree.GetTrees().Any();

            return CreateTreeNode(
                tree.Id,
                parentTree == null ? string.Empty : parentTree.Id,
                queryStrings,
                tree.Title,
                tree.Icon,
                hasSubs,
                tree.RoutePath);
        }
    }
}