using System.Configuration;
using System.Linq;
using Merchello.Core.Services;
using Merchello.Plugin.Taxation.Avalara;
using Merchello.Plugin.Taxation.Avalara.Provider;
using Merchello.Tests.Avalara.Integration.TestBase;
using NUnit.Framework;

namespace Merchello.Tests.Avalara.Integration.Tax
{
    public class AvaTaxGatewayProviderTests : ProviderTestsBase
    {
        [SetUp]
        public void Init()
        {
            foreach (var method in Provider.TaxMethods)
            {
                GatewayProviderService.Delete(method);
            }

            Provider.CreateTaxMethod("US");
        }

        /// <summary>
        /// Test confirms that settings can be retrieved from Providers extended data collection
        /// </summary>
        [Test]
        public void Can_Retrieve_ProviderSettings_From_ExtendedData()
        {
            //// Arrange
            // mainly handled in base class
            var accountNumber = ConfigurationManager.AppSettings["AvaTax:AccountNumber"];
            var licenseKey = ConfigurationManager.AppSettings["AvaTax:LicenseKey"];
            var companyCode = ConfigurationManager.AppSettings["AvaTax:CompanyCode"];

            //// Act
            var settings = Provider.ExtendedData.GetAvaTaxProviderSettings();

            //// Assert
            Assert.AreEqual(accountNumber, settings.AccountNumber);
            Assert.AreEqual(licenseKey, settings.LicenseKey);
            Assert.AreEqual(companyCode, settings.CompanyCode);
        }

        /// <summary>
        /// Test confirms that the AvaTax method is returned for the US
        /// </summary>
        [Test]
        public void Can_Prove_Avalara_TaxMethod_Is_Assigned_To_US()
        {
            //// Arrange
            
            //// Act
            var method = Provider.GetGatewayTaxMethodByCountryCode("US");

            //// Assert
            Assert.NotNull(method);
            Assert.AreEqual(typeof(AvaTaxTaxationGatewayMethod), method.GetType());
        }

        /// <summary>
        /// Test confirms that a tax quote can be returned for an invoice
        /// </summary>
        [Test]
        public void Can_Quote_Invoice_For_An_Invoice()
        {
            //// Arrange
            var method = Provider.GetGatewayTaxMethodByCountryCode("US");

            //// Act
            var result = method.CalculateTaxForInvoice(Invoice);

            //// Assert
            Assert.NotNull(result);
            Assert.IsNullOrEmpty(Invoice.InvoiceNumberPrefix);
        }

        [Test]
        public void Can_Finalize_An_Tax_Quote_With_AvaTax()
        {
            //// Arrange
            var method = Provider.GetGatewayTaxMethodByCountryCode("US") as AvaTaxTaxationGatewayMethod;
            var taxAddress = Provider.ExtendedData.GetAvaTaxProviderSettings().DefaultStoreAddress.ToAddress();

            var invoiceService = new InvoiceService();
            invoiceService.Save(Invoice);

            //// Act
            var result = method.CalculateTaxForInvoice(Invoice, taxAddress, false);

            //// Assert
            Assert.NotNull(result);
        }
    }
}