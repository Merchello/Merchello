using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Tests.IntegrationTests.BootManager
{
    using System.Configuration;

    using Merchello.Core.Persistence.Migrations.Analytics;
    using Merchello.Core.Persistence.UnitOfWork;
    using Merchello.Tests.Base.SqlSyntax;
    using Merchello.Web;

    using NUnit.Framework;

    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;

    [TestFixture]
    public class WebBootManagerTests
    {
        private ISqlSyntaxProvider _sqlSyntax;

        private ILogger _logger;

        private Database _database;

        [TestFixtureSetUp]
        public void Init()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);
            // sets up the Umbraco SqlSyntaxProvider Singleton
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax(syntax);

            _sqlSyntax = SqlSyntaxProviderTestHelper.SqlSyntaxProvider(syntax);

            //AutoMapperMappings.CreateMappings();
            _logger = Logger.CreateWithDefaultLog4NetConfiguration();

            _database = new PetaPocoUnitOfWorkProvider(_logger).GetUnitOfWork().Database;
        }

        [Test]
        public void Can_EnsureDatabaseIsInstalled()
        {
            var manager = new WebMigrationManager(_database, _sqlSyntax, _logger);
            var installed = manager.EnsureDatabase();
            Assert.IsTrue(installed);
        }

        [Test]
        public void Can_CreateAMigrationRecord()
        {
            var record = new MigrationRecord();

            Assert.NotNull(record);
        }
    }
}
