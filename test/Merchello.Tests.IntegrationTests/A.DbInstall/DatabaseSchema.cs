using System;
using System.Configuration;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Tests.Base.SqlSyntax;
using Merchello.Tests.Base.TestHelpers;

using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.IntegrationTests.A.DbInstall
{
    [TestFixture]
    public class DatabaseSchema
    {
        private UmbracoDatabase _database;

        [TestFixtureSetUp]
        public void Init()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);
            var worker = new DbPreTestDataWorker {SqlSyntax = syntax };
            _database = worker.Database;
        }

        [Test]
        public void Can_Drop_All_Database_Tables()
        {
            var deletions = new DatabaseSchemaCreation(_database);
            deletions.UninstallDatabaseSchema();
        }

        [Test]
        public void Successfully_Create_Default_Database_Schema()
        {
            var creation = new DatabaseSchemaCreation(_database);
            creation.InitializeDatabaseSchema();
        }


        [TestFixtureTearDown]
        public void TearDown()
        {           
            
            _database.Dispose();
        }

    }
}
