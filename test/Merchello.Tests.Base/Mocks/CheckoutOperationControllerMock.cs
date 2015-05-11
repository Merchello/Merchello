namespace Merchello.Tests.Base.Mocks
{
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Web.Mvc;

    [GatewayMethodUi("GooseHead")]
    public class CheckoutOperationControllerMock : CheckoutOperationControllerBase
    {
        public override ActionResult RenderForm()
        {
            throw new System.NotImplementedException();
        }
    }
}