namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class TreesElementTests : MerchelloExtensibilityTests
    {
        [Test]
        public void Trees()
        {
            //// Arrange
            const int expected = 6;

            //// Act
            var trees = ExtensibilitySection.BackOffice.Trees;

            //// Assert
            Assert.AreEqual(expected, trees.Count());

        }
    }
}