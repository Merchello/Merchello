using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.AuthorizeNet.Provider;
using Merchello.Tests.AuthorizeNet.Integration.TestHelpers;
using NUnit.Framework;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.AuthorizeNet.Integration.Authorization
{
    [TestFixture]
    public class AuthorizationTests
    {
        private IGatewayProvider _gatewayProvider;
        private IPaymentMethod _paymentMethod;
        private IGatewayProviderService _gatewayProviderService;
        
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            _gatewayProviderService = new GatewayProviderService();
            var providers =
                _gatewayProviderService.GetAllGatewayProviders()
                    .Where(x => x.GatewayProviderType == GatewayProviderType.Payment);

            _gatewayProvider = providers.FirstOrDefault(x => x.Key == new Guid("C6BF6743-3565-401F-911A-33B68CACB11B"));
            if (_gatewayProvider == null)
            {
                var petaPoco = new PetaPocoUnitOfWorkProvider();
                var sql = new Sql();
                sql.Append(
                    "INSERT INTO merchGatewayProvider (pk, providerTfKey, name, typeFullName, extendedData, encryptExtendedData)");
                sql.Append("VALUES (");
                sql.Append(
                    "'C6BF6743-3565-401F-911A-33B68CACB11B', 'A0B4F835-D92E-4D17-8181-6C342C41606E', 'Merchello.Plugin.Payments.AuthorizeNet.AuthorizeNetPaymentGatewayProvider, Merchello.Plugin.Payments.AuthorizeNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','<extendedData />', 0");

                petaPoco.GetUnitOfWork().Database.Execute(sql);
            }
            
            
            //_paymentMethod = _gatewayProviderService.CreatePaymentMethodWithKey()
        }

        [Test]
        public void Can_Authorize_A_Payment()
        {
            
           // var method = new AuthorizeNetPaymentGatewayMethod(service,)
        }
    }
}
