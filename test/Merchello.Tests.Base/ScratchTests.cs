using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Tests.Base
{
    using NUnit.Framework;

    [TestFixture]
    public class ScratchTests
    {
        [Test]
        public void LookAtDateFormat()
        {
            var dt = DateTime.Now;
            var tomorrow = dt.AddDays(1);
            Assert.NotNull(dt);
        }
    }
}
