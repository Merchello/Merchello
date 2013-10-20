using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml.Linq;
using Examine;
using Examine.LuceneEngine;
using Examine.LuceneEngine.Config;
using Lucene.Net.Analysis;
using Merchello.Core.Models;
using Merchello.Examine.DataServices;
using UmbracoExamine.Config;


namespace Merchello.Examine
{
    public class MerchelloProductIndexer : BaseMerchelloIndexer
    {

        public MerchelloProductIndexer()
            :base()
        {}

                /// <summary>
        /// Constructor to allow for creating an indexer at runtime
        /// </summary>
        /// <param name="indexerData"></param>
        /// <param name="indexPath"></param>
        /// <param name="dataService"></param>
        /// <param name="analyzer"></param>
		[SecuritySafeCritical]
		public MerchelloProductIndexer(IIndexCriteria indexerData, DirectoryInfo indexPath, IDataService dataService, Analyzer analyzer, bool async)
            : base(indexerData, indexPath, dataService, analyzer, async) { }

		/// <summary>
		/// Constructor to allow for creating an indexer at runtime
		/// </summary>
		/// <param name="indexerData"></param>
		/// <param name="luceneDirectory"></param>
		/// <param name="dataService"></param>
		/// <param name="analyzer"></param>
		/// <param name="async"></param>
		[SecuritySafeCritical]
        public MerchelloProductIndexer(IIndexCriteria indexerData, Lucene.Net.Store.Directory luceneDirectory, IDataService dataService, Analyzer analyzer, bool async)
			: base(indexerData, luceneDirectory, dataService, analyzer, async) { }
        

        /// <summary>
        /// Adds all product variants to the index
        /// </summary>
        /// <param name="type"></param>
        protected override void PerformIndexAll(string type)
        {
            if(!SupportedTypes.Contains(type)) return;

            var products = DataService.ProductDataService.GetAll();
            var productsArray = products as IProduct[] ?? products.ToArray();

            if (!productsArray.Any()) return;
            var id = 1000;
            var nodes = new List<XElement>();
            foreach (var p in productsArray)
            {
                foreach (var el in p.SerializeToXml().Descendants("productVariant"))
                {
                    el.AddIdAttribute(id);
                    
                    nodes.Add(el);
                    id++;
                }                
            }

            AddNodesToIndex(nodes, type);
        }

        protected override void PerformIndexRebuild()
        {
            foreach (var t in SupportedTypes)
            {
                IndexAll(t);
            }
        }


        protected override IEnumerable<string> SupportedTypes
        {
            get { return new[] { IndexTypes.ProductVariant }; }
        }


        internal static readonly List<StaticField> IndexFieldPolicies
            = new List<StaticField>()
            {
                new StaticField("productKey", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("productVariantId", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("name", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("sku", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("price", FieldIndexTypes.ANALYZED, true, "DOUBLE"),
                new StaticField("onSale", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("salePrice", FieldIndexTypes.NOT_ANALYZED, true, "DOUBLE"),
                new StaticField("weight", FieldIndexTypes.NOT_ANALYZED, false, "DOUBLE"),
                new StaticField("length", FieldIndexTypes.NOT_ANALYZED, false, "DOUBLE"),
                new StaticField("height", FieldIndexTypes.NOT_ANALYZED, false, "DOUBLE"),
                new StaticField("barcode", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),                
                new StaticField("available", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("trackInventory", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("outOfStockPurchase", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("taxable", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("shippable", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("download", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("downloadMediaId", FieldIndexTypes.NOT_ANALYZED, false, "NUMBER"),
                new StaticField("master", FieldIndexTypes.ANALYZED, false, string.Empty)
            };


        public override void RebuildIndex()
        {
            DataService.LogService.AddVerboseLog(-1, "Rebuilding index");
            base.RebuildIndex();
        }

        /// <summary>
        /// Creates an IIndexCriteria object based on the indexSet passed in and our DataService
        /// </summary>
        /// <param name="indexSet"></param>
        /// <returns></returns>
        /// <remarks>
        /// If we cannot initialize we will pass back empty indexer data since we cannot read from the database
        /// </remarks>
        protected override IIndexCriteria GetIndexerData(IndexSet indexSet)
        {
            if (CanInitialize())
            {
                return indexSet.ToIndexCriteria(DataService.ProductDataService.GetIndexFieldNames(),  IndexFieldPolicies);
            }
            else
            {
                return base.GetIndexerData(indexSet);
            }

        }
        

        /// <summary>
        /// return the index policy for the field name passed in, if not found, return normal
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected override FieldIndexTypes GetPolicy(string fieldName)
        {
            var def = IndexFieldPolicies.Where(x => x.Name == fieldName).ToArray();
            return (def.Any() == false ? FieldIndexTypes.ANALYZED : def.Single().IndexType);
        }

    }
}