using System;
using System.Linq;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.Base.Services;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class AddressServiceTests : ServiceTestsBase<IAddress>
    {
        private AddressService _addressService;
        
        protected override void Initialize()
        {
            _addressService = new AddressService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            Before = null;
            After = null;

            AddressService.Saving += delegate(IAddressService sender, SaveEventArgs<IAddress> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            AddressService.Saved += delegate(IAddressService sender, SaveEventArgs<IAddress> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };


            AddressService.Created += delegate(IAddressService sender, NewEventArgs<IAddress> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            AddressService.Deleting += delegate(IAddressService sender, DeleteEventArgs<IAddress> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            AddressService.Deleted += delegate(IAddressService sender, DeleteEventArgs<IAddress> args)
            {
                AfterTriggered = true;
                After = args.DeletedEntities.FirstOrDefault();
            };

            // General tests
            MockDatabaseUnitOfWork.Committed += delegate(object sender)
            {
                CommitCalled = true;
            };



        }


        [Test]
        public void Create_Triggers_Event_Assert_And_Address_Is_Passed()
        {
            var address = _addressService.CreateAddress(new Guid(), "Billing", new AddressTypeField().Residential, "111 somewhere", string.Empty, "Somewhere", "Outthere", "11111", "US");

            Assert.IsTrue(AfterTriggered);
        }

        [Test]
        public void Save_Triggers_Events_And_Address_Is_Passed()
        {
            var address = MockAddressDataMaker.AddressForInserting();

            _addressService.Save(address);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(address.Id, Before.Id);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(address.Label, After.Label);
        }

        [Test]
        public void Save_Is_Committed()
        {
            var address = MockAddressDataMaker.AddressForInserting();

            _addressService.Save(address);

            Assert.IsTrue(CommitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Address_Is_Passed()
        {
            var address = MockAddressDataMaker.AddressForUpdating();

            _addressService.Delete(address);


            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(address.Id, Before.Id);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(address.Label, After.Label);
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var address = MockAddressDataMaker.AddressForUpdating();

            _addressService.Delete(address);

            Assert.IsTrue(CommitCalled);
        }


    }
}
