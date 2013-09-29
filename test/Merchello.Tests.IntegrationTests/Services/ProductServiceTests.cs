using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class ProductServiceTests : ServiceIntegrationTestBase
    {
        private IProductService _productService;

        //[SetUp]
        //public void Setup()
        //{
        //    _productService = PreTestDataWorker.ProductService;
        //}

        ///// <summary>
        ///// Test to verify that a product can be created
        ///// </summary>
        //[Test]
        //public void Can_Create_A_Product()
        //{
        //    //// Arrage
        //    var sku = MockDataMakerBase.MockSku();
        //    var expected = MockProductDataMaker.MockProductForInserting(sku, "fake product", 15.00m);

        //    //// Act
        //    var product = _productService.CreateProduct(sku, "fake product", 15.00m);

        //    //// Assert
        //    Assert.AreEqual(expected.Name, product.Name);
        //}

        ///// <summary>
        ///// Test to verify that a product can be saved
        ///// </summary>
        //[Test]
        //public void Can_Save_A_Product()
        //{
        //    //// Arrange
        //    var product = MockProductDataMaker.MockProductForInserting();

        //    //// Act
        //    _productService.Save(product);

        //    //// Assert
        //    Assert.IsTrue(product.HasIdentity);
        //}

        ///// <summary>
        ///// Verifies that a product can be retreived by Key
        ///// </summary>
        //[Test]
        //public void Can_Retrieve_A_Product_By_Key()
        //{
        //    //// Arrange
        //    var expected = PreTestDataWorker.MakeExistingProduct();
        //    var key = expected.Key;

        //    //// Act
        //    var retrieved = _productService.GetByKey(key);

        //    //// Assert
        //    Assert.NotNull(retrieved);
        //    Assert.AreEqual(expected.Key, retrieved.Key);
        //}

        ///// <summary>
        ///// Verifies that a multiple products can be saved
        ///// </summary>
        //[Test]
        //public void Can_Save_Multiple_Products()
        //{
        //    //// Arrange
        //    PreTestDataWorker.DeleteAllProducts();
        //    var expected = 3;
        //    var generated = MockProductDataMaker.MockProductCollectionForInserting(expected);

        //    //// Act
        //    _productService.Save(generated);

        //    //// Assert
        //    var retrieved = ((ProductService) _productService).GetAll();
        //    Assert.IsTrue(retrieved.Any());
        //    Assert.AreEqual(expected, retrieved.Count());
        //}

        ///// <summary>
        ///// Test to verify a product can be deleted
        ///// </summary>
        //[Test]
        //public void Can_Delete_A_Product()
        //{
        //    //// Arrange
        //    var product = PreTestDataWorker.MakeExistingProduct();
        //    var key = product.Key;

        //    //// Act
        //    _productService.Delete(product);
            
        //    //// Assert
        //    var retrieved = _productService.GetByKey(key);
        //    Assert.IsNull(retrieved);
        //}

        ///// <summary>
        ///// Can update a product
        ///// </summary>
        //[Test]
        //public void Can_Update_A_Product()
        //{
        //    //// Arrange
        //    var generated = PreTestDataWorker.MakeExistingProduct();
        //    var old = generated.Name;
        //    var newName = "Wizard's Hat";

        //    //// Act
        //    generated.Name = newName;
        //    _productService.Save(generated);

        //    //// Assert
        //    var retrieved = _productService.GetByKey(generated.Key);
        //    Assert.AreNotEqual(old, retrieved.Name);
        //    Assert.AreEqual(newName, retrieved.Name);
        //}
        
    }
}
