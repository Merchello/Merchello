namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class BackOfficeElementTests : MerchelloExtensibilityTests
    {
        [Test]
        public void Trees()
        {
            //// Arrange
            const int expected = 6;

            //// Act
            var trees = ExtensibilitySection.BackOffice.Trees;

            //// Assert
            Assert.NotNull(trees);
            Assert.AreEqual(expected, trees.Count());

        }

        [Test]
        public void EnableProductOptionUiElementSelection()
        {
            //// Arrange
            const bool expected = false;

            //// Act
            var value = ExtensibilitySection.BackOffice.EnableProductOptionUiElementSelection;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void ProductOptionUi()
        {
            Assert.NotNull(ExtensibilitySection.BackOffice.ProductOptionUi);
            Assert.IsTrue(ExtensibilitySection.BackOffice.ProductOptionUi.Any());
        }
    }
}