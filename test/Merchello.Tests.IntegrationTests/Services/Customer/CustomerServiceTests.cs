using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Customer
{
    using Umbraco.Core.Serialization;

    [TestFixture]
    [Category("Service Integration")]
    public class CustomerServiceTests : DatabaseIntegrationTestBase
    {
        private ICustomerService _customerService;

        private const string FixtureLoginName = "fixtureTester";

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _customerService = PreTestDataWorker.CustomerService;
        }

        [SetUp]
        public void Initialize()
        {
            PreTestDataWorker.DeleteAllAnonymousCustomers();
            PreTestDataWorker.DeleteAllCustomers();

            // create a customer for retrieval tests
            var customer = _customerService.CreateCustomerWithKey(
                FixtureLoginName,
                "firstName",
                "lastName",
                "test@test.com");
        }

        /// <summary>
        /// Tests verifies that an anonymous customer can be created and persisted
        /// </summary>
        [Test]
        public void Can_Create_An_AnonymousCustomer()
        {
            //// Arrange

            //// Act
            var anonymous = _customerService.CreateAnonymousCustomerWithKey();

            //// Assert
            Assert.NotNull(anonymous);
            Assert.IsTrue(anonymous.HasIdentity);
        }

        [Test]
        public void Can_Retrieve_An_AnonymousCustomer()
        {
            //// Arrange
            var anonymous = _customerService.CreateAnonymousCustomerWithKey();
            var key = anonymous.Key;

            //// Act
            var retrieved = _customerService.GetAnyByKey(key);

            //// Assert
            Assert.NotNull(retrieved);            
        }

        /// <summary>
        /// Test verifies a customer can be created (does not confirm is persisted)
        /// </summary>
        [Test]
        public void Can_Create_A_Customer()
        {
            //// Arrange
            var loginName = "loginName";
            var firstName = "firstName";
            var lastName = "lastName";
            var email = "test@test.com";
            
            //// Act
            var customer = _customerService.CreateCustomer(loginName, firstName, lastName, email);
 
            //// Assert
            Assert.NotNull(customer, "Customer was null");
            Assert.AreEqual(loginName, customer.LoginName);
            Assert.AreEqual(firstName, customer.FirstName);
            Assert.AreEqual(lastName, customer.LastName);
            Assert.AreEqual(email, customer.Email);
        }

        /// <summary>
        /// Test confirms a customer can be created and persisted
        /// </summary>
        [Test]
        public void Can_Create_A_CustomerWithKey()
        {
            //// Arrange
            var loginName = "loginName.test1";
            var firstName = "firstName";
            var lastName = "lastName";
            var email = "test@test.com";
            
            //// Act
            var customer = _customerService.CreateCustomerWithKey(loginName, firstName, lastName, email);


            //// Assert
            Assert.NotNull(customer, "Customer was null");
            Assert.AreEqual(loginName, customer.LoginName, "Login names did not match");
            Assert.AreEqual(firstName, customer.FirstName, "First name did not match");
            Assert.AreEqual(lastName, customer.LastName, "Last name did not match");
            Assert.AreEqual(email, customer.Email, "Email did not match");
            // the big test
            Assert.IsTrue(customer.HasIdentity, "Identity was not set.");
        }

        /// <summary>
        /// Test confirms a customer can be retrieved by their presumably Umbraco login name
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Customer_By_LoginName()
        {
            //// Arrange
            var firstName = "firstName";
            var lastName = "lastName";
            var email = "test@test.com";
            
            //// Act
            var customer = _customerService.GetByLoginName(FixtureLoginName);

            //// Assert
            Assert.NotNull(customer, "customer was null");
            Assert.AreEqual(FixtureLoginName, customer.LoginName);
            Assert.AreEqual(firstName, customer.FirstName);
            Assert.AreEqual(lastName, customer.LastName);
            Assert.AreEqual(email, customer.Email);

        }
    }
}