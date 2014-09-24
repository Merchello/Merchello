using Merchello.Tests.Base.TestHelpers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests
{
    [TestFixture]
    public class MigrationTests 
    {
        //[Test]
        public void Can_Upgrade_VersionOneZeroOne_ToVersionOneoneZero_A_Database()
        {
            var worker = new DbPreTestDataWorker();
            var database = worker.Database;

            var migrationHelper = new MigrationHelper(database);

            migrationHelper.UpgradeTargetVersionOneOneZero();


        }
    }
}