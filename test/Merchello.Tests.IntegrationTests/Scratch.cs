namespace Merchello.Tests.IntegrationTests
{
    using Merchello.Core;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class Scratch : MerchelloContextOnlyBase
    {
        [Test]
        public void Can_Get_MerchelloContext()
        {
            var context = MerchelloContext.Current;
            Assert.NotNull(context);
        }
    }
}