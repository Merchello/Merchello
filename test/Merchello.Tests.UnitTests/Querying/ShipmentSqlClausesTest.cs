using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class ShipmentSqlClausesTest : BaseUsingSqlServerSyntax
    {
        [Test]
        public void Can_Verify_Shipment_Base_Sql_Clause()
        {
            var id = 10;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchShipment]")
                .InnerJoin("[merchShipMethod]").On("[merchShipment].[shipMethodId] = [merchShipMethod].[id]")
                .Where("[merchShipment].[id] = " + id.ToString());

            var sql = new Sql();
            sql.Select("*")
                .From<ShipmentDto>()
                .InnerJoin<ShipMethodDto>()
                .On<ShipmentDto, ShipMethodDto>(left => left.ShipMethodId, right => right.Id)
                .Where<ShipmentDto>(x => x.Id == id);
        }
    }
}
