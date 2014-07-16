namespace Merchello.Web.PackageActions
{
    using System.Web;
    using System.Xml;
    using Core.Configuration;
    using umbraco.cms.businesslogic.packager.standardPackageActions;    
    using umbraco.interfaces;
    using Umbraco.Core;

    /// <summary>
    ///  Package action adds the custom Merchello Examine xml configurations to the Examine configuration files
    /// </summary>
    public class AddExamineConfigs : IPackageAction
    {
        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return string.Concat(MerchelloConfiguration.ApplicationName, "_AddExamineConfigs");
        }

        /// <summary>
        /// Appends the xmlData to the applicable Examine configuration elements
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">Xml Values</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {      
            // Check if the xmlData has a childnode (the IndexSet rule node)
            if (!xmlData.HasChildNodes) return false;


            // Select IndexSet(s) from the supplied xmlData
            var indexSets = xmlData.SelectNodes("//MerchelloIndexSets/IndexSet");
            var indexers = xmlData.SelectNodes("//MerchelloIndexProviders/add");
            var searchers = xmlData.SelectNodes("//MerchelloSearchProviders/add");

            if (indexSets == null || indexers == null || searchers == null) return false;


            //// --------------- ExamineIndex.config -----------------------------

            var examineIndexFile = XmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/ExamineIndex.config"));
            var examineLuceneIndexSetsNode = examineIndexFile.SelectSingleNode("//ExamineLuceneIndexSets");
            foreach (XmlNode set in indexSets)
            {
                // Add the node
                var newNode = examineLuceneIndexSetsNode.OwnerDocument.ImportNode(set, true);
                examineLuceneIndexSetsNode.AppendChild(newNode);
            }

            //// Save the config file
            examineIndexFile.Save(HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("/config/ExamineIndex.config")));
           

            //// ----------------ExamineSettings.config -------------------------
            var examineSettingsFile = XmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("/config/ExamineSettings.config"));

            //// the index providers
            var examineIndexProviders = examineSettingsFile.SelectSingleNode("//ExamineIndexProviders/providers");
            foreach (XmlNode indexer in indexers)
            {
                var newNode = examineIndexProviders.OwnerDocument.ImportNode(indexer, true);
                examineIndexProviders.AppendChild(newNode);
            }

            //// the searchers
            var examineSearchProviders = examineSettingsFile.SelectSingleNode("//ExamineSearchProviders/providers");
            foreach (XmlNode searcher in searchers)
            {
                var newNode = examineSearchProviders.OwnerDocument.ImportNode(searcher, true);
                examineSearchProviders.AppendChild(newNode);
            }

            examineSettingsFile.Save(HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("/config/ExamineSettings.config")));

            return true;
        }

        /// <summary>
        /// Removes the xmlData Node from the ExamineIndex.config file based on the rulename 
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, XmlNode xmlData)
        {
            // Check if the xmlData has a childnode (the IndexSet rule node)
            if (!xmlData.HasChildNodes) return false;

            // Select IndexSet(s) from the supplied xmlData
            var indexSets = xmlData.SelectNodes("//MerchelloIndexSets/IndexSet");
            var indexers = xmlData.SelectNodes("//MerchelloIndexProviders/add");
            var searchers = xmlData.SelectNodes("//MerchelloSearchProviders/add");

            if (indexSets == null || indexers == null || searchers == null) return false;

            //// -------------- ExamineIndex.config -------------------------

            var examineIndexFile = XmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/ExamineIndex.config"));
            XmlNode examineLuceneIndexSetsNode = examineIndexFile.SelectSingleNode("//ExamineLuceneIndexSets");         
            foreach (XmlNode set in indexSets)
            {
                // Get the index name
                string indexName = set.Attributes["SetName"].Value;

                // Select the node by name from the config file
                XmlNode index = examineLuceneIndexSetsNode.SelectSingleNode("//IndexSet[@SetName = '" + indexName + "']");
                if (index != null)
                {
                    // Index is found, remove it from the xml document
                    examineLuceneIndexSetsNode.RemoveChild(index);
                }
            }

            ////Save the modified configuration file
            examineIndexFile.Save(HttpContext.Current.Server.MapPath("/config/ExamineIndex.config"));

            //// -------------- ExamineSettings.config -----------------------
            var examineSettingsFile = XmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("/config/ExamineSettings.config"));

            var examineIndexProviders = examineSettingsFile.SelectSingleNode("//ExamineIndexProviders/providers");
            foreach (XmlNode indexer in indexers)
            {
                var indexName = indexer.Attributes["name"].Value;

                var provider = examineIndexProviders.SelectSingleNode("//add[@name='" + indexName + "']");
                if (provider != null) examineIndexProviders.RemoveChild(provider);
            }
 
            var examineSearchProviders = examineSettingsFile.SelectSingleNode("//ExamineSearchProviders/providers");
            foreach (XmlNode searcher in searchers)
            {
                var searcherName = searcher.Attributes["name"].Value;
                var provider = examineSearchProviders.SelectSingleNode("//add[@name='" + searcherName + "']");
                if (provider != null) examineSearchProviders.RemoveChild(provider);
            }

            examineSettingsFile.Save(HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("/config/ExamineSettings.config")));

            return true;
        }

        /// <summary>
        /// Returns a Sample XML Node
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            string sample =
                "<Action runat=\"install\" undo=\"true\" alias=\"AddExamineIndex\">" +
                  "<MerchelloIndexSets>" +
                    "<IndexSet SetName=\"MerchelloProductIndexSet\" IndexPath=\"~/App_Data/TEMP/ExamineIndexes/Merchello/Product/\" />" +
                    "<IndexSet SetName=\"MerchelloCustomerIndexSet\" IndexPath=\"~/App_Data/TEMP/ExamineIndexes/Merchello/Customer/\" />" +
                    "<IndexSet SetName=\"MerchelloOrderIndexSet\" IndexPath=\"~/App_Data/TEMP/ExamineIndexes/Merchello/Product/\" />" + 
                  "</MerchelloIndexSets>" +
                  "<MerchelloIndexProviders>" +
                    "<add name=\"MerchelloProductIndexer\" type=\"Merchello.Examine.Providers.ProductIndexer, Merchello.Examine\" />" +
                  "</MerchelloIndexProviders>" +
                  "<MerchelloSearchProviders>" +
                    "<add name=\"MerchelloProductSearcher\" type=\"Examine.LuceneEngine.Providers.LuceneSearcher, Examine\" />" +
                  "</MerchelloSearchProviders>" +
                "</Action>";

            return helper.parseStringToXmlNode(sample);
        }

        #endregion

    }
}

//<IndexSet SetName="MerchelloProductIndexSet" IndexPath="../../App_Data/Merchello/Product/" />