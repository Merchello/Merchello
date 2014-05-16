using System;
using System.Linq;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Services;
using Moq;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Service Events")]
    public class OrderServiceEventTests : ServiceTestsBase<IOrder>
    {
        private IOrderService _orderService;


        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var mockSettingService = new Mock<IStoreSettingService>();
            mockSettingService.Setup(x => x.GetNextOrderNumber(1)).Returns(111);

            _orderService = new OrderService(new MockUnitOfWorkProvider(), new RepositoryFactory(), mockSettingService.Object, new Mock<ShipmentService>().Object);
            

            OrderService.StatusChanging += OrderStatusChanging;
            OrderService.StatusChanged += OrderStatusChanged;
        }

        [TestFixtureTearDown]
        public override void FixtureTearDown()
        {
            base.FixtureTearDown();

            OrderService.StatusChanging -= OrderStatusChanging;
            OrderService.StatusChanged -= OrderStatusChanged;
        }

        private void OrderStatusChanging(Object sender, EventArgs args)
        {
            Console.Write(sender.GetType().Name + " " + typeof(StatusChangeEventArgs<>).Name);
            var beforeArgs = args as StatusChangeEventArgs<IOrder>;

            Before = beforeArgs.StatusChangedEntities.First();
        }

        private void OrderStatusChanged(IOrderService sender, StatusChangeEventArgs<IOrder> args)
        {
            After = args.StatusChangedEntities.First();
        }

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            Before = null;
            After = null;

        }

        /// <summary>
        /// Test verifies that saving an order with an updated status does trigger events
        /// </summary>
        [Test]
        public void Changing_Order_Status_Triggers_StatusChange_Events()
        {
            //// Arrange
            var notFulfilled = MockOrderStatusMaker.OrderStatusNotFulfilledMock();
            var fulfulled = MockOrderStatusMaker.OrderStatusFulfilledMock();
            var order = new Order(notFulfilled, Guid.NewGuid()) {Key = Guid.NewGuid()};

            _orderService.Save(order);

            //// Act
            order.OrderStatus = fulfulled;
            _orderService.Save(order);

            //// Assert
            Assert.NotNull(Before, "Before was null");
            Assert.NotNull(After, "After was null");
            Assert.AreEqual(fulfulled.Key, Before.OrderStatusKey);
            Assert.AreEqual(fulfulled.Key, After.OrderStatusKey);

        }



    }
}