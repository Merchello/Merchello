namespace Merchello.Tests.Avalara.Integration
{
    using Merchello.Plugin.Taxation.Avalara.Services;

    using NUnit.Framework;

    public abstract class ApiTestBase
    {
        protected IAvaTaxService AvaTaxService;

        [TestFixtureSetUp]
        public void Init()
        {
            this.AvaTaxService = new AvaTaxService(TestHelper.GetAvaTaxProviderSettings());
        }
 
    }
}