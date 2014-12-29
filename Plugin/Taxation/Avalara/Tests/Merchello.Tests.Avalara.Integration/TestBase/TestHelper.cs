namespace Merchello.Tests.Avalara.Integration.TestBase
{
    using System.Configuration;

    using Merchello.Plugin.Taxation.Avalara.Models;

    public class TestHelper
    {
        public static AvaTaxProviderSettings GetAvaTaxProviderSettings()
        {
            return new AvaTaxProviderSettings()
                {
                    LicenseKey = ConfigurationManager.AppSettings["AvaTax:LicenseKey"],
                    AccountNumber = ConfigurationManager.AppSettings["AvaTax:AccountNumber"],
                    CompanyCode = ConfigurationManager.AppSettings["AvaTax:CompanyCode"],
                    UseSandBox =  true
                };
        }
    }
}