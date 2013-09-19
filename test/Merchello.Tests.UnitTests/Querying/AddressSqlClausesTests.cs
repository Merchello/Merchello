using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using Umbraco.Core.Persistence;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Querying
{
    
    [TestFixture]
    [Category("SqlSyntax")]
    public class AddressSqlClausesTests  : BaseUsingSqlServerSyntax
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
    }
}
