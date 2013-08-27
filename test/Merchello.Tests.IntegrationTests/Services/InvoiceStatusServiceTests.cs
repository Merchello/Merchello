using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class InvoiceStatusServiceTests : BaseUsingSqlServerSyntax
    {

        private IInvoiceStatusService _invoiceStatusService;
        private IInvoiceStatus _createdStatus;

        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            _invoiceStatusService = new InvoiceStatusService();
            _createdStatus = null;

            InvoiceStatusService.Created += delegate(IInvoiceStatusService sender, NewEventArgs<IInvoiceStatus> args)
                {
                    _createdStatus = args.Entity;
                };
        }

        [Test]
        public void Can_Create_An_InvoiceStatus_And_Event_Returns_Status()
        {
            var status = _invoiceStatusService.CreateInvoiceStatus("test", "test", false, true, 1);

            Assert.AreEqual(status.Alias, _createdStatus.Alias);
            
        }

        [Test]
        public void Can_Save_A_Status()
        {
            var status = _invoiceStatusService.CreateInvoiceStatus("test", "test", false, true, 1);

            _invoiceStatusService.Save(status);
            Assert.IsTrue(status.Id > 0);

        }

        [Test]
        public void Can_Delete_A_Status()
        {
            var status = _invoiceStatusService.CreateInvoiceStatus("test", "test", false, true, 1);

            _invoiceStatusService.Save(status);
            Assert.IsTrue(status.Id > 0);

            _invoiceStatusService.Delete(status);
            
            Assert.IsNull(_invoiceStatusService.GetById(status.Id));
        }

    }
}
