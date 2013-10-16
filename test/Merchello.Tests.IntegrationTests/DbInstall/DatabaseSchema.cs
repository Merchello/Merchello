﻿using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Tests.Base.SqlSyntax;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.IntegrationTests.DbInstall
{
    [TestFixture]
    public class DatabaseSchema
    {
        private UmbracoDatabase _database;

        [SetUp]
        public void Init()
        {
            var worker = new DbPreTestDataWorker {SqlSyntax = DbSyntax.SqlServer };
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

    }
}
