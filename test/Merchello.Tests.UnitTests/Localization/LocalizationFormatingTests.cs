using System;
using System.Globalization;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Localization
{
    [TestFixture]
    public class LocalizationFormatingTests
    {

        [Test]
        public void Can_Format_Currency_BasedOn_US_CountryCode()
        {
            //// Arrange
            const string countryCode = "US";

            //// Act
            var regionInfo = new RegionInfo(countryCode);

            //var cultureInfo = CultureInfo.CreateSpecificCulture(countryCode);

            //Console.Write(cultureInfo.TwoLetterISOLanguageName);

        }
         
    }
}