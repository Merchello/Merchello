using Merchello.Core.Configuration;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Configuration
{
    [TestFixture]
    public class Versioning
    {
        [Test]
        public void CurrentVersion_Equals_AssemblyVersion()
        {
            var current = MerchelloVersion.Current.ToString();
            var version = MerchelloVersion.AssemblyVersion;
            
            StringAssert.Contains(current, version);
        }


        [Test]
        public void ConfigurationStatus_Equals_MerchelloVersion()
        {
            Assert.AreEqual(MerchelloConfiguration.ConfigurationStatus, MerchelloVersion.Current.ToString());
        }
    }
}
