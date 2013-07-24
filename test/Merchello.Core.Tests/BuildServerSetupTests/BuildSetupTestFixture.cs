using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Configuration;
using NUnit.Framework;

namespace Merchello.Core.Tests.BuildServerSetupTests
{
    [TestFixture]
    public class BuildSetupTestFixture
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
