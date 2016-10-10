using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Tests.IntegrationTests.Dto
{
    using Merchello.Core.Models.Rdbms;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;

    [TestFixture]
    public class KeyOnlyDtoTests : MerchelloAllInTestBase
    {
        private Database _db;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();
            DbPreTestDataWorker.DeleteAllInvoices();

            DbPreTestDataWorker.MakeExistingInvoices(15);

            _db = DbPreTestDataWorker.Database;
        }

        [Test]
        [NUnit.Framework.Ignore]
        public void Can_Get_A_Page_Of_Keys()
        {
            var sql = new Sql();
            sql.Select("*").Append("FROM merchInvoice");

            var page = _db.Page<KeyDto>(1, 10, sql);

            Assert.NotNull(page);
            Assert.AreEqual(15, page.TotalItems);
            Assert.AreEqual(2, page.TotalPages);
        }
    }

    [TableName("merchInvoice")]
    [PrimaryKey("pk", autoIncrement = false)]
    internal class KeyDto : IPageableDto
    {
        [Column("pk")]
        public Guid Key { get; set; }
    }
}
