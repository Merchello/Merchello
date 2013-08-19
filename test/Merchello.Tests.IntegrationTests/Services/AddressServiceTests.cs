using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class AddressServiceTests : BaseUsingSqlServerSyntax
    {

        [Test]
        public void CacheDebug()
        {

            var service = new AddressService();

            var address = service.GetByKey(0);

            var addresses = service.GetAll();


        }


    }
}
