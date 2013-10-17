using System;
using System.Collections.Generic;
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
        private IProductVariantService _productVariantService;

        public MerchelloIndexDataService()
            : this(MerchelloContext.Current)
        { }

        public MerchelloIndexDataService(IMerchelloContext merchelloContext)
        {
            _productVariantService = merchelloContext.Services.ProductVariantService;
        }

        public IEnumerable<SimpleDataSet> GetAllData(string indexType)
        {
            var allVariants = ((ProductVariantService) _productVariantService).GetAll();

            var dataSets = new List<SimpleDataSet>();

            var i = 1;

            foreach (var variant in allVariants)
            {
                try
                {
                    var simpleDataSet = new SimpleDataSet()
                    {
                        NodeDefinition = new IndexedNode(),
                        RowData = new Dictionary<string, string>()
                    };

                    simpleDataSet = MapProductVariantToSimpleDataIndexItem(variant, simpleDataSet, i, indexType);

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

        private static SimpleDataSet MapProductVariantToSimpleDataIndexItem(IProductVariant productVariant, SimpleDataSet simpleDataSet, int index, string indexType)
        {
            simpleDataSet.NodeDefinition.NodeId = index;
            simpleDataSet.NodeDefinition.Type = indexType;
            //simpleDataSet.NodeDefinition.

            return simpleDataSet;
        }
    }
}