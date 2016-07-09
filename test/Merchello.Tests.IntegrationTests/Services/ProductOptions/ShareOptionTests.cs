namespace Merchello.Tests.IntegrationTests.Services.ProductOptions
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class ShareOptionTests : MerchelloAllInTestBase
    {
        private IProductOptionService _productOptionService;

        private Guid _optionKey;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            this._productOptionService = MerchelloContext.Current.Services.ProductOptionService;

            this.DbPreTestDataWorker.DeleteAllSharedOptions();

            var option = this._productOptionService.CreateProductOption("Retrieve", true);
            option.AddChoice("Choice 1", "choice1");
            option.AddChoice("Choice 2", "choice2");
            option.AddChoice("Choice 3", "choice3");
            option.AddChoice("Choice 4", "choice4");
            this._productOptionService.Save(option);

            MerchelloContext.Current.Cache.RuntimeCache.ClearAllCache();

            _optionKey = option.Key;
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Can_Add_A_Shared_Option()
        {
            //// Arrange
            
            //// Act
            var option = this._productOptionService.CreateProductOption("Shared", true);
            option.AddChoice("Shared", "shared");

            this._productOptionService.Save(option);

            //// Assert
            Assert.IsTrue(option.HasIdentity);
            Assert.AreEqual(option.Choices.Count, 1);
        }

        [Test]
        public void Can_Add_A_Shared_Option_With_Multiple_Choices()
        {
            //// Arrange

            //// Act
            var option = this._productOptionService.CreateProductOption("Shared", true);
            option.AddChoice("Shared", "shared");
            option.AddChoice("Shared", "shared");
            option.AddChoice("Shared", "shared");
            option.AddChoice("Shared", "shared");

            this._productOptionService.Save(option);

            //// Assert
            Assert.IsTrue(option.HasIdentity);
            Assert.AreEqual(option.Choices.Count, 4);
        }


        [Test]
        public void Can_Delete_An_Option()
        {
            //// Arrange
            var option = this._productOptionService.CreateProductOption("Shared", true);
            option.AddChoice("Shared", "shared");
            option.AddChoice("Shared", "shared");
            option.AddChoice("Shared", "shared");
            option.AddChoice("Shared", "shared");
            this._productOptionService.Save(option);

            Assert.IsTrue(option.HasIdentity);
            Assert.AreEqual(option.Choices.Count, 4);

            var key = option.Key;

            //// Act
            _productOptionService.Delete(option);

            //// Assert
            var deleted = _productOptionService.GetByKey(key);

            Assert.IsNull(deleted);

        }

        [Test]
        public void Can_Get_An_Option_By_Its_Key()
        {
            //// Arrange

            //// Act
            var retrieved = _productOptionService.GetByKey(_optionKey);

            //// Assert
            Assert.NotNull(retrieved, "Retrieved was null");
            Assert.NotNull(retrieved.Choices, "Choice collection was null");
            Assert.AreEqual(retrieved.Choices.Count, 4, "Did not have any choices");
        }
    }
}