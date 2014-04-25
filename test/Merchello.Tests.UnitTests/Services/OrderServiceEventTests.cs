using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Services;
using Moq;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class OrderServiceEventTests : ServiceTestsBase<IOrder>
    {
        private IOrderService _orderService;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {

            var mockStoreSettingService = new Mock<StoreSettingService>();
            mockStoreSettingService.Setup(x => x.GetNextOrderNumber(1)).Returns(111);

            _orderService = new OrderService(new MockUnitOfWorkProvider(), new RepositoryFactory(), new Mock<IStoreSettingService>().Object, new Mock<IShipmentService>().Object);

        }

        [Test]
        public void Setup()
        {
            Before = null;
            After = null;

        }
    }
}