using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Configuration
{
    [TestFixture]
    public class RegionalProvinceConfigurationCollectionTest
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


        //// REMOVED IN VERSION 2.5.0 in prep for new configuration sections.

        ///// <summary>
        ///// Test confirms that the regional province section in the Merchello.config contains 2 elements
        ///// </summary>
        //[Test]
        //public void RegionalProvince_Collection_Contains_2_Elements()
        //{
        //    //// Arrange
        //    const int expected = 2;

        //    //// Act
        //    var count = _config.RegionalProvinces.Count;

        //    //// Assert
        //    Assert.AreEqual(count, expected);
        //}

        ///// <summary>
        ///// Test confirms that the region US has provinces
        ///// </summary>
        //[Test]
        //public void RegionalProvince_Collection_Contains_US_States()
        //{
        //    //// Arrange
        //    const int expected = 62;

        //    //// Act
        //    var us = _config.RegionalProvinces["US"];
        //    Console.Write(us.ProvincesConfiguration.Count);

        //    //// Assert
        //    Assert.NotNull(us);
        //    Assert.AreEqual("US", us.Code);
        //    Assert.AreEqual(expected, us.ProvincesConfiguration.Count);
            
        //}
    }
}