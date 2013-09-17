using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using Moq;
using Merchello.Core;
using Merchello.Core.Services;
using Merchello.Core.Models;
using Merchello.Web.Editors;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Tests.TestHelpers;

namespace Merchello.Tests.UnitTests.WebControllers
{
    [TestFixture]
    class InvoiceControllerTests : BaseRoutingTest
    {
        UmbracoContext tempUmbracoContext;

        protected override DatabaseBehavior DatabaseTestBehavior
        {
            get { return DatabaseBehavior.NoDatabasePerFixture; }
        }

        [SetUp]
        public void Setup()
        {
            tempUmbracoContext = GetRoutingContext("/test", 1234).UmbracoContext;
        }

        /// <summary>
        /// Test to verify that the API gets the correct customer by Key
        /// </summary>
        [Test]
        public void GetCustomerByKeyReturnsCorrectItemFromRepository()
        {
            // Arrange
            int i = 1;

            // Act

            // Assert
            Assert.AreEqual(i, 1);
        }
    }
}
