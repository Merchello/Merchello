using System.Linq;
using Examine;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine;
using Merchello.Examine.Providers;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Tests.IntegrationTests.Examine
{
    [TestFixture]
    public class StartupExamineEventTests : DatabaseIntegrationTestBase
    {
        private bool _productSavedEventTriggered;
        private IProductService _productService;

        [SetUp]
        public void Init()
        {
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();
            _productSavedEventTriggered = false;

            _productService = PreTestDataWorker.ProductService;

            ProductService.Saved += delegate(IProductService sender, SaveEventArgs<IProduct> args)
            {
                _productSavedEventTriggered = true;
                args.SavedEntities.ForEach(IndexProduct);
            };
        }

        [Test]
        public void Can_Identify_Registered_Providers()
        {
            //// Arrange
            var expected = 4;

            //// Act
            var registeredProviders =
                ExamineManager.Instance.IndexProviderCollection.OfType<BaseMerchelloIndexer>()
                    .Count(x => x.EnableDefaultEventHandler);

            //// Assert
            Assert.AreEqual(expected, registeredProviders);
        }

        [Test]
        public void ProductIndex_Is_Updated_With_Product_Save()
        {
            //// Arrange
            var product = MockProductDataMaker.MockProductCollectionForInserting(1).First();
            

            //// Act
            _productService.Save(product);

            //// Assert
            Assert.IsTrue(_productSavedEventTriggered);

        }

        #region ExamineEvents private methods
        

        private static void IndexProduct(IProduct product)
        {
            product.ProductVariants.ForEach(IndexProductVariant);
            IndexProductVariant(((Product)product).MasterVariant);
        }

        private static void IndexProductVariant(IProductVariant productVariant)
        {
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].ReIndexNode(productVariant.SerializeToXml().Root, IndexTypes.ProductVariant);          
        }

        private static void DeleteProductFromIndex(IProduct product)
        {
            product.ProductVariants.ForEach(DeleteProductVariantFromIndex);
            DeleteProductVariantFromIndex(((Product)product).MasterVariant);
        }

        private static void DeleteProductVariantFromIndex(IProductVariant productVariant)
        {
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].DeleteFromIndex(((ProductVariant)productVariant).ExamineId.ToString());            
        }

        #endregion
    }
}
