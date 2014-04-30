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
            
            StringAssert.AreEqualIgnoringCase(current, version);
        }



    }
}
