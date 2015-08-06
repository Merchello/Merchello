namespace Merchello.Web.Trees
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http.Formatting;
    using Core.Configuration;
    using Core.Configuration.Outline;

    using Merchello.Web.Reporting;
    using Merchello.Web.Trees.Actions;

    using umbraco.BusinessLogic.Actions;

    using Umbraco.Core;
    using Umbraco.Core.Models.Membership;
    using Umbraco.Core.Services;
    using Umbraco.Web;
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
        /// The dialogs path.
        /// </summary>
        private static string _dialogsPath = "/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/";

        /// <summary>
        /// The text service.
        /// </summary>
        private readonly ILocalizedTextService _textService;

        /// <summary>
        /// The <see cref="IUser"/>.
        /// </summary>
        private readonly IUser _user;

        /// <summary>
        /// The <see cref="CultureInfo"/>.
        /// </summary>
        private readonly CultureInfo _culture;

        /// <summary>
        /// The root trees.
        /// </summary>
        private readonly IEnumerable<TreeElement> _rootTrees;

        /// <summary>
        /// The collection trees.
        /// </summary>
        private readonly string[] _collectiontrees = new[] { "products", "sales", "customers" };

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloTreeController"/> class.
        /// </summary>
        public MerchelloTreeController()
            : this(UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloTreeController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the Umbraco ApplicationContent is null
        /// </exception>
        public MerchelloTreeController(UmbracoContext context)
        {
            if (ApplicationContext == null) throw new NullReferenceException("Umbraco ApplicationContent is null");
            Mandate.ParameterNotNull(context, "context");

            //// http://issues.merchello.com/youtrack/issue/M-732
            _textService = ApplicationContext.Services.TextService;

            _culture = LocalizationHelper.GetCultureFromUser(context.Security.CurrentUser);

            _rootTrees = MerchelloConfiguration.Current.BackOffice.GetTrees().Where(x => x.Visible).ToArray();
        }

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
            var currentTree = _rootTrees.FirstOrDefault(x => x.Id == id && x.Visible);
            var isChildCollection = id.IndexOf('-') > 0;
            var collectionId = isChildCollection ? id.Split('-')[0] : id;
            var collectionKey = isChildCollection ? id.Split('-')[1] : null;

            collection.AddRange(
                currentTree != null
                    ? currentTree.Id == "reports" ? 
                        GetAttributeDefinedTrees(queryStrings) :
                        currentTree.SubTree.GetTrees().Where(x => x.Visible)
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

            // Products
            if (id == "products")
            {
                menu.Items.Add<NewEntityAction>(
                    _textService.Localize(string.Format("merchelloVariant/newProduct"), _culture),
                    false).NavigateToRoute("merchello/merchello/productedit/create");
            }

            if (id == "customers")
            {
                menu.Items.Add<NewEntityAction>(
                    _textService.Localize(string.Format("merchelloCustomers/newCustomer"), _culture), false)
                    .LaunchDialogView(_dialogsPath + "customer.newcustomer.html", _textService.Localize(string.Format("merchelloCustomers/newCustomer"), _culture));
            }

            //// child nodes will have an id separated with a hypen and key
            //// e.g.  products-[GUID]

            var isChildCollection = id.IndexOf('-') > 0;
            var collectionId = isChildCollection ? id.Split('-')[0] : id;

            if (_collectiontrees.Contains(collectionId))
            {
                if (isChildCollection)
                {
                    // add the delete button
                    menu.Items.Add<DeleteCollectionAction>(
                        _textService.Localize("actions/delete", _culture), false)
                        .LaunchDialogView(_dialogsPath + "delete.cshtml", _textService.Localize("actions/delete", _culture));
                }

                menu.Items.Add<NewCollectionAction>(
                    _textService.Localize(string.Format("merchelloCollections/newCollection"), _culture),
                    false,
                    new Dictionary<string, object>()
                        {
                            { "dialogData", new { tree = collectionId } }
                        }).LaunchDialogView(_dialogsPath + "create.staticcollection.html", _textService.Localize(string.Format("merchelloCollections/newCollection"), _culture));                
            }

            menu.Items.Add<RefreshNode, ActionRefresh>(_textService.Localize(string.Format("actions/{0}", ActionRefresh.Instance.Alias), _culture), _collectiontrees.Contains(collectionId));

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

            if (tree.Id == "reports" && hasSubs == false) hasSubs = ReportApiControllerResolver.Current.ResolvedTypes.Any();

            return CreateTreeNode(
                tree.Id,
                parentTree == null ? string.Empty : parentTree.Id,
                queryStrings,
                this.LocalizeTitle(tree),
                tree.Icon,
                hasSubs,
                tree.RoutePath);
        }

        /// <summary>
        /// The localize title.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string LocalizeTitle(TreeElement tree)
        {
            var name = tree.LocalizeName.IsNullOrWhiteSpace() ? tree.Id : tree.LocalizeName;

            var localized = _textService.Localize(string.Format("{0}/{1}", tree.LocalizeArea, name), _culture);

            return localized.IsNullOrWhiteSpace() ? tree.Title : localized;
        }

        /// <summary>
        /// Adds attribute defined trees.
        /// </summary>
        /// <param name="queryStrings">
        /// The query Strings.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> GetAttributeDefinedTrees(FormDataCollection queryStrings)
        {
            var types = ReportApiControllerResolver.Current.ResolvedTypes.ToArray();
            if (!types.Any()) return new TreeNode[] { };

            var atts = types.Select(x => x.GetCustomAttribute<BackOfficeTreeAttribute>(true)).OrderBy(x => x.SortOrder);

            return
                atts.Select(
                    att =>
                    CreateTreeNode(
                        att.RouteId,
                        att.ParentRouteId,
                        queryStrings,
                        att.Title,
                        att.Icon,
                        false,
                        string.Format("{0}{1}", "/merchello/merchello/reports.viewreport/", att.RoutePath)));
        }
    }
}