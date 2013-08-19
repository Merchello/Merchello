using System;
using Merchello.Core.Models;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.Services;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class AddressServiceTests : ServiceTestsBase
    {
        [Test]
        public void Create_Triggers_Event_Assert_And_Address_Is_Passed()
        {
            var address = AddressService.CreateAddress(0, new Guid(), "Home");

            Assert.IsTrue(AfterTriggered);
        }

        [Test]
        public void Save_Triggers_Events_And_Address_Is_Passed()
        {
            var address = AddressData.AddressForInserting();

            AddressService.Save(address);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(address.Id, BeforeAddress.Id);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(address.Label, AfterAddress.Label);
        }

        [Test]
        public void Save_Is_Committed()
        {
            var address = AddressData.AddressForInserting();

            AddressService.Save(address);

            Assert.IsTrue(CommitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Address_Is_Passed()
        {
            var address = AddressData.AddressForUpdating();

            AddressService.Delete(address);


            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(address.Id, BeforeAddress.Id);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(address.Label, AfterAddress.Label);
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var address = AddressData.AddressForUpdating();

            AddressService.Delete(address);

            Assert.IsTrue(CommitCalled);
        }
    }
}
