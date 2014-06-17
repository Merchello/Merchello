using Merchello.Core.Models;
using Merchello.Web;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Mappers
{
    [TestFixture]
    public class AutoMapperTests
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            AutoMapperMappings.CreateMappings();  
        }

        /// <summary>
        /// Test verifies the a ProductOption can be mapped to a ProductOptionDisplay using the AutoMapper
        /// </summary>
        [Test]
        public void Can_Map_ProductOption_To_ProductOptionDisplay()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
                {
                    new ProductAttribute("One", "One"),
                    new ProductAttribute("Two", "Two"),
                    new ProductAttribute("Three", "Three"),
                    new ProductAttribute("Four", "Four")
                };

            var productOption = new ProductOption("Numbers", true, attributes);

            //// Act
            var productOptionDisplay = AutoMapper.Mapper.Map<ProductOptionDisplay>(productOption);

            //// Assert
            Assert.NotNull(productOptionDisplay);
        }

    }
}