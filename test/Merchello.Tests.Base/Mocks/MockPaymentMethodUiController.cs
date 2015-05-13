namespace Merchello.Tests.Base.Mocks
{
    using System.Web.Mvc;

    using global::Umbraco.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Web.Mvc;

    [PluginController("Mocks")]
    [GatewayMethodUi("MockOperationController")]
    public class MockPaymentMethodUiController : PaymentMethodUiController<object>
    {
        public override ActionResult RenderForm(object model)
        {
            throw new System.NotImplementedException();
        }
    }
}