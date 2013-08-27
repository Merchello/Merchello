using Merchello.Core.Models;
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

        [Test]
        public void Mapper_Resolves_IInvoiceStatus_To_InvoiceStatusMapper()
        {
            var expected = typeof (InvoiceStatusMapper);

            var resolved = MerchelloMappers.ResolveByType(typeof (IInvoiceStatus));

            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }


        [Test]
        public void Mapper_Resolves_IInvoice_To_InvoiceMapper()
        {
            var expected = typeof(InvoiceMapper);

            var resolved = MerchelloMappers.ResolveByType(typeof(IInvoice));

            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }
    }
}
