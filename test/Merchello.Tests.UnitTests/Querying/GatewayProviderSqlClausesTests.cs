using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class GatewayProviderSqlClausesTests : BaseUsingSqlServerSyntax<IGatewayProviderSetting>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="GatewayProviderSettingDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_GatewayProvider_Base_Clause()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchGatewayProvider]")
                .Where("[merchGatewayProvider].[pk] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<GatewayProviderSettingDto>()
                .Where<GatewayProviderSettingDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
    }
}