using System;
using System.Configuration;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Tests.Base.SqlSyntax;
using Merchello.Tests.Base.TestHelpers;

using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.IntegrationTests.A.DbInstall
{
    using Merchello.Core.Persistence.Migrations;

    using Umbraco.Core.Logging;

    [TestFixture]
    public class DatabaseSchema
    {
        private UmbracoDatabase _database;

        private MerchelloDatabaseSchemaHelper _databaseSchemaHelper;

        [TestFixtureSetUp]
        public void Init()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);
            var worker = new DbPreTestDataWorker {SqlSyntax = syntax };
            var logger = Logger.CreateWithDefaultLog4NetConfiguration();
            var sqlSyntax = SqlSyntaxProviderTestHelper.SqlSyntaxProvider(syntax);
            _database = worker.Database;
            _databaseSchemaHelper = new MerchelloDatabaseSchemaHelper(_database, logger, sqlSyntax);

        }

        [Test]
        public void Can_Drop_All_Database_Tables()
        {
            _databaseSchemaHelper.UninstallDatabaseSchema();
        }

        [Test]
        public void Successfully_Create_Default_Database_Schema()
        {
            _databaseSchemaHelper.CreateDatabaseSchema();
        }


        [TestFixtureTearDown]
        public void TearDown()
        {           
            
            _database.Dispose();
        }

    }
}
