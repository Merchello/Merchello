﻿using System.Security.Cryptography;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Services;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class WarehouseSqlClauseTests : BaseUsingSqlServerSyntax<IWarehouse>
    {
        /// <summary>
        /// Test verifies the warehouse base sql select clause
        /// </summary>
        [Test]
        public void Can_Verify_Warehouse_Base_Clause()
        {
            //// Arrange
            var id = 1000;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchWarehouse]")
                .Where("[merchWarehouse].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<WarehouseDto>()
                .Where<WarehouseDto>(x => x.Id == id);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }

       
    }
}