using System;
using System.Collections.Generic;
using System.Globalization;
using Examine;
using Examine.LuceneEngine;
using Lucene.Net.Documents;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using umbraco.BusinessLogic;
using Umbraco.Core.Logging;

namespace Merchello.Web.Indexing
{
    public class MerchelloIndexDataService : ISimpleDataService
    {
        private IProductService _productService;

        public MerchelloIndexDataService()
            : this(MerchelloContext.Current)
        { }

        public MerchelloIndexDataService(IMerchelloContext merchelloContext)
        {
            _productService = merchelloContext.Services.ProductService;
        }

        public IEnumerable<SimpleDataSet> GetAllData(string indexType)
        {
            var allProducts = ((ProductService) _productService).GetAll();

            var dataSets = new List<SimpleDataSet>();

            var i = 1;

            foreach (var product in allProducts)
            {
                try
                {
                    var simpleDataSet = new SimpleDataSet()
                    {
                        NodeDefinition = new IndexedNode(),
                        RowData = new Dictionary<string, string>()
                    };

                    simpleDataSet = MapProductVariantToSimpleDataIndexItem(product, simpleDataSet, i, indexType);

                    dataSets.Add(simpleDataSet);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<MerchelloIndexDataService>(ex.Message, ex);                   
                }

                i++;
            }

            return dataSets;
        }

        private static SimpleDataSet MapProductVariantToSimpleDataIndexItem(IProduct product, SimpleDataSet simpleDataSet, int index, string indexType)
        {
            simpleDataSet.NodeDefinition.NodeId = index;
            simpleDataSet.NodeDefinition.Type = indexType;
            simpleDataSet.RowData.Add("Name", product.Name);
            simpleDataSet.RowData.Add("Sku", product.Sku);
            simpleDataSet.RowData.Add("Price", product.Price.ToString(CultureInfo.InvariantCulture));

            return simpleDataSet;
        }
    }
}