namespace Merchello.Tests.UnitTests.Models
{
    using Merchello.Core.Models;

    using NUnit.Framework;

    [TestFixture]
    public class AddressTests
    {
        [Test]
        public void Can_Get_Simple_First_And_LastNames_From_IAddress()
        {
            //// Arrange
            var address = new Address() { Name = "John Doe" };

            //// Act
            var firstName = address.TrySplitFirstName();
            var lastName = address.TrySplitLastName();

            //// Assert
            Assert.AreEqual("John", firstName);
            Assert.AreEqual("Doe", lastName);
        }

        [Test]
        public void Can_Prove_LastName_Is_Empty_If_Not_Provided()
        {
            //// Arrange
            var address = new Address() { Name = "John" };

            //// Act
            var firstName = address.TrySplitFirstName();
            var lastName = address.TrySplitLastName();

            //// Assert
            Assert.AreEqual("John", firstName);
            Assert.IsEmpty(lastName);

        }

        [Test]
        public void Can_Get_A_Multiple_Word_LastName()
        {
            //// Arrange
            var address = new Address() { Name = "John El Macho" };

            //// Act
            var firstName = address.TrySplitFirstName();
            var lastName = address.TrySplitLastName();

            //// Assert
            Assert.AreEqual("John", firstName);
            Assert.AreEqual("El Macho", lastName);
        }
    }
}