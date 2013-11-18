using System.Collections.Generic;
using System.Globalization;
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

namespace Merchello.Examine.Providers
{
    public class ProductIndexer : BaseMerchelloIndexer
    {

        public ProductIndexer()
        {}

                /// <summary>
        /// Constructor to allow for creating an indexer at runtime
        /// </summary>
        /// <param name="indexerData"></param>
        /// <param name="indexPath"></param>
        /// <param name="dataService"></param>
        /// <param name="analyzer"></param>
		[SecuritySafeCritical]
		public ProductIndexer(IIndexCriteria indexerData, DirectoryInfo indexPath, IDataService dataService, Analyzer analyzer, bool async)
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
        public ProductIndexer(IIndexCriteria indexerData, Lucene.Net.Store.Directory luceneDirectory, IDataService dataService, Analyzer analyzer, bool async)
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
            var nodes = new List<XElement>();
            foreach (var p in productsArray)
            {
                nodes.AddRange(p.SerializeToXml().Descendants("productVariant"));
            }

            AddNodesToIndex(nodes, type);
        }


        public override void RebuildIndex()
        {
            DataService.LogService.AddVerboseLog(-1, "Rebuilding index");
            base.RebuildIndex();
        }

        /// <summary>
        /// Adds all variants for a given product to the index
        /// </summary>
        /// <param name="product"></param>
        /// <remarks>For testing</remarks>
        internal void AddProductToIndex(IProduct product)
        {
            var nodes = new List<XElement>();
            nodes.AddRange(product.SerializeToXml().Descendants("productVariant"));
            AddNodesToIndex(nodes, IndexTypes.ProductVariant);
        }

        /// <summary>
        /// Removes all variants for a given product from the index
        /// </summary>
        /// <param name="product"></param>
        /// <remarks>For testing</remarks>
        internal void DeleteProductFromIndex(IProduct product)
        {
            var ids = product.ProductVariants.Select(x => ((ProductVariant)x).ExamineId).ToList();
            ids.Add(
                ((ProductVariant)((Product) product).MasterVariant).ExamineId
                );
            
            foreach (var id in ids)
            {
                DeleteFromIndex(id.ToString(CultureInfo.InvariantCulture));
            }
        }

        protected override IEnumerable<string> SupportedTypes
        {
            get { return new[] { IndexTypes.ProductVariant }; }
        }


        internal static readonly List<StaticField> IndexFieldPolicies
            = new List<StaticField>()
            {
                new StaticField("productKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("productVariantKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("name", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("sku", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("price", FieldIndexTypes.ANALYZED, true, "DOUBLE"),
                new StaticField("onSale", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("salePrice", FieldIndexTypes.NOT_ANALYZED, true, "DOUBLE"),
                new StaticField("costOfGoods", FieldIndexTypes.NOT_ANALYZED, false, "DOUBLE"),
                new StaticField("weight", FieldIndexTypes.NOT_ANALYZED, false, "DOUBLE"),
                new StaticField("length", FieldIndexTypes.NOT_ANALYZED, false, "DOUBLE"),
                new StaticField("height", FieldIndexTypes.NOT_ANALYZED, false, "DOUBLE"),
                new StaticField("width", FieldIndexTypes.NOT_ANALYZED, false, "DOUBLE"),
                new StaticField("barcode", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),                
                new StaticField("available", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("trackInventory", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("outOfStockPurchase", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("taxable", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("shippable", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("download", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("downloadMediaId", FieldIndexTypes.NOT_ANALYZED, false, "NUMBER"),
                new StaticField("master", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("totalInventoryCount", FieldIndexTypes.NOT_ANALYZED, false, "NUMBER"),
                new StaticField("attributes", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("warehouses", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("options", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("createDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
                new StaticField("updateDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
                new StaticField("allDocs", FieldIndexTypes.ANALYZED, false, string.Empty)
            };


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
            return indexSet.ToIndexCriteria(DataService.ProductDataService.GetIndexFieldNames(),  IndexFieldPolicies);
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