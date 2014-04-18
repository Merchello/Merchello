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
        protected IGatewayProvider GatewayProvider;
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

            GatewayProvider = providers.FirstOrDefault(x => x.Key == new Guid("C6BF6743-3565-401F-911A-33B68CACB11B"));

            if (GatewayProvider != null)
            {
                GatewayProviderService.Delete(GatewayProvider);
            }

            var petaPoco = new PetaPocoUnitOfWorkProvider();

            var xLogin = ConfigurationManager.AppSettings["xlogin"];
            var xtrankey = ConfigurationManager.AppSettings["xtrankey"];

            var sql = new Sql();

            var dto = new GatewayProviderDto()
            {
                Key = new Guid("C6BF6743-3565-401F-911A-33B68CACB11B"),
                Name = "AuthorizeNet",
                Description = "AuthorizeNet",
                TypeFullName =
                    "Merchello.Plugin.Payments.AuthorizeNet.AuthorizeNetPaymentGatewayProvider, Merchello.Plugin.Payments.AuthorizeNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                ExtendedData = "<extendedData />",
                EncryptExtendedData = false,
                ProviderTfKey = Constants.TypeFieldKeys.GatewayProvider.PaymentProviderKey,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };


            petaPoco.GetUnitOfWork().Database.Insert(dto);

            GatewayProvider =
                GatewayProviderService.GetGatewayProviderByKey(new Guid("C6BF6743-3565-401F-911A-33B68CACB11B"));

            var providerSettings = new AuthorizeNetProcessorSettings()
            {
                LoginId = xLogin,
                TransactionKey = xtrankey
            };

            GatewayProvider.ExtendedData.SaveProcessorSettings(providerSettings);

            Provider = new AuthorizeNetPaymentGatewayProvider(GatewayProviderService, GatewayProvider,
                cacheProvider.Object);
        }
    }
}