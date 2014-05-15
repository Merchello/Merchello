using System;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.AuthorizeNet;
using Merchello.Plugin.Payments.AuthorizeNet.Models;
using Merchello.Plugin.Payments.AuthorizeNet.Provider;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;
using Constants = Merchello.Core.Constants;

namespace Merchello.Tests.AuthorizeNet.Integration.TestHelpers
{
    public abstract class ProviderTestsBase
    {
        protected IGatewayProviderSettings GatewayProviderSettings;
        protected IGatewayProviderService GatewayProviderService;
        protected AuthorizeNetPaymentGatewayProvider Provider;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            var cacheProvider = new Mock<IRuntimeCacheProvider>();

            GatewayProviderService = new GatewayProviderService();

            var providers =
                GatewayProviderService.GetAllGatewayProviders()
                    .Where(x => x.GatewayProviderType == GatewayProviderType.Payment);

            GatewayProviderSettings = providers.FirstOrDefault(x => x.Key == new Guid("C6BF6743-3565-401F-911A-33B68CACB11B"));

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
                Key = new Guid("C6BF6743-3565-401F-911A-33B68CACB11B"),
                Name = "AuthorizeNet",
                Description = "AuthorizeNet",
                ExtendedData = "<extendedData />",
                EncryptExtendedData = false,
                ProviderTfKey = Constants.TypeFieldKeys.GatewayProvider.PaymentProviderKey,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };


            petaPoco.GetUnitOfWork().Database.Insert(dto);

            GatewayProviderSettings =
                GatewayProviderService.GetGatewayProviderByKey(new Guid("C6BF6743-3565-401F-911A-33B68CACB11B"));

            var providerSettings = new AuthorizeNetProcessorSettings()
            {
                LoginId = xLogin,
                TransactionKey = xtrankey
            };

            GatewayProviderSettings.ExtendedData.SaveProcessorSettings(providerSettings);

            Provider = new AuthorizeNetPaymentGatewayProvider(GatewayProviderService, GatewayProviderSettings,
                cacheProvider.Object);
        }
    }
}