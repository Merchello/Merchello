using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Analysis;
using Merchello.Core;
using Merchello.Examine.DataServices;

namespace Merchello.Examine.Providers
{
    public abstract class BaseMerchelloIndexer : LuceneIndexer
    {

        #region Constructors

        protected BaseMerchelloIndexer()
            : base()
        { }


        /// <summary>
        /// Constructor to allow for creating an indexer at runtime
        /// </summary>
        /// <param name="indexerData"></param>
        /// <param name="indexPath"></param>
        /// <param name="dataService"></param>
        /// <param name="analyzer"></param>
        /// <param name="async"></param>
        [SecuritySafeCritical]
        protected BaseMerchelloIndexer(IIndexCriteria indexerData, DirectoryInfo indexPath, IDataService dataService, Analyzer analyzer, bool async)
            : base(indexerData, indexPath, analyzer, async)
        {
            DataService = dataService;
        }

        [SecuritySafeCritical]
        protected BaseMerchelloIndexer(IIndexCriteria indexerData, Lucene.Net.Store.Directory luceneDirectory, IDataService dataService, Analyzer analyzer, bool async)
			: base(indexerData, luceneDirectory, analyzer, async)
		{
			DataService = dataService;
		}

        #endregion

        /// <summary>
        /// Used for unit tests
        /// </summary>
        internal static bool? DisableInitializationCheck = null;


        #region Properties
        
        /// <summary>
        /// If true, the IndexingActionHandler will be run to keep the default index up to date.
        /// </summary>
        public bool EnableDefaultEventHandler { get; protected set; }

        /// <summary>
        /// The data service used for retreiving and saving data
        /// </summary>
        public IDataService DataService { get; protected internal set; }

        /// <summary>
        /// the supported indexable types
        /// </summary>
        protected abstract IEnumerable<string> SupportedTypes { get; }

        #endregion


        #region Initialize


        /// <summary>
        /// Setup the properties for the indexer from the provider settings
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        [SecuritySafeCritical]
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config["dataService"] != null && !string.IsNullOrEmpty(config["dataService"]))
            {
                //this should be a fully qualified type
                var serviceType = Type.GetType(config["dataService"]);
                DataService = (IDataService)Activator.CreateInstance(serviceType);
            }
            else if (DataService == null)
            {
                //By default, we will be using the UmbracoDataService
                //generally this would only need to be set differently for unit testing
                DataService = new MerchelloDataService();
            }

            DataService.LogService.ProviderName = name;

            EnableDefaultEventHandler = true; //set to true by default
            bool enabled;
            if (bool.TryParse(config["enableDefaultEventHandler"], out enabled))
            {
                EnableDefaultEventHandler = enabled;
            }

            DataService.LogService.AddVerboseLog(-1, string.Format("{0} indexer initializing", name));

            base.Initialize(name, config);
        }

        #endregion


        /// <summary>
        /// override to check if we can actually initialize. 
        /// </summary>
        /// <remarks>
        /// This check is required since the base examine lib will try to rebuild on startup
        /// </remarks>
        public override void RebuildIndex()
        {
            if (CanInitialize())
            {
                base.RebuildIndex();
            }
        }

        /// <summary>
        /// override to check if we can actually initialize. 
        /// </summary>
        /// <remarks>
        /// This check is required since the base examine lib will try to rebuild on startup
        /// </remarks>
        public override void IndexAll(string type)
        {
            if (CanInitialize())
            {
                base.IndexAll(type);
            }
        }

        public override void ReIndexNode(XElement node, string type)
        {
            if (CanInitialize())
            {
                if (!SupportedTypes.Contains(type))
                    return;

                base.ReIndexNode(node, type);
            }
        }

        /// <summary>
        /// override to check if we can actually initialize. 
        /// </summary>
        /// <remarks>
        /// This check is required since the base examine lib will try to rebuild on startup
        /// </remarks>
        public override void DeleteFromIndex(string nodeId)
        {
            if (CanInitialize())
            {
                base.DeleteFromIndex(nodeId);
            }
        }


        #region Protected

        /// <summary>
        /// Returns true if the Merchello application is in a state that we can initialize the examine indexes
        /// </summary>
        /// <returns></returns>
        [SecuritySafeCritical]
        protected bool CanInitialize()
        {
            //check the DisableInitializationCheck and ensure that it is not set to true
            if (!DisableInitializationCheck.HasValue || !DisableInitializationCheck.Value)
            {
                //We need to check if we actually can initialize, if not then don't continue
                if (MerchelloContext.Current == null
                    || !MerchelloContext.Current.IsConfigured
                    || !MerchelloContext.Current.IsReady)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Reindexes all supported types
        /// </summary>
        protected override void PerformIndexRebuild()
        {
            foreach (var t in SupportedTypes)
            {
                IndexAll(t);
            }
        }


        #endregion


        protected override void OnIndexingError(IndexingErrorEventArgs e)
        {
            DataService.LogService.AddErrorLog(e.NodeId, IndexSetName, e.InnerException);
            base.OnIndexingError(e);
        }

        protected override void OnNodeIndexed(IndexedNodeEventArgs e)
        {
            DataService.LogService.AddVerboseLog(e.NodeId, string.Format("Index created for node"));
            base.OnNodeIndexed(e);
        }

        protected override void OnIndexDeleted(DeleteIndexEventArgs e)
        {
            DataService.LogService.AddVerboseLog(-1, string.Format("Index deleted for term: {0} with value {1}", e.DeletedTerm.Key, e.DeletedTerm.Value));
            base.OnIndexDeleted(e);
        }

        protected override void OnIndexOptimizing(EventArgs e)
        {
            DataService.LogService.AddInfoLog(-1, string.Format("Index is being optimized"));
            base.OnIndexOptimizing(e);
        }

        /// <summary>
        /// Called when a duplicate field is detected in the dictionary that is getting indexed.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="indexSetName"></param>
        /// <param name="fieldName"></param>
        protected override void OnDuplicateFieldWarning(int nodeId, string indexSetName, string fieldName)
        {
            base.OnDuplicateFieldWarning(nodeId, indexSetName, fieldName);

            DataService.LogService.AddInfoLog(nodeId, "Field \"" + fieldName + "\" is listed multiple times in the index set \"" + indexSetName + "\". Please ensure all names are unique");
        }



    }
}