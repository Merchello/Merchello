namespace Merchello.Tests.IntegrationTests.Migrations
{
    using Merchello.Core;
    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.Migrations;
    using Merchello.Core.Persistence.Migrations.Initial;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Migrations;

    using Version = System.Version;

    [TestFixture]
    public class MigrationsTest : MerchelloAllInTestBase
    {
        private DatabaseSchemaCreation _databaseSchemaCreation;

        private MigrationResolver _resolver;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var serviceContext = (ServiceContext)MerchelloContext.Current.Services;
            _resolver = new MigrationResolver(
                Logger.CreateWithDefaultLog4NetConfiguration(),
                PluginManager.Current.ResolveMerchelloMigrations());
        }

        [Test]
        public void Can_Resolve_MerchelloOnlyMigrations()
        {
            foreach (var t in _resolver.InstanceTypes)
            {
                Assert.AreEqual(MerchelloConfiguration.MerchelloMigrationName, t.GetCustomAttribute<MigrationAttribute>(false).ProductName);
            }
        }

        [Test]
        public void Can_Get_Ordered_Migrations_For_1_12_0()
        {
            var oneEleven = new Version(1, 11, 0);
            var oneTwelve = new Version(1, 12, 0);

            //var ordered = _resolver.OrderedUpgradeMigrations(oneEleven, oneTwelve);


            //foreach (var m in ordered)
            //{
            //    Assert.AreEqual(oneTwelve, m.GetType().GetCustomAttribute<MigrationAttribute>(false).TargetVersion);
            //}
        }
     
    }
}