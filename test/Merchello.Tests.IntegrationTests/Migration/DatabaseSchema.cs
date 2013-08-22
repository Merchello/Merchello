using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Tests.Base.Db;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Tests.IntegrationTests.Migration
{
    [TestFixture]
    public class DatabaseSchema  : DbIntegrationTestBase
    {
       
        [Test]
        public void Can_Drop_All_Database_Tables()
        {
            var deletions = new DatabaseSchemaCreation(Database);
            deletions.UninstallDatabaseSchema();
        }

        [Test]
        public void Successfully_Create_Default_Database_Schema()
        {
            var creation = new DatabaseSchemaCreation(Database);
            creation.InitializeDatabaseSchema();
        }



    }
}
