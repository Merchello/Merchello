namespace Merchello.Examine.Providers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Xml.Linq;

    using global::Examine;

    using global::Examine.LuceneEngine;

    using global::Examine.LuceneEngine.Config;

    using Lucene.Net.Analysis;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Examine.DataServices;

    /// <summary>
    /// The product indexer.
    /// </summary>
    public class ProductIndexer : BaseMerchelloIndexer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductIndexer"/> class.
        /// </summary>
        public ProductIndexer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductIndexer"/> class.
        /// </summary>
        /// <param name="indexerData">
        /// The indexer data.
        /// </param>
        /// <param name="indexPath">
        /// The index path.
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
        public ProductIndexer(
            IIndexCriteria indexerData,
            DirectoryInfo indexPath,
            IDataService dataService,
            Analyzer analyzer,
            bool async)
            : base(indexerData, indexPath, dataService, analyzer, async)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductIndexer"/> class.
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
        public ProductIndexer(
            IIndexCriteria indexerData,
            Lucene.Net.Store.Directory luceneDirectory,
            IDataService dataService,
            Analyzer analyzer,
            bool async)
            : base(indexerData, luceneDirectory, dataService, analyzer, async)
        {    
        }

        /// <summary>
        /// The index field policies.
        /// </summary>
        internal static readonly List<StaticField> IndexFieldPolicies = new List<StaticField>()
            {
                new StaticField("productKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("productVariantKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("name", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("sku", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("price", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("onSale", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("manufacturer", FieldIndexTypes.ANALYZED,false, string.Empty),
                new StaticField("modelNumber", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("salePrice", FieldIndexTypes.NOT_ANALYZED, true, string.Empty),
                new StaticField("costOfGoods", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("weight", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("length", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("height", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("width", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("barcode", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),                
                new StaticField("available", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("trackInventory", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("outOfStockPurchase", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("taxable", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("shippable", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("download", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("downloadMediaId", FieldIndexTypes.ANALYZED, false, "NUMBER"),
                new StaticField("downloadMediaPropertyIds", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("master", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("totalInventoryCount", FieldIndexTypes.NOT_ANALYZED, false, "NUMBER"),
                new StaticField("attributes", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("catalogInventories", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("productOptions", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("versionKey", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("createDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
                new StaticField("updateDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
                new StaticField("allDocs", FieldIndexTypes.ANALYZED, false, string.Empty)
            };

        /// <summary>
        /// Gets the supported types.
        /// </summary>
        protected override IEnumerable<string> SupportedTypes
        {
            get { return new[] { IndexTypes.ProductVariant }; }
        }
              

        /// <summary>
        /// Completely rebuilds the index.
        /// </summary>
        public override void RebuildIndex()
        {
            DataService.LogService.AddVerboseLog(-1, "Rebuilding the product index");

            EnsureIndex(true);

            PerformIndexAll(IndexTypes.ProductVariant);
        }

        /// <summary>
        /// Adds all variants for a given product to the index
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <remarks>
        /// For testing
        /// </remarks>
        internal void AddProductToIndex(IProduct product)
        {
            var nodes = new List<XElement>();
            nodes.AddRange(product.SerializeToXml().Descendants("productVariant"));
            AddNodesToIndex(nodes, IndexTypes.ProductVariant);
        }

        /// <summary>
        /// Removes all variants for a given product from the index
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <remarks>
        /// For testing
        /// </remarks>
        internal void DeleteProductFromIndex(IProduct product)
        {
            var ids = product.ProductVariants.Select(x => ((ProductVariant)x).ExamineId).ToList();
            ids.Add(((ProductVariant)((Product) product).MasterVariant).ExamineId);
            
            foreach (var id in ids)
            {
                DeleteFromIndex(id.ToString(CultureInfo.InvariantCulture));
            }
        }
        
        /// <summary>
        /// Creates an IIndexCriteria object based on the indexSet passed in and our DataService
        /// </summary>
        /// <param name="indexSet">
        /// The index Set.
        /// </param>
        /// <remarks>
        /// If we cannot initialize we will pass back empty indexer data since we cannot read from the database
        /// </remarks>
        /// <returns>
        /// The <see cref="IIndexCriteria"/>.
        /// </returns>
        protected override IIndexCriteria GetIndexerData(IndexSet indexSet)
        {
            return indexSet.ToIndexCriteria(DataService.ProductDataService.GetIndexFieldNames(),  IndexFieldPolicies);
        }
        
        /// <summary>
        /// return the index policy for the field name passed in, if not found, return normal
        /// </summary>
        /// <param name="fieldName">
        /// The field Name.
        /// </param>
        /// <returns>
        /// The <see cref="FieldIndexTypes"/>.
        /// </returns>
        protected override FieldIndexTypes GetPolicy(string fieldName)
        {
            var def = IndexFieldPolicies.Where(x => x.Name == fieldName).ToArray();
            return def.Any() == false ? FieldIndexTypes.ANALYZED : def.Single().IndexType;
        }

        /// <summary>
        /// Adds all product variants to the index
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        protected override void PerformIndexAll(string type)
        {
            if (!SupportedTypes.Contains(type)) return;

            var products = DataService.ProductDataService.GetAll();
            var productsArray = products as IProduct[] ?? products.ToArray();

            if (!productsArray.Any()) return;
            var nodes = new List<XElement>();
            foreach (var p in productsArray)
            {
                nodes.AddRange(p.SerializeToXml().Descendants("productVariant"));
            }

            AddNodesToIndex(nodes, IndexTypes.ProductVariant);
        }
    }
}