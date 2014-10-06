using System;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.Chase;
using Merchello.Plugin.Payments.Chase.Models;
using Merchello.Plugin.Payments.Chase.Provider;
using Merchello.Tests.Base.TestHelpers;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;
using Constants = Merchello.Core.Constants;

namespace Merchello.Tests.Chase.Integration.TestHelpers
{
    public abstract class ChaseTestBase : MerchelloAllInTestBase
    {
        protected ChaseProcessorSettings ChaseProcessorSettings;

        protected ICustomer TestCustomer;

        protected Guid CustomerKey = new Guid("D584F356-454B-4D14-BE44-13D9D25D6A74");

        protected ChasePaymentGatewayProvider Provider;

        protected IGatewayProviderSettings GatewayProviderSettings;
        protected IGatewayProviderService GatewayProviderService;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();
                   
            var cacheProvider = new Mock<IRuntimeCacheProvider>();

            GatewayProviderService = new GatewayProviderService();

            var providers =
                GatewayProviderService.GetAllGatewayProviders()
                    .Where(x => x.GatewayProviderType == GatewayProviderType.Payment);

            GatewayProviderSettings = providers.FirstOrDefault(x => x.Key == new Guid("D584F356-454B-4D14-BE44-13D9D25D6A74"));

            if (GatewayProviderSettings != null)
            {
                GatewayProviderService.Delete(GatewayProviderSettings);
            }

            var petaPoco = new PetaPocoUnitOfWorkProvider();

            var merchantId = ConfigurationManager.AppSettings["merchantId"];
            var bin = ConfigurationManager.AppSettings["bin"];
            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];
            var sql = new Sql();

            var dto = new GatewayProviderSettingsDto()
            {
                Key = new Guid("D584F356-454B-4D14-BE44-13D9D25D6A74"),
                Name = "Chase",
                Description = "Chase",
                ExtendedData = "<extendedData />",
                EncryptExtendedData = false,
                ProviderTfKey = Constants.TypeFieldKeys.GatewayProvider.PaymentProviderKey,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };


            petaPoco.GetUnitOfWork().Database.Insert(dto);

            GatewayProviderSettings =
                GatewayProviderService.GetGatewayProviderByKey(new Guid("D584F356-454B-4D14-BE44-13D9D25D6A74"));

            var providerSettings = new ChaseProcessorSettings()
            {
                MerchantId = merchantId,
                Bin = bin,
                Username = username,
                Password = password
            };

            GatewayProviderSettings.ExtendedData.SaveProcessorSettings(providerSettings);

            Provider = new ChasePaymentGatewayProvider(GatewayProviderService, GatewayProviderSettings,
                cacheProvider.Object);

            //TestCustomer = MerchelloContext.Current.Services.CustomerService.CreateCustomerWithKey(
            //    Guid.NewGuid().ToString(),
            //    "debug",
            //    "debug",
            //    "debug@debug.com");
        }
    }
}