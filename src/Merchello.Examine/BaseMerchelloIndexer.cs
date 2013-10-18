using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security;
using Examine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Analysis;
using UmbracoExamine.DataServices;
using IDataService = Merchello.Examine.DataService.IDataService;

namespace Merchello.Examine
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
            //if (config["dataService"] != null && !string.IsNullOrEmpty(config["dataService"]))
            //{
            //    //this should be a fully qualified type
            //    var serviceType = Type.GetType(config["dataService"]);
            //    DataService = (IDataService)Activator.CreateInstance(serviceType);
            //}
            //else if (DataService == null)
            //{
            //    //By default, we will be using the UmbracoDataService
            //    //generally this would only need to be set differently for unit testing
            //    DataService = new UmbracoDataService();
            //}   
        }

        #endregion
    }
}