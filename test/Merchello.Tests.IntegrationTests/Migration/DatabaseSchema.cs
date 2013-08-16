using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Persistence.Migrations.Initial;
using NUnit.Framework;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Tests.IntegrationTests.Migration
{
    [TestFixture]
    public class DatabaseSchema  
    {
        private Database _database;

        [SetUp]
        public void Setup()
        {
            var config = (MerchelloSection) ConfigurationManager.GetSection("merchello");
            var connectionString = ConfigurationManager.ConnectionStrings[config.DefaultConnectionStringName].ConnectionString;

            _database = new Database(connectionString, "System.Data.SqlClient");

            SqlSyntaxContext.SqlSyntaxProvider = new SqlServerSyntaxProvider();
        }

        [Test]
        public void Can_Create_Default_Database_Schema()
        {
            var creation = new DatabaseSchemaCreation(_database);
            creation.InitializeDatabaseSchema();
        }

        [Test]
        public void Can_Drop_All_Database_Tables()
        {
            var deletions = new DatabaseSchemaCreation(_database);
            deletions.UninstallDatabaseSchema();
        }

    }
}
