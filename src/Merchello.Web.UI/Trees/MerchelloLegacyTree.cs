using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using umbraco.businesslogic;
using umbraco.cms.presentation.Trees;
using umbraco.interfaces;

namespace Merchello.Web.UI.Trees
{
    [Tree("merchello", "merchelloTree", "Merchello Tree")]
    public class MerchelloLegacyTree : BaseTree
    {
        public MerchelloLegacyTree(string application)
            : base(application)
        { }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            rootNode.Icon = FolderIcon;
            rootNode.OpenIcon = FolderIconOpen;
            rootNode.NodeType = "init" + TreeAlias;
            rootNode.NodeID = "init";
        }

        public override void Render(ref XmlTree tree)
        {
            // Create tree node to allow sending a newsletter
            var sendNewsletter = XmlTreeNode.Create(this);
            sendNewsletter.Text = "Merchello Invoices";
            sendNewsletter.Icon = "docPic.gif";
            sendNewsletter.Action = "javascript:openInvoices()";
            // Add the node to the tree
            tree.Add(sendNewsletter);
        }

        public override void RenderJS(ref StringBuilder Javascript)
        {
            Javascript.Append(@"
            function openInvoices() {
               alert('clicked invoices');
            }
            ");
        }
    }
}