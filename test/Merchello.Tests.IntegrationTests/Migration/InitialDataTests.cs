using System.Linq;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Tests.Base.Db;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Migration
{
    [TestFixture]
    public class InitialDataTests : DbIntegrationTestBase
    {
        private BaseDataCreation _creation;

        public override void Setup()
        {
           base.Setup();

            _creation = new BaseDataCreation(Database);
        }
        
        [Test]
        public void Can_Populate_typeFieldData_Into_merchDBTypeField()
        {
                     

            _creation.InitializeBaseData("merchDBTypeField");

            var dtos = Database.Query<TypeFieldDto>("SELECT * FROM merchDBTypeField");

            var count = dtos.Count();

            Assert.AreEqual(15, count);
        }

        [Test]
        public void Can_Populate_InvoiceStatusData_Into_merchInvoiceStatus()
        {
            _creation.InitializeBaseData("merchInvoiceStatus");
            var dtos = Database.Query<InvoiceStatusDto>("SELECT * FROM merchInvoiceStatus");

            Assert.IsTrue(dtos.Any());
            Assert.IsTrue(dtos.First().Name == "Unpaid");
            Assert.IsTrue(dtos.First().SortOrder == 1);
            Assert.IsTrue(dtos.Last().Name == "Fraud");
            Assert.IsTrue(dtos.Last().SortOrder == 5);
        }


        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            _creation = null;
        }
    }
}
