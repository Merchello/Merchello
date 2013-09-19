using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class ShipmentServiceTests : ServiceIntegrationTestBase
    {
        private IShipmentService _shipmentService;
        private IInvoice _invoice;
        private ICustomer _customer;
        private IEnumerable<IInvoiceStatus> _statuses;
        private IShipMethod _shipMethod;

        [SetUp]
        public void Setup()
        {
            _shipmentService = new ShipmentService();

            var invoiceStatusService = new InvoiceStatusService();
            _statuses = invoiceStatusService.GetAll();
            if (!_statuses.Any())
            {
                // populate the table
                var creation = new BaseDataCreation(new PetaPocoUnitOfWorkProvider().GetUnitOfWork().Database);
                creation.InitializeBaseData("merchInvoiceStatus");
                _statuses = invoiceStatusService.GetAll();
            }

            var customerService = new CustomerService();
            _customer = CustomerData.CustomerForInserting();
            customerService.Save(_customer);

            var invoiceService = new InvoiceService();
            var all = invoiceService.GetAll().ToArray();
            invoiceService.Delete(all);

            var unpaid = _statuses.FirstOrDefault(x => x.Alias == "unpaid");

            _invoice = invoiceService.CreateInvoice(_customer, unpaid, "test111", "name", "address1",
              "address2", "city", "state", "98225", "US", "test@test.com", string.Empty, string.Empty);

            invoiceService.Save(_invoice);

            var shipMethodService = new ShipMethodService();
            _shipMethod = ShipmentData.MockShipMethodForInserting();
            shipMethodService.Save(_shipMethod);

        }

        [Test]
        public void Can_Create_A_Shipment()
        {
            var expected = ShipmentData.MockShipmentForInserting(_invoice.Id, null);

            var shipment = _shipmentService.CreateShipment(null, _invoice, AddressData.MindflyAddressForInserting());

            Assert.AreEqual(expected.Address1, shipment.Address1);
        }

        [Test]
        public void Can_Save_A_Shipment()
        {

            IShipment savedShipment = null;

            ShipmentService.Saved += delegate(IShipmentService sender, SaveEventArgs<IShipment> args)
                {
                    savedShipment = args.SavedEntities.FirstOrDefault();
                };

            var shipment = _shipmentService.CreateShipment(_shipMethod, _invoice, AddressData.MindflyAddressForInserting());

            _shipmentService.Save(shipment);

            Assert.NotNull(savedShipment);

            var key = shipment.Key;

            Assert.AreEqual(savedShipment.Key, key);
            
        }

        [Test]
        public void Can_Delete_A_Shipment()
        {
            IShipment deletedShipment = null;

            ShipmentService.Deleted += delegate(IShipmentService sender, DeleteEventArgs<IShipment> args)
                {
                    deletedShipment = args.DeletedEntities.FirstOrDefault();
                };

            var shipment = _shipmentService.CreateShipment(null, _invoice, AddressData.MindflyAddressForInserting());

            _shipmentService.Save(shipment);
            Assert.IsTrue(shipment.HasIdentity);

            var key = shipment.Key;
            _shipmentService.Delete(shipment);

            Assert.NotNull(deletedShipment);
            Assert.AreEqual(key, deletedShipment.Key);

        }

        [Test]
        public void Can_Update_A_Shipment()
        {
            IShipment updatedShipment = null;

            ShipmentService.Saved += delegate(IShipmentService sender, SaveEventArgs<IShipment> args)
                {
                    updatedShipment = args.SavedEntities.FirstOrDefault();
                };

            var shipment = _shipmentService.CreateShipment(_shipMethod, _invoice, AddressData.MindflyAddressForInserting());

            _shipmentService.Save(shipment);
            updatedShipment = null;

            shipment.Address1 += " Suite 504";
            shipment.Address2 = null;

            _shipmentService.Save(shipment);
            Assert.NotNull(updatedShipment);
            Assert.AreEqual(shipment.Address1, updatedShipment.Address1);
            Assert.IsTrue(string.IsNullOrEmpty(shipment.Address2));
        }

        [Test]
        public void Can_Get_A_List_Of_ShipMethod_Shipments()
        {
            var nullShipMethodShipments = new List<IShipment>()
            {
                new Shipment(_invoice.Id),
                new Shipment(_invoice.Id),
                new Shipment(_invoice.Id),
                new Shipment(_invoice.Id)
            };

            _shipmentService.Save(nullShipMethodShipments);

            var shipment = _shipmentService.CreateShipment(_shipMethod, _invoice, AddressData.MindflyAddressForInserting());
            _shipmentService.Save(shipment);

            var shipments = _shipmentService.GetShipmentsForShipMethod(_shipMethod.Id);

            Assert.IsTrue(shipments.Any());

            var ids = shipments.Select(x => x.ShipMethodId).Distinct();

            Assert.IsTrue(ids.Count() == 1);
            Assert.IsTrue(ids.First() == _shipMethod.Id);
        }

        [Test]
        public void Can_Get_A_List_Of_Invoice_Shipments()
        {
            var nullShipMethodShipments = new List<IShipment>()
            {
                new Shipment(_invoice.Id),
                new Shipment(_invoice.Id),
                new Shipment(_invoice.Id),
                new Shipment(_invoice.Id)
            };

            _shipmentService.Save(nullShipMethodShipments);

            var shipments = _shipmentService.GetShipmentsForInvoice(_invoice.Id);

            Assert.IsTrue(shipments.Any());

            var ids = shipments.Select(x => x.Id).Distinct();

            Assert.IsTrue(ids.Count() == 4);
        }


    }
}
