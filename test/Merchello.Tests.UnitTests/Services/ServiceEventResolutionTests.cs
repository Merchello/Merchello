using System;
using System.Linq;
using System.Reflection;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Services;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Events;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Service Events")]
    public class ServiceEventResolutionTests : ServiceTestsBase<IOrder>
    {
        private IOrderService _orderService;
        

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var mockSettingService = new Mock<IStoreSettingService>();
            mockSettingService.Setup(x => x.GetNextOrderNumber(1)).Returns(111);

            _orderService = new OrderService(new MockUnitOfWorkProvider(), new RepositoryFactory(), mockSettingService.Object, new Mock<ShipmentService>().Object);


            var saved = typeof(OrderService).GetEvent("Saved");
            Assert.NotNull(saved, "Saved could not be found");
            var mi = GetType().GetMethod("OrderServiceOnSaved", BindingFlags.Instance | BindingFlags.NonPublic);
            saved.AddEventHandler(this, Delegate.CreateDelegate(saved.EventHandlerType, this, mi));

        }

        private void OrderServiceOnSaved(IOrderService sender, SaveEventArgs<IOrder> saveEventArgs)
        {
            After = saveEventArgs.SavedEntities.First();
        }

        private void OrderServiceOnCreated(IOrderService sender, Core.Events.NewEventArgs<IOrder> newEventArgs)
        {
            After = newEventArgs.Entity;
        }

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            Before = null;
            After = null;

        }

        [TestFixtureTearDown]
        public override void FixtureTearDown()
        {
            base.FixtureTearDown();

            OrderService.Saved -= OrderServiceOnSaved;
        }

        
        [Test]
        public void Can_Show_Dynamically_Bound_Event_Triggers()
        {
            //// Arrange
            var notFulfilled = MockOrderStatusMaker.OrderStatusNotFulfilledMock();
            var order = new Order(notFulfilled, Guid.NewGuid()) { Key = Guid.NewGuid() };

            //// Act
            _orderService.Save(order);

            //// Assert
            Assert.NotNull(After);
            Assert.AreEqual(After.Key, order.Key);
        }
 
    }
}