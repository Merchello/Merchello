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
        public void Assembly_Version_Correct()
        {
            var version = MerchelloVersion.AssemblyVersion;
            
            StringAssert.AreEqualIgnoringCase("1.0.0.0", version);
        }

    }
}
