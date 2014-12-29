using System;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Taxation.Avalara;
using Merchello.Plugin.Taxation.Avalara.Models;
using Merchello.Plugin.Taxation.Avalara.Models.Address;
using Merchello.Plugin.Taxation.Avalara.Provider;
using Merchello.Web.Models.ContentEditing;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.Avalara.Integration.TestBase
{
    public abstract class ProviderTestsBase : AvaTaxTestBase
    {

        protected AvaTaxTaxationGatewayProvider Provider;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            var cacheProvider = new Mock<IRuntimeCacheProvider>();

            GatewayProviderService = new GatewayProviderService();

            var providers =
                GatewayProviderService.GetAllGatewayProviders()
                    .Where(x => x.GatewayProviderType == GatewayProviderType.Taxation);

            GatewayProviderSettings = providers.FirstOrDefault(x => x.Key == new Guid("DBC48C38-0617-44EA-989A-18AAD8D5DE52"));

            if (GatewayProviderSettings != null)
            {
                GatewayProviderService.Delete(GatewayProviderSettings);
            }

            var petaPoco = new PetaPocoUnitOfWorkProvider();


            var accountNumber = ConfigurationManager.AppSettings["AvaTax:AccountNumber"];
            var licenseKey = ConfigurationManager.AppSettings["AvaTax:LicenseKey"];
            var companyCode = ConfigurationManager.AppSettings["AvaTax:CompanyCode"];

            var sql = new Sql();

            var dto = new GatewayProviderSettingsDto()
            {
                Key = new Guid("DBC48C38-0617-44EA-989A-18AAD8D5DE52"),
                Name = "Avalara AvaTax Provider",
                Description = "Avalara AvaTax Provider",
                ExtendedData = "<extendedData />",
                EncryptExtendedData = false,
                ProviderTfKey = Constants.TypeFieldKeys.GatewayProvider.TaxationProviderKey,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };


            petaPoco.GetUnitOfWork().Database.Insert(dto);

            GatewayProviderSettings =
                GatewayProviderService.GetGatewayProviderByKey(new Guid("DBC48C38-0617-44EA-989A-18AAD8D5DE52"));

            var providerSettings = new AvaTaxProviderSettings()
            {
                LicenseKey = licenseKey,
                AccountNumber = accountNumber,
                CompanyCode = companyCode,
                DefaultStoreAddress = (new Core.Models.Address()
                {
                    Name = "Mindfly, Inc.",
                    Address1 = "114 W. Magnolia St. Suite 300",
                    Locality = "Bellingham",
                    Region = "WA",
                    PostalCode = "98225",
                    CountryCode = "US"
                }).ToTaxAddress() as TaxAddress,
                UseSandBox = true
            };

            GatewayProviderSettings.ExtendedData.SaveProviderSettings(providerSettings);

            GatewayProviderService.Save(GatewayProviderSettings);

            Provider = new AvaTaxTaxationGatewayProvider(GatewayProviderService, GatewayProviderSettings, cacheProvider.Object);

            // ShipCountry
            DataWorker.DeleteAllShipCountries();
            var shipCountryDto = new ShipCountryDto()
            {
                CatalogKey = Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey,
                CountryCode = "US",
                Name = "United States",
                Key = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
            petaPoco.GetUnitOfWork().Database.Insert(shipCountryDto);
        }
    }
}