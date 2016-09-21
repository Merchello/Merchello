namespace Merchello.Tests.Umbraco.TestHelpers
{
    using global::Umbraco.Core;

    public class TestUmbracoBootManager : CoreBootManager
    {
        public TestUmbracoBootManager(UmbracoApplicationBase umbracoApplication, string baseDirectory)
            : base(umbracoApplication)
        {
            base.InitializeApplicationRootPath(baseDirectory);
        }
    }
}