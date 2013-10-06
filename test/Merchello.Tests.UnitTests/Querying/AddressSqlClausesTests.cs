using System;
using Lucene.Net.Search;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Querying;
using Merchello.Tests.Base.SqlSyntax;
using Rhino.Mocks;
using Umbraco.Core.Persistence;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Querying
{
    
    [TestFixture]
    [Category("SqlSyntax")]
    public class AddressSqlClausesTests  : BaseUsingSqlServerSyntax<IAddress>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="AddressDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Address_Base_Clause()
        {
            //// Arrange
            var id = 111;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchAddress]")
                .Where("[merchAddress].[id] = 111");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<AddressDto>()
                .Where<AddressDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify that the typed <see cref="IAddress"/> query for addresses by customer
        /// </summary>
        [Test]
        public void Can_Verify_Sql_For_Address_By_Customer_Query()
        {
            //// Arrange
            var key = new Guid("E7ADD433-DF59-42AC-B195-BAF0E4F4392A");

            var expected = new Sql();
            expected.Select("*")
                .From("[merchAddress]")
                .Where("[merchAddress].[customerKey] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<AddressDto>()
                .Where<AddressDto>(x => x.CustomerKey == key);

            
            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }
    }
}
