namespace Merchello.Tests.Umbraco
{
    using global::Umbraco.Core.Logging;

    using Merchello.Core;
    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Tests.Umbraco.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class ScratchTests : UmbracoInstanceBase
    {
        [Test]
        public void LogTest()
        { 
            Logger.Info<ScratchTests>("Logging test");

            Assert.NotNull(IoC.Current);


            Assert.NotNull(ApplicationContext.DatabaseContext, "DatabaseContext was null");
            Assert.NotNull(ApplicationContext.DatabaseContext.SqlSyntax, "SqlSyntax was null");

            var mappingResolver = IoC.Container.GetInstance<MappingResolver>();

            Assert.NotNull(mappingResolver);
        }
    }
}
