using Merchello.Core.Services;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class AddressServiceTests : BaseUsingSqlServerSyntax
    {

        [Test]
        public void CacheDebug()
        {

            var service = new AddressService();

            var address = service.GetById(0);

            var addresses = service.GetAll();


        }


    }
}
