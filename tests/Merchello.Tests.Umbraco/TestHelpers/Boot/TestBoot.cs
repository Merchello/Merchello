namespace Merchello.Tests.Umbraco.TestHelpers.Boot
{
    using global::Umbraco.Core;

    using Merchello.Umbraco.Boot;
    using Merchello.Web.Boot;

    internal class TestBoot : UmbracoBootManager
    {
        public TestBoot(ApplicationContext appContext)
            : base(new BootSettings { IsForTesting = true }, appContext)
        {
        }
    }
}