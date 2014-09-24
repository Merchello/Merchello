using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.TestHelpers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Api
{
    [TestFixture]
    public class ProductApiController : MerchelloAllInTestBase
    {

        private IProductService _productService;
        private IProductVariantService _productVariantService;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _productService = MerchelloContext.Current.Services.ProductService;
            _productVariantService = MerchelloContext.Current.Services.ProductVariantService;

            DbPreTestDataWorker.DeleteAllProducts();
        }

        [Test]
        public void Can_Add_A_Product_From_New_Product_Display()
        {
            //// Arrange
            var display = MockProductDataMaker.MockProductDisplayForInserting();

            //// Act
            var product = AddProduct(display);

            //// Assert
            Assert.NotNull(product);
            Assert.IsTrue(product.HasIdentity);
        }


        private IProduct AddProduct(ProductDisplay product)
        {
            var p = _productService.CreateProduct(product.Name, product.Sku, product.Price);
            p = product.ToProduct(p);
            _productService.Save(p);

            if (!p.ProductOptions.Any()) return p;

            var attributeLists = p.GetPossibleProductAttributeCombinations();

            foreach (var list  in attributeLists)
            {
                _productVariantService.CreateProductVariantWithKey(p, list.ToProductAttributeCollection());
            }

            return p;
        }

        
    }
}
