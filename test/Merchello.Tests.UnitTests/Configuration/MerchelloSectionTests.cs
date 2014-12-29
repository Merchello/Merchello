using System;
using System.Configuration;
using ClientDependency.Core;
using Merchello.Core.Configuration;
using Merchello.Core.Configuration.Outline;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Configuration
{
    using System.Linq;

    [TestFixture]
    [Category("Configuration")]
    public class MerchelloSectionTests
    {
        private MerchelloSection _config;

        /// <summary>
        /// Setup values for each test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _config = ConfigurationManager.GetSection("merchello") as MerchelloSection;
        }



        /// <summary>
        /// Verifies the defaultCountryCode attribute is accessible
        /// </summary>
        [Test]
        public void DefaultCountryCode_Is_US()
        {
            Assert.AreEqual("US", _config.DefaultCountryCode);
        }

        /// <summary>
        /// Verifies the defaultConnectionString matches Umbraco's connection string name
        /// </summary>
        [Test]
        public void ConnectionString_Is_umbracoDbDSN()
        {
            Assert.AreEqual("umbracoDbDSN", _config.DefaultConnectionStringName);
        }

        private enum CustomerAddress
        {
            Residential,
            Commercial
        }

        /// <summary>
        /// Verifies an empty collection 
        /// </summary>
        [Test]
        public void Product_Item_Collection_Is_Empty()
        {
            var productTypeCollection = _config.TypeFields.Product;

            Assert.IsEmpty(productTypeCollection);
        }

        /// <summary>
        /// Test to verify that the DefaultBasketPackagingStrategy can be retrieved
        /// </summary>
        [Test]
        public void Can_Retrieve_DefaultBasketPackagingStrategy_Setting()
        {
            //// Arrage
            const string expected = "Merchello.Core.Strategies.Packaging.DefaultWarehousePackagingStrategy, Merchello.Core";

            //// Act
            var actual = _config.Strategies[Core.Constants.StrategyTypeAlias.DefaultPackaging].Type;

            //// Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test to verify that a task chain can be retrieved by its alias
        /// </summary>
        [Test]
        public void Can_Retrieve_A_TaskChain_By_Alias()
        {
            //// Arrange
            const string alias = "SalesPreparationInvoiceCreate";

            //// Act
            var taskChain = _config.TaskChains[alias];

            //// Assert
            Assert.NotNull(taskChain);
            Assert.AreEqual(typeof(TaskChainElement), taskChain.GetType());

        }

        /// <summary>
        /// Test confirms that teh pattern replacements can be retrieved from the configuration file
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Collection_Of_PatternReplacements()
        {
            //// Arrange
            
            //// Act
            var replacementCollection = _config.PatternFormatter;

            //// Assert
            Assert.NotNull(replacementCollection);
        }

        [Test]
        public void Can_Retrieve_AnonymousCustomerMaxDays()
        {
            //// Arrange
            const string expected = "7";

            //// Act
            var value = MerchelloConfiguration.Current.GetSetting("AnonymousCustomersMaxDays");

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void Can_Retrieve_CustomerMemberTypes()
        {
            //// Arrange
            const string expected = "Customer";

            //// Act
            var values = MerchelloConfiguration.Current.CustomerMemberTypes;

            //// Assert
            Assert.NotNull(values);
            Assert.IsTrue(values.Any());
            Assert.AreEqual(1, values.Count());
            Assert.AreEqual(expected, values.First());
        }
    }


}