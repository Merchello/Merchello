//using Merchello.Core;
//using Merchello.Core.Models;
//using Merchello.Tests.IntegrationTests.TestHelpers;
//using Merchello.Web.Models.ContentEditing;
//using NUnit.Framework;
//using RazorEngine;

//namespace Merchello.Tests.IntegrationTests.PartialView
//{
//    [TestFixture]
//    public class RazorEngineTests : MerchelloAllInTestBase
//    {

//        private IProduct _product;

//        [TestFixtureSetUp]
//        public override void FixtureSetup()
//        {
//            base.FixtureSetup();

//            _product = DbPreTestDataWorker.MakeExistingProduct(weight: 10, price: 15);
//            var option = new ProductOption("Color");
//            _product.ProductOptions.Add(option);
//            _product.ProductOptions.Add(new ProductOption("Size"));
//            MerchelloContext.Current.Services.ProductService.Save(_product);
//        }

//        [Test]
//        public void Can_Parse_Simple_Razor_Temple_With_Product_Model()
//        {
//            //// Arrange
//            var template = @"
//            <html>
//                <body>                
//                    <div>@Model.Name, @Model.Sku, @Model.Price</div>
//                    <ul>
//                    @foreach(var option in Model.ProductOptions) 
//                    { 
//                        <li>@option.Name</li> 
//                    }
//                    </ul>
//                </body>
//            </html>";

            
//            //// Act
//            var result = Razor.Parse(template, _product.ToProductDisplay());

//            //// Assert
//            Assert.NotNull(result);
//            Assert.IsTrue(result.Contains("Color"));
//        }

//        [Test]
//        public void Can_Use_MerchelloContext_In_RazorEngine()
//        {
            
//            //// Arrange
//            var model = new TestViewModel();
//            model.MerchelloContext = MerchelloContext.Current;
//            model.Product = _product.ToProductDisplay();

//            var template = @"
//            @{
//                var product = Model.MerchelloContext.Services.ProductService.GetByKey(Model.Product.Key);
//            }
//
//            <html>
//                <body>                
//                    <div>@Model.Product.Name, @Model.Product.Sku, @Model.Product.Price</div>
//                    
//                    <ul>
//                    @foreach(var option in Model.Product.ProductOptions) 
//                    { 
//                        <li>@option.Name</li> 
//                    }
//                    </ul>
//                </body>
//            </html>";

//            //// Act
//            var result = Razor.Parse(template, model);

//            //// Assert
//            Assert.NotNull(result);
//            Assert.IsTrue(result.Contains("Color"));

//        }

//        public class TestViewModel
//        {
//            public IMerchelloContext MerchelloContext { get; set; }
//            public ProductDisplay Product { get; set; }
//        }

//    }

 
//}