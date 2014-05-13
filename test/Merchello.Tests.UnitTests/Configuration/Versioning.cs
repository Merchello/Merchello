using System;
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
            //// Arrage
            
            //// Act
            var configStatus = MerchelloConfiguration.ConfigurationStatus;
            var currentVersion = MerchelloVersion.Current.ToString();
            
            Console.Write("Config status {0} - ", configStatus);
            Console.Write("Current Version {0} - ", currentVersion);

            //// Assert
            Assert.AreEqual(configStatus, currentVersion);
        }
    }
}
