using System;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.Stripe;
using Merchello.Plugin.Payments.Stripe.Models;
using Merchello.Plugin.Payments.Stripe.Provider;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;
using Constants = Merchello.Core.Constants;

namespace Merchello.Tests.Stripe.Integration.TestHelpers
{
    public abstract class ProviderTestsBase
    {
        protected IGatewayProviderSettings GatewayProviderSettings;
        protected IGatewayProviderService GatewayProviderService;
        protected StripePaymentGatewayProvider Provider;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            var cacheProvider = new Mock<IRuntimeCacheProvider>();

            GatewayProviderService = new GatewayProviderService();

            var providers =
                GatewayProviderService.GetAllGatewayProviders()
                    .Where(x => x.GatewayProviderType == GatewayProviderType.Payment);

            GatewayProviderSettings = providers.FirstOrDefault(x => x.Key == new Guid("15C87B6F-7987-49D9-8444-A2B4406941A8"));

            if (GatewayProviderSettings != null)
            {
                GatewayProviderService.Delete(GatewayProviderSettings);
            }

            var petaPoco = new PetaPocoUnitOfWorkProvider();

            var xLogin = ConfigurationManager.AppSettings["xlogin"];
            var xtrankey = ConfigurationManager.AppSettings["xtrankey"];

            var sql = new Sql();

            var dto = new GatewayProviderSettingsDto()
            {
                Key = new Guid("15C87B6F-7987-49D9-8444-A2B4406941A8"),
                Name = "Stripe",
                Description = "Stripe",
                ExtendedData = "<extendedData />",
                EncryptExtendedData = false,
                ProviderTfKey = Constants.TypeFieldKeys.GatewayProvider.PaymentProviderKey,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };


            petaPoco.GetUnitOfWork().Database.Insert(dto);

            GatewayProviderSettings =
                GatewayProviderService.GetGatewayProviderByKey(new Guid("15C87B6F-7987-49D9-8444-A2B4406941A8"));

            var providerSettings = new StripeProcessorSettings()
            {
                // TODO
                //LoginId = xLogin,
                //TransactionKey = xtrankey
            };

            GatewayProviderSettings.ExtendedData.SaveProcessorSettings(providerSettings);

            Provider = new StripePaymentGatewayProvider(GatewayProviderService, GatewayProviderSettings,
                cacheProvider.Object);
        }
    }

}
