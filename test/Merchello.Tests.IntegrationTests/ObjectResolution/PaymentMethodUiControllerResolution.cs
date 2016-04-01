namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    using System.Linq;

    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.Ui;

    using NUnit.Framework;

    [TestFixture]
    public class PaymentMethodUiControllerResolution : MerchelloAllInTestBase
    {
        [Test]
        public void Can_Resolve_All_CheckoutOperationController_Types()
        {
            //// Arrange
            // Handled in base

            //// Act
            var types = PaymentMethodUiControllerResolver.Current.GetAllTypes();            

            //// Assert
            Assert.IsTrue(types.Any());
        }

        [Test]
        public void Can_Retrieve_A_Single_Type_By_The_GatewayMethodUiAttribute_Alias()
        {
            //// Arrange
            // Handled in base

            //// Act
            var mock = PaymentMethodUiControllerResolver.Current.GetTypeByGatewayMethodUiAlias("MockOperationController");

            //// Assert
            Assert.NotNull(mock);
        }

        [Test]
        public void Can_Show_Resolve_Types_With_Method()
        {
            var controller = "MockPaymentMethodUi";
            var method = "FakeMethod";

            var urlParams =
                PaymentMethodUiControllerResolver.Current.GetUrlActionParamsByGatewayMethodUiAliasOnControllerAndMethod(
                    "MockOperation3");

            Assert.AreEqual(controller, urlParams.Controller);
            Assert.AreEqual(method, urlParams.Method);
        }

        [Test]
        public void Can_Show_Resolve_Types_With_DefaultMethod()
        {
            var controller = "MockPaymentMethodUi";
            var method = "RenderForm";

            var urlParams =
                PaymentMethodUiControllerResolver.Current.GetUrlActionParamsByGatewayMethodUiAliasOnControllerAndMethod(
                    "MockOperation2");

            Assert.AreEqual(controller, urlParams.Controller);
            Assert.AreEqual(method, urlParams.Method);
        }

        [Test]
        public void Can_Get_UrlActionParams_From_Resolver()
        {
            //// Arrange
            // Handled in base

            //// Act
            var urlActionParam =
                PaymentMethodUiControllerResolver.Current.GetUrlActionParamsByGatewayMethodUiAlias(
                    "MockOperationController");
            
            Assert.NotNull(urlActionParam);
            Assert.IsTrue(urlActionParam.RouteParams.Any(), "Does not have any route parameters");
            var areaTuple = urlActionParam.RouteParams.First();
            var paramName = areaTuple.Item1;
            var paramValue = areaTuple.Item2;

            //// Assert
            Assert.AreEqual("MockPaymentMethodUi", urlActionParam.Controller);
            Assert.AreEqual("RenderForm", urlActionParam.Method);
            Assert.AreEqual("area", paramName);
            Assert.AreEqual("Mocks", paramValue);            
        }
    }
}