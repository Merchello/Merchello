using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.Base.Services;
using NUnit.Framework;
using Umbraco.Core.Events;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class InvoiceServiceTests : ServiceTestsBase<IInvoice>
    {
        private InvoiceService _invoiceService;
        private IAnonymousCustomer _anonymous;
        private bool _statusBefore;
        private bool _statusAfter;
        private ICustomer _customer;
        private IInvoiceStatus _invoiceStatus;
        private IInvoice _invoice;
        private IEnumerable<IInvoice> _beforeInvoice;
        private IEnumerable<IInvoice> _afterInvoice;

        protected override void Initialize()
        {
            _invoiceService = new InvoiceService(new MockUnitOfWorkProvider(), new RepositoryFactory());

            _invoice = _invoiceService.CreateInvoice(_customer, _invoiceStatus, "test111", "name", "address1",
              "address2", "city", "state", "98225", "US", "test@test.com", string.Empty, string.Empty);

            Before = null;
            After = null;
            _beforeInvoice = null;
            _afterInvoice = null;
            _statusBefore = false;
            _statusAfter = false;

            _customer = MockCustomerDataMaker.CustomerForInserting().MockSavedWithKey(111);

            _invoiceStatus = MockInvoiceStatusDataMaker.InvoiceStatusUnpaidMock();

            _anonymous = MockAnonymousCustomerDataMaker.AnonymousCustomerForInserting().MockSavedWithKey(Guid.NewGuid());

            InvoiceService.Saving += delegate(IInvoiceService sender, SaveEventArgs<IInvoice> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            InvoiceService.Saved += delegate(IInvoiceService sender, SaveEventArgs<IInvoice> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };


            InvoiceService.Created += delegate(IInvoiceService sender, Core.Events.NewEventArgs<IInvoice> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            InvoiceService.Deleting += delegate(IInvoiceService sender, DeleteEventArgs<IInvoice> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            InvoiceService.Deleted += delegate(IInvoiceService sender, DeleteEventArgs<IInvoice> args)
            {
                AfterTriggered = true;
                After = args.DeletedEntities.FirstOrDefault();
            };

            InvoiceService.StatusChanging += delegate(IInvoiceService sender, StatusChangeEventArgs<IInvoice> args) {  
                _statusBefore = true;
                _beforeInvoice = args.StatusChangedEntities;
            };

            InvoiceService.StatusChanged += delegate(IInvoiceService sender, StatusChangeEventArgs<IInvoice> args)
            {
                _statusAfter = true;
                _afterInvoice = args.StatusChangedEntities;
            };

            // General tests
            MockDatabaseUnitOfWork.Committed += delegate {
                CommitCalled = true;
            };


        }

        [Test]
        public void Can_Create_An_Invoice_And_Event_Invoice_Is_Passed()
        {

            var invoice = _invoiceService.CreateInvoice(_customer, _invoiceStatus, "test111", "name", "address1",
                "address2", "city", "state", "98225", "US", "test@test.com", string.Empty, string.Empty);

            Assert.IsTrue(AfterTriggered);
            Assert.IsTrue("address1" == After.BillToAddress1);
        }


        [Test]
        public void Save_Invoice_Triggers_Events_And_Invoice_Is_Passed()
        {

            _invoiceService.Save(_invoice);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(_invoice.InvoiceNumber, Before.InvoiceNumber);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(_invoice.InvoiceDate, After.InvoiceDate);
        }

        [Test]
        public void Save_Is_Committed()
        {

            CommitCalled = false;

           _invoiceService.Save(_invoice);

            Assert.IsTrue(CommitCalled);

        }




        [Test]
        public void Delete_Triggers_Events_And_Invoice_Is_Passed()
        {


            _invoice.Id = 12;

            _invoice.ResetDirtyProperties();

            CommitCalled = false;

            _invoiceService.Delete(_invoice);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(_invoice.Id, Before.Id);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(_invoice.InvoiceStatus, After.InvoiceStatus);

            Assert.IsTrue(CommitCalled);
        }

        [Test]
        public void Updating_InvoiceStatus_Triggers_Events_And_Invoice_Is_Passed()
        {

            _invoice.Id = 12;
            _invoice.ResetDirtyProperties();
            CommitCalled = false;

            _invoice.InvoiceStatus = MockInvoiceStatusDataMaker.InvoiceStatusCompletedMock();

            _invoiceService.Save(_invoice);

            Assert.IsTrue(_statusBefore);
            var firstOrDefault = _beforeInvoice.FirstOrDefault();
            if (firstOrDefault != null)
                Assert.AreEqual(_invoice.InvoiceNumber, firstOrDefault.InvoiceNumber);
            else
            {
                Assert.Fail();
            }

            Assert.IsTrue(_statusAfter);
            var orDefault = _afterInvoice.FirstOrDefault();
            if (orDefault != null)
                Assert.AreEqual(_invoice.InvoiceDate, orDefault.InvoiceDate);
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void Event_Handler_Returns_Which_Invoices_In_Collection_Have_Status_Changed()
        {
            var invoices = new List<IInvoice>()
            {
                {MakeFakeInvoice(1)},
                {MakeFakeInvoice(2)},
                {MakeFakeInvoice(3)},
                {MakeFakeInvoice(4)},
                {MakeFakeInvoice(5)}
            };

            invoices[1].InvoiceStatus = MockInvoiceStatusDataMaker.InvoiceStatusCompletedMock();
            invoices[2].InvoiceStatus = MockInvoiceStatusDataMaker.InvoiceStatusCompletedMock();
            invoices[4].InvoiceStatus = MockInvoiceStatusDataMaker.InvoiceStatusCompletedMock();

            _invoiceService.Save(invoices);

            Console.Write(_beforeInvoice.Count().ToString());

            Assert.IsTrue(3 ==_beforeInvoice.Count());
            Assert.IsTrue(invoices[1].InvoiceStatus.Name == _beforeInvoice.First().InvoiceStatus.Name);
            Assert.IsTrue(invoices[4].InvoiceStatus.Name == _beforeInvoice.Last().InvoiceStatus.Name);
        }


        private IInvoice MakeFakeInvoice(int id)
        {
            var invoice = CloneHelper.DeepClone<IInvoice>(_invoice);
            invoice.Id = id;
            invoice.ResetDirtyProperties();
            return invoice;
        }

        [Test]
        public void Delete_Is_Committed()
        {
            _invoice.Id = 12;

            _invoice.ResetDirtyProperties();

            CommitCalled = false;

            _invoiceService.Delete(_invoice);

            Assert.IsTrue(CommitCalled);
        }




    }
}
