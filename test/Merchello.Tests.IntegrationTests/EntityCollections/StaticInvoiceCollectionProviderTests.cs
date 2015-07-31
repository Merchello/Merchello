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
        public void Can_Add_Products_To_Collections()
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
            invoice3.SetBillingAddress(billTo);
            var invoice4 = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            invoice4.SetBillingAddress(billTo);

          
            

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


            //// Act
            //var odd = false;
            //foreach (var p in products)
            //{
            //    odd = !odd;
            //    p.AddToCollection(collection1.Key);
            //    if (odd) p.AddToCollection(collection2.Key);
            //}

            ////// Assert
            //var c1Products = provider1.GetEntities().ToArray();
            //var c2Products = provider2.GetEntities().ToArray();
            //Assert.IsTrue(c1Products.Any());
            //Assert.IsTrue(c2Products.Any());

            //Assert.Greater(c1Products.Count(), c2Products.Count());

            //var p1 = c1Products.First();
            //Assert.IsTrue(p1.GetEntityCollections().Any());

        }
    }
}