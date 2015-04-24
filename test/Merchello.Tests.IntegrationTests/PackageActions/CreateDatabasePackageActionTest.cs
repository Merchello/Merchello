namespace Merchello.Tests.IntegrationTests.PackageActions
{
    using System;
    using System.Configuration;

    using Merchello.Core.Persistence.Migrations.Initial;
    using Merchello.Tests.Base.SqlSyntax;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.PackageActions;

    using NUnit.Framework;

    using Umbraco.Core.Persistence;

    [TestFixture]
    public class CreateDatabasePackageActionTest
    {
         private UmbracoDatabase _database;

        [TestFixtureSetUp]
        public void Init()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);
            var worker = new DbPreTestDataWorker {SqlSyntax = syntax };
            _database = worker.Database;
            var deletions = new DatabaseSchemaCreation(_database);
            deletions.UninstallDatabaseSchema();
        }

        [Test]
        public void Can_ExecuteThePackageAction()
        {
            var packageAction = new CreateDatabase(_database);
            packageAction.Execute("merchello", null);
        }
    }
}