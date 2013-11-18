using System.Linq;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.IntegrationTests.DbInstall
{
    [TestFixture]
    public class InitialDataTests
    {
        private BaseDataCreation _creation;
        private UmbracoDatabase _database;

        [SetUp]
        public void Init()
        {
            var worker = new DbPreTestDataWorker();
            _database = worker.Database;
            _creation = new BaseDataCreation(_database);
        }

        /// <summary>
        /// Test to verify Merchello 
        /// </summary>
        [Test]
        public void Can_Populate_typeFieldData_Into_merchTypeField()
        {
            //// Arrange
            var expected = 21;

            //// Act
            _creation.InitializeBaseData("merchTypeField");

            //// Assert
            var dtos = _database.Query<TypeFieldDto>("SELECT * FROM merchTypeField");
            var count = dtos.Count();
            Assert.AreEqual(expected, count);
        }

        [Test]
        public void Can_Populate_InvoiceStatusData_Into_merchInvoiceStatus()
        {
            //// Arrange


            //// Act
            _creation.InitializeBaseData("merchInvoiceStatus");
            var dtos = _database.Query<InvoiceStatusDto>("SELECT * FROM merchInvoiceStatus").OrderBy(x => x.SortOrder);

            //// Assert
            Assert.IsTrue(dtos.Any());
            Assert.IsTrue(dtos.First().Name == "Unpaid");
            Assert.IsTrue(dtos.First().SortOrder == 1);
            Assert.IsTrue(dtos.Last().Name == "Fraud");
            Assert.IsTrue(dtos.Last().SortOrder == 5);
        }

        [Test]
        public void Can_Populate_DefaultWarehouse_Into_merchWarehouse()
        {
            //// Arrange
            var expected = 1;

            //// Act
            _creation.InitializeBaseData("merchWarehouse");
            var dtos = _database.Query<WarehouseDto>("SELECT * FROM merchWarehouse");

            //// Assert
            Assert.IsTrue(dtos.Any());
        }

}
}
