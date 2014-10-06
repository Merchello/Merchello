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
    public class ShipmentSqlClausesTest : BaseUsingSqlServerSyntax<IShipment>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="ShipmentDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Shipment_Base_Sql_Clause()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchShipment]")
                .Where("[merchShipment].[pk] = @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<ShipmentDto>()
                .Where<ShipmentDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        [Test]
        public void Can_Verify_ShipMethod_Base_Sql_Clause()
        {
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchShipMethod]")
                .Where("[merchShipMethod].[pk] = @0", new { key });

            var sql = new Sql();
            sql.Select("*")
                .From<ShipMethodDto>()
                .Where<ShipMethodDto>(x => x.Key == key);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
    }
}
