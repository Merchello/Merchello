using System;
using Merchello.Core.Services;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class CustomerServiceTests : BaseUsingSqlServerSyntax
    {
        
        [Test]
        public void CacheDebug()
        {
            
            var service = new CustomerService();

            var customer = service.GetByKey(new Guid("780C2263-C465-44D8-B29C-0A61130481BE"));

            var customers = service.GetAll();

            
        }

        
    }
}
