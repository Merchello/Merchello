using Merchello.Core.Models;
using Merchello.Tests.Base.Services;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    public class ProductServiceTests
    {
        [Test]
        public void Can_Create_A_Product()
        {
            var service = new MockProductService();
            var product = service.CreateProduct("1111", "Rusty's product", 12M);

            Assert.NotNull(product);
            Assert.IsFalse(product.HasIdentity);
            Assert.IsFalse(((Product)product).MasterVariant.HasIdentity);
        }
    }
}