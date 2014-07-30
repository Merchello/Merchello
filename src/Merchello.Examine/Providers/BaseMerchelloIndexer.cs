namespace Merchello.Examine.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Xml.Linq;
    using DataServices;
    using global::Examine;
    using global::Examine.LuceneEngine.Providers;
    using Lucene.Net.Analysis;

    /// <summary>
    /// The base merchello indexer.
    /// </summary>
    public abstract class BaseMerchelloIndexer : LuceneIndexer
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMerchelloIndexer"/> class.
        /// </summary>
        protected BaseMerchelloIndexer()
            : base()
        {            
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMerchelloIndexer"/> class. 
        /// The base merchello indexer.
        /// </summary>
        /// <param name="indexerData">
        /// The indexer Data.
        /// </param>
        /// <param name="indexPath">
        /// The index Path.
        /// </param>
        /// <param name="dataService">
        /// The data Service.
        /// </param>
        /// <param name="analyzer">
        /// The analyzer.
        /// </param>
        /// <param name="async">
        /// A value indicating whether or not to run asyncronously
        /// </param>
        [SecuritySafeCritical]
        protected BaseMerchelloIndexer(IIndexCriteria indexerData, DirectoryInfo indexPath, IDataService dataService, Analyzer analyzer, bool async)
            : base(indexerData, indexPath, analyzer, async)
        {
            DataService = dataService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMerchelloIndexer"/> class.
        /// </summary>
        /// <param name="indexerData">
        /// The indexer data.
        /// </param>
        /// <param name="luceneDirectory">
        /// The lucene directory.
        /// </param>
        /// <param name="dataService">
        /// The data service.
        /// </param>
        /// <param name="analyzer">
        /// The analyzer.
        /// </param>
        /// <param name="async">
        /// The async.
        /// </param>
        [SecuritySafeCritical]
        protected BaseMerchelloIndexer(IIndexCriteria indexerData, Lucene.Net.Store.Directory luceneDirectory, IDataService dataService, Analyzer analyzer, bool async) 
            : base(indexerData, luceneDirectory, analyzer, async)
		{
			DataService = dataService;
		}

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets a value indicating whether the IndexingActionHandler will be run to keep the default index up to date.
        /// </summary>
        public bool EnableDefaultEventHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the data service used for retreiving and saving data
        /// </summary>
        public IDataService DataService { get; protected internal set; }

        /// <summary>
        /// Gets the supported indexable types
        /// </summary>
        protected abstract IEnumerable<string> SupportedTypes { get; }

        #endregion


        #region Initialize


        /// <summary>
        /// Setup the properties for the indexer from the provider settings
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        [SecuritySafeCritical]
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config["dataService"] != null && !string.IsNullOrEmpty(config["dataService"]))
            {
                ////this should be a fully qualified type
                var serviceType = Type.GetType(config["dataService"]);
                DataService = (IDataService)Activator.CreateInstance(serviceType);
            }
            else if (DataService == null)
            {
                ////By default, we will be using the UmbracoDataService
                ////generally this would only need to be set differently for unit testing
                DataService = new MerchelloDataService();
            }

            DataService.LogService.ProviderName = name;

            EnableDefaultEventHandler = true; ////set to true by default
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
        /// Reindexes a node
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public override void ReIndexNode(XElement node, string type)
        {           
            if (!SupportedTypes.Contains(type))
                return;
            base.ReIndexNode(node, type);            
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


        /// <summary>
        /// The on indexing error.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnIndexingError(IndexingErrorEventArgs e)
        {
            DataService.LogService.AddErrorLog(e.NodeId, IndexSetName, e.InnerException);
            base.OnIndexingError(e);
        }

        /// <summary>
        /// The on node indexed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnNodeIndexed(IndexedNodeEventArgs e)
        {
            DataService.LogService.AddVerboseLog(e.NodeId, string.Format("Index created for node"));
            base.OnNodeIndexed(e);
        }

        /// <summary>
        /// The on index deleted.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnIndexDeleted(DeleteIndexEventArgs e)
        {
            DataService.LogService.AddVerboseLog(-1, string.Format("Index deleted for term: {0} with value {1}", e.DeletedTerm.Key, e.DeletedTerm.Value));
            base.OnIndexDeleted(e);
        }

        /// <summary>
        /// The on index optimizing.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnIndexOptimizing(EventArgs e)
        {
            DataService.LogService.AddInfoLog(-1, string.Format("Index is being optimized"));
            base.OnIndexOptimizing(e);
        }

        /// <summary>
        /// Called when a duplicate field is detected in the dictionary that is getting indexed.
        /// </summary>
        /// <param name="nodeId">
        /// The node Id.
        /// </param>
        /// <param name="indexSetName">
        /// The index Set Name.
        /// </param>
        /// <param name="fieldName">
        /// The field Name.
        /// </param>
        protected override void OnDuplicateFieldWarning(int nodeId, string indexSetName, string fieldName)
        {
            base.OnDuplicateFieldWarning(nodeId, indexSetName, fieldName);

            DataService.LogService.AddInfoLog(nodeId, "Field \"" + fieldName + "\" is listed multiple times in the index set \"" + indexSetName + "\". Please ensure all names are unique");
        }
    }
}