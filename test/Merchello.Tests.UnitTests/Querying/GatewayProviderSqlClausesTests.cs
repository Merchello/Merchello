using System;
using Merchello.Core.Gateway;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;
using Umbraco.Core.Persistence;
using umbraco.editorControls.tinyMCE3.webcontrol.plugin;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class GatewayProviderSqlClausesTests : BaseUsingSqlServerSyntax<IGatewayProvider>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="GatewayProviderDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_RegisteredGatewayProvider_Base_Clause()
        {
            //// Arrange
            var key = new Guid("F7FC4493-1654-44DF-9547-CB66644676EB");
            var expected = new Sql();
            expected.Select("*")
                .From("[merchGatewayProvider]")
                .Where("[merchGatewayProvider].[pk] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<GatewayProviderDto>()
                .Where<GatewayProviderDto>(x => x.Key == key);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }

        /// <summary>
        /// Test to verify the get by provider sql clause
        /// </summary>
        [Test]
        public void Can_Verify_RegisteredGatewayProvider_GetByGatewayProviderType_Clause()
        {
            //// Arrange
            var key = TypeFieldMock.GatewayProviderPayment.TypeKey;
            var expected = new Sql();
            expected.Select("*")
                .From("[merchGatewayProvider]")
                .Where("[merchGatewayProvider].[gatewayProviderTfKey] = '" + key.ToString() + "'");

            Console.Write(expected.SQL + Environment.NewLine + Environment.NewLine);

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<GatewayProviderDto>()
                .Where<GatewayProviderDto>(x => x.GatewayProviderTfKey == key);

            Console.Write(sql.SQL);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }
    }
}