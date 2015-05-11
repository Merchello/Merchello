namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    using System.Linq;

    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.Ui.Payments;

    using NUnit.Framework;

    [TestFixture]
    public class CheckoutOperationControllerResolution : MerchelloAllInTestBase
    {
        [Test]
        public void Can_Resolve_All_CheckoutOperationController_Types()
        {
            var types = CheckoutOperationControllerResolver.Current.GetAllTypes();

            var gooseHead = CheckoutOperationControllerResolver.Current.GetTypeByGatewayMethodUiAlias("GooseHead");

            Assert.IsTrue(types.Any());

            Assert.NotNull(gooseHead);
        }
    }
}