using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Mappers;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Mappers
{
    [TestFixture]
    [Category("Mappers")]
    public class MerchelloMapperTests
    {
        
        [Test]
        public void Mapper_Resolves_ICustomer_To_CustomerMapper()
        {
            var expected = typeof (CustomerMapper);

            var resolved = MerchelloMappers.ResolveByType(typeof (ICustomer));

            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        [Test]
        public void Mapper_Resolves_IAddress_To_AddressMapper()
        {
            var expected = typeof (AddressMapper);

            var resolved = MerchelloMappers.ResolveByType(typeof (IAddress));

            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        [Test]
        public void Mapper_Resolves_IAnonymousCustomer_To_AnonymousCustomerMapper()
        {
            var expected = typeof (AnonymousCustomerMapper);

            var resolved = MerchelloMappers.ResolveByType(typeof (IAnonymousCustomer));

            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());

        }
    }
}
