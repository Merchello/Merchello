namespace Merchello.Tests.Base.Mocks
{
    using System.Web.Mvc;

    using global::Umbraco.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Web.Mvc;

    [PluginController("Mocks")]
    [GatewayMethodUi("MockOperationController")]
    [GatewayMethodUi("MockOperation2")]
    [GatewayMethodUi("MockOperation3")]
    public class MockPaymentMethodUiController : PaymentMethodUiController<object>
    {
        public override ActionResult RenderForm(object model)
        {
            throw new System.NotImplementedException();
        }


        [GatewayMethodUi("MockOperationController")]
        public ActionResult MockMethod(object model)
        {
            throw new System.NotImplementedException();
        }

        [GatewayMethodUi("MockOperation3")]
        public ActionResult FakeMethod(object model)
        {
            throw new System.NotImplementedException();
        }
    }
}