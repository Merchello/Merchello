namespace Merchello.Tests.IntegrationTests.Services.Product
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Chains.CopyEntity.Product;
    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    public class CopyProductTests : MerchelloAllInTestBase
    {
        private IProductService _productService;

        private IDetachedContentTypeService _detachedContentTypeService;

        private IEntityCollectionService _entityCollectionService;

        private IProduct _original;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            #region clean up
            _detachedContentTypeService = ((ServiceContext)MerchelloContext.Current.Services).DetachedContentTypeService;

            var existingContentTypes = _detachedContentTypeService.GetAll();
            foreach (var t in existingContentTypes)
            {
                ((ServiceContext)MerchelloContext.Current.Services).DetachedContentTypeService.Delete(t);
            }

            DbPreTestDataWorker.DeleteAllProducts();
            DbPreTestDataWorker.DeleteAllEntityCollections();
            #endregion

            var catalog = DbPreTestDataWorker.WarehouseCatalog;

            var detachedContentType =
                ((ServiceContext)MerchelloContext.Current.Services).DetachedContentTypeService
                    .CreateDetachedContentTypeWithKey(EntityType.Product, new Guid(), "TEST");

            _productService = DbPreTestDataWorker.ProductService;
            _entityCollectionService = DbPreTestDataWorker.EntityCollectionService;

            var collection1 =
                _entityCollectionService.CreateEntityCollection(
                EntityType.Product,
                Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                "Collection1");

            var collection2 =
                _entityCollectionService.CreateEntityCollection(
                EntityType.Product,
                Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                "Collection2");

            this._original = DbPreTestDataWorker.MakeExistingProduct();
            _original.Barcode = "barcode";
            _original.Manufacturer = "manufacturer";
            _original.ManufacturerModelNumber = "modelnumber";
            _original.CostOfGoods = 1M;
            _original.OnSale = false;
            _original.SalePrice = 2M;
            _original.Taxable = true;
            _original.Shippable = true;
            _original.Height = 2;
            _original.Weight = 2;
            _original.Width = 2;
            _original.Length = 2;
            _original.Available = true;
            _original.Download = false;
            _original.OutOfStockPurchase = true;
            _original.AddToCatalogInventory(catalog);
            this._original.ProductOptions.Add(new ProductOption("Color"));
            this._original.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Blk"));
            this._original.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blu"));
            this._original.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Red", "Red"));
            this._original.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Green", "Gre"));
            this._original.ProductOptions.Add(new ProductOption("Size"));
            this._original.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Sm"));
            this._original.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Medium", "M"));
            this._original.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Large", "Lg"));
            this._original.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("X-Large", "XL"));
            this._original.Height = 20;
            this._original.Weight = 20;
            this._original.Length = 20;
            this._original.Width = 20;
            this._original.Shippable = true;
            _productService.Save(this._original);

            _original.AddToCollection(collection1);
            _original.AddToCollection(collection2);

            this._original.DetachedContents.Add(
                new ProductVariantDetachedContent(
                    this._original.ProductVariantKey,
                    detachedContentType,
                    "en-US",
                    new DetachedDataValuesCollection(
                        new[]
                            {
                                new KeyValuePair<string, string>(
                                    "description",
                                    "\"<p><span>Made with real avocados, this Avocado Moisturizing Bar is great for dry skin. Layers of color are achieved by using oxide colorants. Scented with Wasabi Fragrance Oil, this soap smells slightly spicy, making it a great choice for both men and women. To ensure this soap does not overheat, place in the freezer to keep cool and prevent gel phase.</span></p>\""),
                                new KeyValuePair<string, string>(
                                    "brief",
                                    "\"Avocado Moisturizing Bar is great for dry skin.\""),
                                new KeyValuePair<string, string>(
                                    "image",
                                    "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1035/avocadobars.jpg\" }"), 
                            }))
                {
                    CanBeRendered = true
                });

            this._original.DetachedContents.Add(
                new ProductVariantDetachedContent(
                    this._original.ProductVariantKey,
                    detachedContentType,
                    "en-GB",
                    new DetachedDataValuesCollection(
                        new[]
                            {
                                new KeyValuePair<string, string>(
                                    "description",
                                    "\"<p><span>Made with real avocados, this Avocado Moisturizing Bar is great for dry skin. Layers of color are achieved by using oxide colorants. Scented with Wasabi Fragrance Oil, this soap smells slightly spicy, making it a great choice for both men and women. To ensure this soap does not overheat, place in the freezer to keep cool and prevent gel phase.</span></p>\""),
                                new KeyValuePair<string, string>(
                                    "brief",
                                    "\"Avocado Moisturizing Bar is great for dry skin.\""),
                                new KeyValuePair<string, string>(
                                    "image",
                                    "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1035/avocadobars.jpg\" }"), 
                            }))
                {
                    CanBeRendered = true
                });

            var firstVariant = _original.ProductVariants.First();

            firstVariant.DetachedContents.Add(
                new ProductVariantDetachedContent(
                    firstVariant.Key,
                    detachedContentType,
                    "en-US",
                    new DetachedDataValuesCollection(
                        new[]
                            {
                                new KeyValuePair<string, string>(
                                    "description",
                                    "\"<p><span>US ENGLISH VARIANT Made with real avocados, this Avocado Moisturizing Bar is great for dry skin. Layers of color are achieved by using oxide colorants. Scented with Wasabi Fragrance Oil, this soap smells slightly spicy, making it a great choice for both men and women. To ensure this soap does not overheat, place in the freezer to keep cool and prevent gel phase.</span></p>\""),
                                new KeyValuePair<string, string>(
                                    "brief",
                                    "\"Avocado Moisturizing Bar is great for dry skin.\""),
                                new KeyValuePair<string, string>(
                                    "image",
                                    "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1035/avocadobars.jpg\" }"), 
                            }))
                {
                    CanBeRendered = true
                });

            firstVariant.DetachedContents.Add(
                new ProductVariantDetachedContent(
                    firstVariant.Key,
                    detachedContentType,
                    "en-GB",
                    new DetachedDataValuesCollection(
                        new[]
                            {
                                new KeyValuePair<string, string>(
                                    "description",
                                    "\"<p><span>GB ENGLISH VARIANT Made with real avocados, this Avocado Moisturizing Bar is great for dry skin. Layers of color are achieved by using oxide colorants. Scented with Wasabi Fragrance Oil, this soap smells slightly spicy, making it a great choice for both men and women. To ensure this soap does not overheat, place in the freezer to keep cool and prevent gel phase.</span></p>\""),
                                new KeyValuePair<string, string>(
                                    "brief",
                                    "\"Avocado Moisturizing Bar is great for dry skin.\""),
                                new KeyValuePair<string, string>(
                                    "image",
                                    "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1035/avocadobars.jpg\" }"), 
                            }))
                {
                    CanBeRendered = true
                });

            _productService.Save(this._original);

        }

        [Test]
        public void AssertCopyProductTaskChainTaskCount()
        {
            //// Arrange
            const int Expected = 6;

            //// Act
            var copyChain = new CopyProductTaskChain(
                MerchelloContext.Current,
                _original,
                "test",
                Guid.NewGuid().ToString());

            //// Assert
            Assert.AreEqual(Expected, copyChain.TaskCount);

        }

        [Test, Category("LongRunning")]
        public void Can_Copy_Product_As_A_Clone_Of_Original()
        {
            //// Arrange
            const string Sku = "clone";
            const string Name = "Clone";
            var copyChain = new CopyProductTaskChain(
                          MerchelloContext.Current,
                          _original,
                          Name,
                          Sku);

            //// Act
            var attempt = copyChain.Copy();
            

            //// Assert            
            Assert.IsTrue(attempt.Success);
            var clone = attempt.Result;

            Assert.IsTrue(clone.HasIdentity);
            Assert.AreNotEqual(_original.Key, clone.Key);

            Assert.AreEqual(Name, clone.Name);
            Assert.AreEqual(Sku, clone.Sku);
            Assert.AreEqual(false, clone.Available);

            //// product data
            this.AssertProductBaseData(_original, clone);

            //// product variant data
            foreach (var cloneVariant in clone.ProductVariants.ToArray())
            {
                var originalVariant = this.GetOrignalMatchingVariant(cloneVariant);
                this.AssertProductBaseData(originalVariant, cloneVariant);
            }
            
            //// collections
            this.AssertCollections(_original, clone);

            Assert.AreEqual(_original.Download, clone.Download);
            Assert.AreEqual(_original.OutOfStockPurchase, clone.OutOfStockPurchase);

            Assert.AreEqual(_original.ProductOptions.Count, clone.ProductOptions.Count);
            Assert.AreEqual(_original.ProductVariants.Count, clone.ProductVariants.Count);

            //// Detached Content
            AssertDetachedContent(_original, clone);

        }

        #region Assertion Methods

        private void AssertProductBaseData(IProductBase original, IProductBase clone)
        {
            Assert.AreEqual(original.Price, clone.Price);
            Assert.AreEqual(original.Barcode, clone.Barcode);
            Assert.AreEqual(original.Manufacturer, clone.Manufacturer);
            Assert.AreEqual(original.ManufacturerModelNumber, clone.ManufacturerModelNumber);
            Assert.AreEqual(original.CostOfGoods, clone.CostOfGoods);
            Assert.AreEqual(original.OnSale, clone.OnSale);
            Assert.AreEqual(original.Taxable, clone.Taxable);
            Assert.AreEqual(original.Shippable, clone.Shippable);
            Assert.AreEqual(original.Height, clone.Height);
            Assert.AreEqual(original.Weight, clone.Weight);
            Assert.AreEqual(original.Width, clone.Width);
            Assert.AreEqual(original.Length, clone.Length);
            
            Assert.AreEqual(original.CatalogInventories.Count(), clone.CatalogInventories.Count(), "Inventory counts do not match");

            Assert.AreEqual(original.Download, clone.Download);
            Assert.AreEqual(original.OutOfStockPurchase, clone.OutOfStockPurchase);
        }

        private void AssertCollections(IProduct original, IProduct clone)
        {
            var originalCollections = original.GetCollectionsContaining().ToArray();
            var cloneCollections = clone.GetCollectionsContaining().ToArray();

            Assert.AreEqual(originalCollections.Count(), cloneCollections.Count());
            Assert.IsTrue(originalCollections.All(x => cloneCollections.Any(y => y.Key == x.Key)));

        }

        private void AssertDetachedContent(IProduct original, IProduct clone)
        {
            // product content counts are equal
            Assert.AreEqual(original.DetachedContents.Count, clone.DetachedContents.Count);

            this.AssertDetachedContentValues(original, clone);


            // product variants with content counts are equal
            var originalVariantsWithContent = original.ProductVariants.Where(x => x.DetachedContents.Any()).ToArray();
            var cloneVariantsWithContent = clone.ProductVariants.Where(x => x.DetachedContents.Any()).ToArray();

            Assert.AreEqual(1, originalVariantsWithContent.Count());
            Assert.AreEqual(originalVariantsWithContent.Count(), cloneVariantsWithContent.Count());

            // variants match content
            var originVariant = originalVariantsWithContent.First();
            var cloneVariant = cloneVariantsWithContent.First();

            Assert.AreEqual(2, originVariant.DetachedContents.Count);
            Assert.AreEqual(originVariant.DetachedContents.Count, cloneVariant.DetachedContents.Count);

            AssertDetachedContentValues(originVariant, cloneVariant);
        }

        private void AssertDetachedContentValues(IProductBase original, IProductBase clone)
        {
                        // product contents match
            foreach (var dc in original.DetachedContents.ToArray())
            {
                // this also checks the culture
                var cloneDc = clone.DetachedContents.FirstOrDefault(x => x.CultureName == dc.CultureName);
                Assert.NotNull(cloneDc);

                foreach (var value in dc.DetachedDataValues)
                {
                    var value1 = value;
                    var cloneValue = cloneDc.DetachedDataValues.FirstOrDefault(x => x.Key == value1.Key);
                    Assert.NotNull(cloneValue);
                    Assert.AreEqual(value.Value, cloneValue.Value);
                }
            }
        }

        #endregion

        private IProductVariant GetOrignalMatchingVariant(IProductVariant cloneVariant)
        {
            var skus = cloneVariant.Attributes.Select(x => x.Sku).ToArray();

            return _original.ProductVariants.FirstOrDefault(x => x.Attributes.All(y => skus.Contains(y.Sku)));
        }
    }
}