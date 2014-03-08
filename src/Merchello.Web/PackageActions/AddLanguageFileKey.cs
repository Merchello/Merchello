using System;
using System.Xml;
using Merchello.Core.Configuration;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace Merchello.Web.PackageActions
{
    /// <summary>
    /// This package action will add a key to one of the language files.
    /// </summary>
    /// <remarks>
    /// This package action has been customized from the PackageActionsContrib Project.
    /// http://packageactioncontrib.codeplex.com
    /// </remarks>
    public class AddLanguageFileKey : IPackageAction
    {
        // Set the path of the language files directory
        private const string UmbracoLangPath = "~/umbraco/config/lang/{0}.xml";

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML.
        /// </summary>
        /// <returns>The Alias of the package action.</returns>
        public string Alias()
        {
            return string.Concat(MerchelloConfiguration.ApplicationName, "_AddLanguageFileKey");
        }

        /// <summary>
        /// Executes the specified package name.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <returns></returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            // get attribute values of xmlData
            var requiredFields = new[] { "area", "key", "value" };
            foreach (var field in requiredFields)
            {
                if (xmlData.Attributes[field] == null)
                    return false;
            }

            var area = xmlData.Attributes["area"].Value;
            var key = xmlData.Attributes["key"].Value;
            var value = xmlData.Attributes["value"].Value;
            var language = xmlData.Attributes["language"] != null ? xmlData.Attributes["language"].Value : "en";
            var position = xmlData.Attributes["position"] != null ? xmlData.Attributes["position"].Value : null;

            // create a new xml document
            var document = new XmlDocument() { PreserveWhitespace = true };
            document.Load(IOHelper.MapPath(string.Format(UmbracoLangPath, language)));

            // select root node in the web.config file to insert new node
            var rootNode = EnsureAreaRootNode(area, document);

            var modified = false;
            var insertNode = true;

            // look for existing nodes with same path like the new node
            if (rootNode.HasChildNodes)
            {
                var node = rootNode.SelectSingleNode(string.Format("//key[@alias = '{0}']", key));
                if (node != null)
                {
                    insertNode = false;
                }
            }

            // check for insert flag
            if (insertNode)
            {
                // create new node with attributes
                var newNode = document.CreateElement("key");
                newNode.Attributes.Append(XmlHelper.AddAttribute(document, "alias", key));
                newNode.InnerText = value;

                // select for new node insert position
                if (position == null || position == "end")
                {
                    // append new node at the end of root node
                    rootNode.AppendChild(newNode);

                    // mark document modified
                    modified = true;
                }
                else if (position == "beginning")
                {
                    // prepend new node at the beginning of root node
                    rootNode.PrependChild(newNode);

                    // mark document modified
                    modified = true;
                }
            }

            // check for modified document
            if (modified)
            {
                try
                {
                    document.Save(IOHelper.MapPath(string.Format(UmbracoLangPath, language)));
                    return true;
                }
                catch (Exception ex)
                {
                    var message = string.Concat("Error at install ", this.Alias(), " package action: ", ex);
                    LogHelper.Error(typeof(AddLanguageFileKey), message, ex);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a Sample XML Node
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            var xml = string.Concat("<Action runat=\"install\" undo=\"true\" alias=\"", this.Alias(), "\" language=\"en\" position=\"end\" area=\"sections\" key=\"", MerchelloConfiguration.ApplicationName.ToLowerInvariant(), "\" value=\"", MerchelloConfiguration.ApplicationName, "\" />");
            return helper.parseStringToXmlNode(xml);
        }

        /// <summary>
        /// Undoes the specified package name.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <returns></returns>
        public bool Undo(string packageName, XmlNode xmlData)
        {
            // get attribute values of xmlData
            var requiredFields = new[] { "area", "key", "value" };
            foreach (var field in requiredFields)
            {
                if (xmlData.Attributes[field] == null)
                    return false;
            }

            var area = xmlData.Attributes["area"].Value;
            var key = xmlData.Attributes["key"].Value;
            var value = xmlData.Attributes["value"].Value;
            var language = xmlData.Attributes["language"] != null ? xmlData.Attributes["language"].Value : "en";

            // create a new xml document
            var document = new XmlDocument(){PreserveWhitespace = true};
            document.Load(IOHelper.MapPath(string.Format(UmbracoLangPath, language)));

            // select root node in the web.config file for insert new nodes
            var rootNode = document.SelectSingleNode(string.Format("//language/area[@alias = '{0}']", area));

            // check for rootNode exists
            if (rootNode == null)
                return false;

            var modified = false;

            // look for existing nodes with same path of undo attribute
            if (rootNode.HasChildNodes)
            {
                foreach (XmlNode existingNode in rootNode.SelectNodes(string.Format("//key[@alias = '{0}']", key)))
                {
                    rootNode.RemoveChild(existingNode);
                    modified = true;
                }
            }

            if (modified)
            {
                try
                {
                    document.Save(IOHelper.MapPath(string.Format(UmbracoLangPath, language)));

                    return true;
                }
                catch (Exception ex)
                {
                    var message = string.Concat("Error at undo ", this.Alias(), " package action: ", ex);
                    LogHelper.Error(typeof(AddLanguageFileKey), message, ex);
                }
            }
            return false;
        }

        private XmlNode EnsureAreaRootNode(string area, XmlDocument document)
        {
            var node = document.SelectSingleNode(string.Format("//language/area[@alias = '{0}']", area));

            // if the root node doesn't exist, create it.
            if (node == null)
            {
                node = document.CreateElement("area");
                node.Attributes.Append(XmlHelper.AddAttribute(document, "alias", area));
                document.DocumentElement.AppendChild(node);
            }

            return node;
        }
    }
}