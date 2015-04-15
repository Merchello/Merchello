namespace Merchello.Tests.IntegrationTests.Migrations
{
    using Merchello.Core;
    using Merchello.Core.Persistence.Migrations.Initial;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    using Version = System.Version;

    [TestFixture]
    public class MigrationsTest : MerchelloAllInTestBase
    {
        private DatabaseSchemaCreation _databaseSchemaCreation;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var serviceContext = (ServiceContext)MerchelloContext.Current.Services;

            _databaseSchemaCreation = new DatabaseSchemaCreation(serviceContext.DatabaseUnitOfWorkProvider.GetUnitOfWork().Database);
        }

        /// <summary>
        /// Test to check database schema for current Merchello version
        /// </summary>
        [Test]
        public void Can_Determine_The_MerchelloDatabaseVersion()
        {
            //// Arrange
            var expected = new Version(1, 7, 0);
            //// Act
            var result = _databaseSchemaCreation.ValidateSchema();
            Assert.NotNull(result);

            var version = result.DetermineInstalledVersion();

            //// Assert 
            Assert.AreEqual(expected, version);
            
        }

        public void Can_Resolve_OneNineZero_Migration()
        {

        }
    }
}