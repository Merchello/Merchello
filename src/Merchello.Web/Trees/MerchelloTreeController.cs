namespace Merchello.Web.Trees
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http.Formatting;
    using Core.Configuration;
    using Core.Configuration.Outline;

    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Web.Models.ContentEditing.Collections;
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
        /// The <see cref="EntityCollectionProviderResolver"/>.
        /// </summary>
        private readonly EntityCollectionProviderResolver _resolver = EntityCollectionProviderResolver.Current;

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
            var splitId = new SplitRoutePath(id);

            collection.AddRange(
                currentTree != null
                    ? currentTree.Id == "reports" ? 
                        GetAttributeDefinedTrees(queryStrings) :  
    
                        _collectiontrees.Contains(splitId.CollectionId) ?

                        this.GetTreeNodesFromCollection(splitId.CollectionId, MakeCollectionRoutePathId(splitId.CollectionId, splitId.CollectionKey), queryStrings) 

                            :
                        currentTree.SubTree.GetTrees().Where(x => x.Visible)
                            .Select(tree => GetTreeNodeFromConfigurationElement(tree, queryStrings, currentTree))                            

                    : 
                    _collectiontrees.Contains(splitId.CollectionId) ?

                    this.GetTreeNodesFromCollection(splitId.CollectionId, MakeCollectionRoutePathId(splitId.CollectionId, splitId.CollectionKey), queryStrings, false) :

                    backoffice.GetTrees().Where(x => x.Visible)
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
                menu.Items.Add<NewCollectionAction>(
                    _textService.Localize(string.Format("merchelloVariant/newProduct"), _culture),
                    false).NavigateToRoute("merchello/merchello/productedit/create");
            }

            if (id == "customers")
            {
                menu.Items.Add<NewCollectionAction>(
                    _textService.Localize(string.Format("merchelloCustomers/newCustomer"), _culture), false)
                    .LaunchDialogView(_dialogsPath + "customer.newcustomer.html", _textService.Localize(string.Format("merchelloCustomers/newCustomer"), _culture));
            }

            if (id == "marketing")
            {
                menu.Items.Add<NewOfferSettingsAction>(
                    _textService.Localize("merchelloMarketing/newOffer", _culture),
                    false)
                    .LaunchDialogView(
                        _dialogsPath + "marketing.newofferproviderselection.html",
                        _textService.Localize("merchelloMarketing/newOffer", _culture));
            }

            //// child nodes will have an id separated with a hypen and key
            //// e.g.  products_[GUID]
            var splitId = new SplitRoutePath(id);

            if (_collectiontrees.Contains(splitId.CollectionId))
            {
                menu.Items.Add<NewCollectionAction>(
                    _textService.Localize("merchelloCollections/newCollection", _culture),
                    _collectiontrees.Contains(id),
                    new Dictionary<string, object>()
                        {
                            { "dialogData", new { entityType = splitId.CollectionId, parentKey = splitId.CollectionKey } }
                        }).LaunchDialogView(_dialogsPath + "create.staticcollection.html", _textService.Localize("merchelloCollections/newCollection", _culture));

                if (!_collectiontrees.Contains(id)) // don't show this on root nodes
                menu.Items.Add<ManageEntitiesAction>(
                    _textService.Localize("merchelloCollections/manageCollection", _culture),
                    false,
                    new Dictionary<string, object>()
                        {
                            { "dialogData", new { entityType = splitId.CollectionId, collectionKey = splitId.CollectionKey } }
                        }).LaunchDialogView(_dialogsPath + "addentity.staticcollection.html", _textService.Localize("merchelloCollections/manageCollection", _culture));

                menu.Items.Add<SortCollectionAction>(
                    _textService.Localize("actions/sort", _culture),
                    false,
                    new Dictionary<string, object>()
                        {
                             { "dialogData", new { entityType = splitId.CollectionId, parentKey = splitId.CollectionKey } }
                        }).LaunchDialogView(_dialogsPath + "sort.staticcollection.html", _textService.Localize("merchelloCollections/sortCollections", _culture));                

                if (splitId.IsChildCollection)
                {
                    // add the delete button
                    menu.Items.Add<DeleteCollectionAction>(
                        _textService.Localize("actions/delete", _culture), 
                        false,
                        new Dictionary<string, object>()
                            {
                                { "dialogData", new { entityType = splitId.CollectionId, collectionKey = splitId.CollectionKey } }
                            })
                        .LaunchDialogView(_dialogsPath + "delete.staticcollection.html", _textService.Localize("actions/delete", _culture));
                }                
            }

            menu.Items.Add<RefreshNode, ActionRefresh>(_textService.Localize(string.Format("actions/{0}", ActionRefresh.Instance.Alias), _culture), id != "gateways");
            
            return menu;
        }


        /// <summary>
        /// </summary>
        /// <param name="collectionId">
        /// The collection id.
        /// </param>
        /// <param name="collectionKey">
        /// Constructs the route path id for collection nodes.
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MakeCollectionRoutePathId(string collectionId, string collectionKey)
        {
            return collectionKey.IsNullOrWhiteSpace()
                       ? collectionId
                       : string.Format("{0}_{1}", collectionId, collectionKey);
        }



        /// <summary>
        /// Gets tree nodes for collections.
        /// </summary>
        /// <param name="collectionId">
        /// The collection id.
        /// </param>
        /// <param name="parentRouteId">
        /// The parent route id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <param name="collectionRoots">
        /// Indicates this is a collection root node
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> GetTreeNodesFromCollection(string collectionId, string parentRouteId, FormDataCollection queryStrings, bool collectionRoots = true)
        {

            var info = this.GetCollectionProviderInfo(collectionId);
            var splitId = new SplitRoutePath(parentRouteId);
            
            var collections = collectionRoots
                                  ? info.ManagedCollections.Where(x => x.ParentKey == null).OrderBy(x => x.SortOrder)
                                  : info.ManagedCollections.Where(x => x.ParentKey == splitId.CollectionKeyAsGuid())
                                        .OrderBy(x => x.SortOrder);


            if (!collections.Any()) return new TreeNode[] { };

            return
                collections.Select(
                    collection =>
                    CreateTreeNode(
                        MakeCollectionRoutePathId(collectionId, collection.Key.ToString()),
                        parentRouteId,
                        queryStrings,
                        collection.Name,
                        "icon-list",
                        info.ManagedCollections.Any(x => x.ParentKey == collection.Key),
                        string.Format("/merchello/merchello/{0}/{1}", info.ViewName, collection.Key)));
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
            if (_collectiontrees.Contains(tree.Id)) hasSubs = this.GetCollectionProviderInfo(tree.Id).ManagedCollections.Any();

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

        /// <summary>
        /// Resolves a provider and view for collection node.
        /// </summary>
        /// <param name="collectionId">
        /// The collection id.
        /// </param>
        /// <returns>
        /// The <see cref="CollectionProviderInfo"/>.
        /// </returns>
        private CollectionProviderInfo GetCollectionProviderInfo(string collectionId)
        {
            collectionId = collectionId.ToLowerInvariant();
            var info = new CollectionProviderInfo();

            switch (collectionId)
            {
                case "sales":
                    info.ManagedCollections =
                        _resolver.GetProviderAttribute<StaticInvoiceCollectionProvider>()
                            .ToEntityCollectionProviderDisplay().ManagedCollections;
                    info.ViewName = "saleslist";
                    break;
                case "customers":
                    info.ManagedCollections =
                        _resolver.GetProviderAttribute<StaticCustomerCollectionProvider>()
                            .ToEntityCollectionProviderDisplay().ManagedCollections;
                    info.ViewName = "customerlist";
                    break;
                default:
                    info.ManagedCollections =
                        _resolver.GetProviderAttribute<StaticProductCollectionProvider>()
                            .ToEntityCollectionProviderDisplay().ManagedCollections;
                    info.ViewName = "productlist";
                    break;
            }

            return info;
        }

        /// <summary>
        /// The split route path.
        /// </summary>
        private class SplitRoutePath
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SplitRoutePath"/> class.
            /// </summary>
            /// <param name="routePath">
            /// The route path.
            /// </param>
            public SplitRoutePath(string routePath)
            {
                IsChildCollection = routePath.IndexOf('_') > 0;
                CollectionId = IsChildCollection ? routePath.Split('_')[0] : routePath;
                CollectionKey = IsChildCollection ? routePath.Split('_')[1] : string.Empty;
            }

            /// <summary>
            /// Gets a value indicating whether is child collection.
            /// </summary>
            public bool IsChildCollection { get; private set; }
            
            /// <summary>
            /// Gets the collection id.
            /// </summary>
            public string CollectionId { get; private set; }

            /// <summary>
            /// Gets the collection key.
            /// </summary>
            public string CollectionKey { get; private set; }

            /// <summary>
            /// The collection key as guid.
            /// </summary>
            /// <returns>
            /// The <see cref="Guid"/>.
            /// </returns>
            public Guid? CollectionKeyAsGuid()
            {
                return !CollectionKey.IsNullOrWhiteSpace() ? new Guid(CollectionKey) as Guid? : null;
            }
        }

        /// <summary>
        /// The collection provider info.
        /// </summary>
        private class CollectionProviderInfo
        {
            /// <summary>
            /// Gets or sets the view name.
            /// </summary>
            public string ViewName { get; set; }

            /// <summary>
            /// Gets or sets the managed collections.
            /// </summary>
            public IEnumerable<EntityCollectionDisplay> ManagedCollections { get; set; } 
        }
    }
}