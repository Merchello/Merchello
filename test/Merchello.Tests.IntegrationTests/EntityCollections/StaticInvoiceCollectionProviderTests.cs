namespace Merchello.Tests.IntegrationTests.EntityCollections
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class StaticInvoiceCollectionProviderTests : MerchelloAllInTestBase
    {
        private IEntityCollectionService _entityCollectionService;

        private IInvoiceService _invoiceService;

        private Guid _providerKey;

        private IEntityCollectionProviderResolver _resolver;


        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _entityCollectionService = DbPreTestDataWorker.EntityCollectionService;
            _invoiceService = DbPreTestDataWorker.InvoiceService;
            _resolver = EntityCollectionProviderResolver.Current;
            _providerKey = _resolver.GetProviderKey<StaticInvoiceCollectionProvider>();                       
        }

        [SetUp]
        public void Setup()
        {
            DbPreTestDataWorker.DeleteAllEntityCollections();
            DbPreTestDataWorker.DeleteAllInvoices();
        }

        [Test]
        public void Can_Create_An_Entity_Collection()
        {
            //// Arrage


            //// Act
            var collection = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Invoice,
                _providerKey,
                "Test Collection");

            //// Assert
            Assert.NotNull(collection);
            Assert.IsTrue(collection.HasIdentity);
            Assert.AreEqual("Test Collection", collection.Name);

            var provider = collection.ResolveProvider();

            Assert.NotNull(provider);
            Assert.IsTrue(provider.GetManagedCollections().Any());
        }

        [Test]
        public void Can_Add_Invoices_To_Collections()
        {
            //// Arrange
            var billTo = new Address() { Address1 = "test", CountryCode = "US", PostalCode = "11111" };

            var invoice = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            invoice.SetBillingAddress(billTo);
            _invoiceService.Save(invoice);

            var invoice2 = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            invoice2.SetBillingAddress(billTo);
            _invoiceService.Save(invoice2);
            
            var invoice3 = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            _invoiceService.Save(invoice3);
            invoice3.SetBillingAddress(billTo);
            
            var invoice4 = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            invoice4.SetBillingAddress(billTo);
            _invoiceService.Save(invoice4);
          
            

            var collection1 = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Invoice,
                _providerKey,
                "Invoice Collection1");

            var provider1 = collection1.ResolveProvider<StaticInvoiceCollectionProvider>();
            Assert.NotNull(provider1);
            Assert.AreEqual(typeof(StaticInvoiceCollectionProvider), provider1.GetType());

            var collection2 = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Invoice,
                _providerKey,
                "Invoice Collection2");

            var provider2 = collection2.ResolveProvider<StaticInvoiceCollectionProvider>();
            Assert.NotNull(provider2);
            Assert.AreEqual(typeof(StaticInvoiceCollectionProvider), provider2.GetType());

            invoice.AddToCollection(collection1);
            invoice2.AddToCollection(collection1);
            invoice3.AddToCollection(collection1);
            invoice4.AddToCollection(collection1);

            invoice3.AddToCollection(collection2);
            invoice4.AddToCollection(collection2);

            //// Assert
            var c1Invoices = collection1.GetEntities<IInvoice>().ToArray();
            var c2Invoices = collection2.GetEntities<IInvoice>().ToArray();
            Assert.IsTrue(c1Invoices.Any());
            Assert.IsTrue(c2Invoices.Any());

            Assert.Greater(c1Invoices.Count(), c2Invoices.Count());

            var i1 = c1Invoices.First();
            Assert.IsTrue(i1.GetCollectionsContaining().Any());

        }

        [Test]
        public void Can_Remove_Invoices_From_A_Collection()
        {
            //// Arrange
            //// Arrange
            var billTo = new Address() { Address1 = "test", CountryCode = "US", PostalCode = "11111" };

            var invoice = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            invoice.SetBillingAddress(billTo);
            _invoiceService.Save(invoice);

            var invoice2 = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            invoice2.SetBillingAddress(billTo);
            _invoiceService.Save(invoice2);

            var invoice3 = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            _invoiceService.Save(invoice3);
            invoice3.SetBillingAddress(billTo);

            var invoice4 = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            invoice4.SetBillingAddress(billTo);
            _invoiceService.Save(invoice4);

            var collection1 = _entityCollectionService.CreateEntityCollectionWithKey(
              EntityType.Invoice,
              _providerKey,
              "Invoice Collection1");

            
            invoice.AddToCollection(collection1);
            invoice2.AddToCollection(collection1);
            invoice3.AddToCollection(collection1);
            invoice4.AddToCollection(collection1);
            var provider = collection1.ResolveProvider<StaticInvoiceCollectionProvider>();
            Assert.NotNull(provider);

            //// Act

            var cinvoices = collection1.GetEntities<IInvoice>().ToArray();
            Assert.AreEqual(4, cinvoices.Count());

            var remove = cinvoices.First();
            var key = remove.Key;

            remove.RemoveFromCollection(collection1);

            //// Assert
            var afterRemove = collection1.GetEntities<IInvoice>().ToArray();
            Assert.AreEqual(3, afterRemove.Count());
            Assert.False(afterRemove.Any(x => x.Key == key));
            Assert.IsFalse(collection1.Exists(remove));
            Assert.IsFalse(remove.GetCollectionsContaining().Any());
        }
    }
}